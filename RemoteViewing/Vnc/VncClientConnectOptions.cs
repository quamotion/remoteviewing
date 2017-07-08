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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Called when a password is required and <see cref="VncClientConnectOptions.Password"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="client">The client needing a password.</param>
    /// <returns>The password, or <see langword="null"/> to not supply one.</returns>
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
            this.ShareDesktop = true;
        }

        /// <summary>
        /// Gets or sets the password to authenticate with, if the server requires one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is <see langword="null"/> and a password is required, the connection will fail.
        /// <see langword="null"/> is different from a zero-character password.
        /// </para>
        /// <para>
        /// Only the first eight characters of a password are meaningful in
        /// traditional VNC authentication.
        /// </para>
        /// </remarks>
        public char[] Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a polling thread should be started and frame buffer updates
        /// are published via events or the frame buffer updates are triggered manually (on demand) by calling the respective functions.
        /// </summary>
        public bool OnDemandMode { get; set; }

        /// <summary>
        /// Gets or sets a callback which is called when a password is required and
        /// <see cref="VncClientConnectOptions.Password"/> is <see langword="null"/>.
        /// </summary>
        public PasswordRequiredCallback PasswordRequiredCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the session can be shared with
        /// any currently-connected clients.
        /// </summary>
        /// <value>
        /// <c>true</c> to share the desktop with any currently-connected clients.
        /// <c>false</c> to get exclusive access to the desktop.
        /// </value>
        /// <remarks>
        /// This is set to <c>true</c> by default.
        /// </remarks>
        public bool ShareDesktop
        {
            get;
            set;
        }
    }
}
