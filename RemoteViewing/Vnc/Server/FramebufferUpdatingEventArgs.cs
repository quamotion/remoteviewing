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

using System.ComponentModel;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Provides data for the <see cref="VncServerSession.FramebufferUpdating"/> event.
    /// </summary>
    public class FramebufferUpdatingEventArgs
#if !NETSTANDARD1_5
        : HandledEventArgs
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FramebufferUpdatingEventArgs"/> class.
        /// </summary>
        public FramebufferUpdatingEventArgs()
        {
        }

#if NETSTANDARD1_5
        /// <summary>
        /// Gets or sets a value indicating whether the event is handled.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether you send an update in response to this event.
        /// </summary>
        /// <value>
        /// Set this to <c>true</c> if you send an update in response to this event.
        /// </value>
        public bool SentChanges
        {
            get;
            set;
        }
    }
}
