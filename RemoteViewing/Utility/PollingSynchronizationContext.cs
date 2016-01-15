using System;
using System.Collections.Generic;
using System.Threading;

namespace RemoteViewing.Utility
{
    /// <summary>
    /// A synchronization context that can be used if you want to handle events by polling.
    /// </summary>
    public class PollingSynchronizationContext : SynchronizationContext
    {
        private Queue<Action> posts = new Queue<Action>();

        /// <summary>
        /// Dequeues and runs an action, if one is queued.
        /// </summary>
        /// <param name="timeout">The number of milliseconds to wait for an action to run.</param>
        /// <returns><c>true</c> if an action ran.</returns>
        public bool CheckEvents(int timeout)
        {
            Action action;

            lock (this.posts)
            {
                if (this.posts.Count == 0)
                {
                    if (timeout == 0 || !Monitor.Wait(this.posts, timeout))
                    {
                        return false;
                    }
                }

                action = this.posts.Dequeue();
            }

            action();
            return true;
        }

        /// <summary>
        /// Posts an action to run when <see cref="PollingSynchronizationContext.CheckEvents"/> is called.
        /// This will not block.
        /// </summary>
        /// <param name="d">The callback.</param>
        /// <param name="state">State to pass to the callback.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            this.Post(() => d(state));
        }

        /// <summary>
        /// Sends an action to run when <see cref="PollingSynchronizationContext.CheckEvents"/> is called.
        /// This will block until then. This will lock up if you call it from the same thread you use for
        /// checking events.
        /// </summary>
        /// <param name="d">The callback.</param>
        /// <param name="state">State to pass to the callback.</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            var call = new SynchronizedCall(d, state);

            this.Post(call_ => ((SynchronizedCall)call_).Run(), call);

            call.Wait();
        }

        /// <summary>
        /// Post an action to run when <see cref="PollingSynchronizationContext.CheckEvents"/> is called.
        /// This will not block.
        /// </summary>
        /// <param name="action">The action to run.</param>
        private void Post(Action action)
        {
            lock (this.posts)
            {
                this.posts.Enqueue(action);
                Monitor.Pulse(this.posts);
            }
        }
    }
}
