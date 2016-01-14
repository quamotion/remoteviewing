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
using System.Threading;

namespace RemoteViewing.Utility
{
    /// <summary>
    /// Marshals a single call from one thread to another.
    /// </summary>
    public class SynchronizedCall
    {
        private SendOrPostCallback callback; private object state;

        private ManualResetEvent @event = new ManualResetEvent(false);
        private Exception ex;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedCall"/> class.
        /// </summary>
        /// <param name="callback">The call to marshal.</param>
        /// <param name="state">The state to provide to the call. <see langword="null"/> is fine if you don't need to include any state.</param>
        public SynchronizedCall(SendOrPostCallback callback, object state)
        {
            Throw.If.Null(callback, "callback");

            this.callback = callback;
            this.state = state;
        }

        /// <summary>
        /// Runs the call in the current thread, and marshals the exception back if one occurs.
        /// </summary>
        public void Run()
        {
            try
            {
                this.callback(this.state);
            }
            catch (Exception ex)
            {
                this.ex = ex;
            }
            finally
            {
                this.@event.Set();
            }
        }

        /// <summary>
        /// Waits for <see cref="SynchronizedCall.Run"/> to complete.
        /// </summary>
        public void Wait()
        {
            this.@event.WaitOne();

            var ex = this.ex;
            if (ex != null)
            {
                throw ex;
            }
        }
    }
}
