#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
Copyright (c) 2020 Quamotion bvba <https://github.com/quamotion/remoteviewing>
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
using System.Net;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// A shared interface for VNC servers which can host one or more VNC sessions.
    /// </summary>
    public interface IVncServer : IDisposable
    {
        /// <summary>
        /// Occurs when the VNC client has successfully connected to the server.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Occurs when the VNC client is disconnected.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Occurs when the VNC client provides a password.
        /// Respond to this event by accepting or rejecting the password.
        /// </summary>
        event EventHandler<PasswordProvidedEventArgs> PasswordProvided;

        /// <summary>
        /// Gets a list of all VNC sessions.
        /// </summary>
        IReadOnlyList<IVncServerSession> Sessions { get; }

        /// <summary>
        /// Starts the VNC server.
        /// </summary>
        /// <param name="endPoint">
        /// A <see cref="IPEndPoint"/> which represents the port at which the VNC server should listen.
        /// </param>
        void Start(IPEndPoint endPoint);

        /// <summary>
        /// Stops the VNC server.
        /// </summary>
        void Stop();
    }
}
