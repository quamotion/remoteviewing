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
    sealed class PeriodicThread
    {
        ManualResetEvent _requestExit;
        AutoResetEvent _requestUpdate;
        Thread _requestThread;

        public void Start(Func<bool> action, Func<double> getUpdateRateFunc, bool useSignal)
        {
            Throw.If.Null(action, "action").Null(getUpdateRateFunc, "getUpdateRateFunc");

            _requestExit = new ManualResetEvent(false);
            _requestUpdate = new AutoResetEvent(false);
            _requestThread = new Thread(() =>
            {
                var waitHandles = new WaitHandle[] { _requestUpdate, _requestExit };

                while (true)
                {
                    long startTime = Stopwatch.GetTimestamp();
                    if (useSignal && WaitHandle.WaitAny(waitHandles) == 1) { return; }

                    bool didAction;
                    try { didAction = action(); } catch (Exception) { return; }

                    var elapsedTime = Math.Max(0, Stopwatch.GetTimestamp() - startTime);
                    var secondsToWait = 1.0 / getUpdateRateFunc() - (double)elapsedTime / Stopwatch.Frequency;
                    int timeout = Math.Max(0, Math.Min(60000, (int)Math.Round(1000.0 * secondsToWait)));
                    if (timeout > 0)
                    {
                        if (didAction) // Rate limit if true.
                        {
                            if (_requestExit.WaitOne(timeout)) { return; }
                        }
                        else
                        {
                            if (WaitHandle.WaitAny(waitHandles) == 1) { return; }
                        }
                    }
                }
            });
            _requestThread.IsBackground = true;
            _requestThread.Start();
        }

        public void Signal()
        {
            if (_requestUpdate != null) { _requestUpdate.Set(); }
        }

        public void Stop()
        {
            if (_requestThread == null) { return; }

            _requestExit.Set();
            _requestThread.Join();
        }
    }
}
