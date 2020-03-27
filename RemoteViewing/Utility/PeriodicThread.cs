#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.Diagnostics;
using System.Threading;

namespace RemoteViewing.Utility
{
    /// <summary>
    /// Represents a <seealso cref="Thread"/> that invokes a specific action at a
    /// specific interval.
    /// </summary>
    internal sealed class PeriodicThread
    {
        private ManualResetEvent requestExit;
        private AutoResetEvent requestUpdate;
        private Thread requestThread;

        /// <summary>
        /// Invokes the <paramref name="action"/> at a frequency specified by <paramref name="getUpdateRateFunc"/>;
        /// optionally waiting for a signal to execute the action.
        /// </summary>
        /// <param name="action">
        /// The action to run at the frequency specifie.
        /// </param>
        /// <param name="getUpdateRateFunc">
        /// A function that returns at which frequency (in Hz) the <paramref name="action"/> should
        /// be performed.
        /// </param>
        /// <param name="useSignal">
        /// <see langword="true"/> to wait for <see cref="Signal"/> before executing the action;
        /// otherwise, <see langword="false"/>.
        /// </param>
        public void Start(Action action, Func<double> getUpdateRateFunc, bool useSignal)
        {
            Throw.If.Null(action, "action").Null(getUpdateRateFunc, "getUpdateRateFunc");

            this.requestExit = new ManualResetEvent(false);
            this.requestUpdate = new AutoResetEvent(false);
            this.requestThread = new Thread(() =>
            {
                var waitHandles = new WaitHandle[] { this.requestUpdate, this.requestExit };

                while (true)
                {
                    long startTime = Stopwatch.GetTimestamp();
                    if (useSignal && WaitHandle.WaitAny(waitHandles) == 1)
                    {
                        return;
                    }

                    try
                    {
                        action();
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    var elapsedTime = Math.Max(0, Stopwatch.GetTimestamp() - startTime);
                    var secondsToWait = (1.0 / getUpdateRateFunc()) - ((double)elapsedTime / Stopwatch.Frequency);
                    int timeout = Math.Max(0, Math.Min(60000, (int)Math.Round(1000.0 * secondsToWait)));
                    if (timeout > 0)
                    {
                        if (this.requestExit.WaitOne(timeout))
                        {
                            return;
                        }
                    }
                }
            });
            this.requestThread.IsBackground = true;
            this.requestThread.Start();
        }

        /// <summary>
        /// Signals the <see cref="PeriodicThread"/> that an action should be performed.
        /// </summary>
        public void Signal()
        {
            if (this.requestUpdate != null)
            {
                this.requestUpdate.Set();
            }
        }

        /// <summary>
        /// Stops this <see cref="PeriodicThread"/>.
        /// </summary>
        public void Stop()
        {
            if (this.requestThread == null)
            {
                return;
            }

            this.requestExit.Set();
            this.requestThread.Join();
        }
    }
}
