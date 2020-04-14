#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2017 Quamotion bvba <http://www.quamotion.mobi/>
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

using Microsoft.Extensions.Logging;
using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// Contains all the input required to host a noVNC endpoint.
    /// </summary>
    public class VncContext
    {
        /// <summary>
        /// Gets or sets the <see cref="IVncFramebufferSource"/> which provides the framebuffer data.
        /// </summary>
        public IVncFramebufferSource FramebufferSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the a function which creates a new <see cref="IVncFramebufferCache"/> for use with
        /// the <see cref="FramebufferSource"/>.
        /// </summary>
        public Func<VncFramebuffer, ILogger, IVncFramebufferCache> CreateFramebufferCache
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IVncRemoteController"/> which allows you to remotely control the VNC server.
        /// </summary>
        public IVncRemoteController RemoteController
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IVncRemoteKeyboard"/> which allows you to remotely control the VNC server.
        /// </summary>
        public IVncRemoteKeyboard RemoteKeyboard
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password required to connect to the noVNC endpoint.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an optional delegate which configures a <see cref="VncServerSession"/> after the session
        /// has been created.
        /// </summary>
        public Action<VncServerSession> SessionConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logger to use when logging diagnostic messages. No logging will happen when
        /// set to <see langword="null"/>.
        /// </summary>
        public ILogger Logger
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IVncPasswordChallenge"/> to use when creating or validating VNC passwords.
        /// The default <see cref="VncPasswordChallenge"/> will be used when set to <see langword="null"/>.
        /// </summary>
        public IVncPasswordChallenge PasswordChallenge
        {
            get;
            set;
        }
    }
}
