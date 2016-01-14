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
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Called when a password is required and <see cref="VncClientConnectOptions.Password"/> is <c>null</c>.
    /// </summary>
    /// <param name="client">The client needing a password.</param>
    /// <returns>The password, or <c>null</c> to not supply one.</returns>
    public delegate char[] PasswordRequiredCallback(VncClient client);

    /// <summary>
    /// Specifies options for connecting to a VNC server.
    /// </summary>
    public sealed class VncClientConnectOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VncClientConnectOptions"/> class.
        /// </summary>
        public VncClientConnectOptions()
        {
            ShareDesktop = true;
        }

        /// <summary>
        /// The password to authenticate with, if the server requires one.
        /// 
        /// If this is <c>null</c> and a password is required, the connection will fail.
        /// <c>null</c> is different from a zero-character password.
        /// 
        /// Only the first eight characters of a password are meaningful in
        /// traditional VNC authentication.
        /// </summary>
        public char[] Password
        {
            get;
            set;
        }

        /// <summary>
        /// Called when a password is required and <see cref="VncClientConnectOptions.Password"/> is <c>null</c>.
        /// </summary>
        public PasswordRequiredCallback PasswordRequiredCallback
        {
            get;
            set;
        }

        /// <summary>
        /// <c>true</c> to share the desktop with any currently-connected clients.
        /// <c>false</c> to get exclusive access to the desktop.
        /// 
        /// This is set to <c>true</c> by default.
        /// </summary>
        public bool ShareDesktop
        {
            get;
            set;
        }
    }
}
