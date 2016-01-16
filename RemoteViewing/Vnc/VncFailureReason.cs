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
    /// Possible reasons a <see cref="VncException"/> is thrown.
    /// </summary>
    public enum VncFailureReason
    {
        /// <summary>
        /// Unknown reason.
        /// </summary>
        Unknown,

        /// <summary>
        /// The server isn't a VNC server.
        /// </summary>
        WrongKindOfServer,

        /// <summary>
        /// RemoteViewing can't speak the protocol versions this server offers.
        /// </summary>
        UnsupportedProtocolVersion,

        /// <summary>
        /// The server offered no authentication methods. This could mean that VNC is temporarily disabled.
        /// </summary>
        ServerOfferedNoAuthenticationMethods,

        /// <summary>
        /// The server offered no supported authentication methods.
        /// </summary>
        NoSupportedAuthenticationMethods,

        /// <summary>
        /// A password was required to authenticate but wasn't supplied.
        /// </summary>
        PasswordRequired,

        /// <summary>
        /// Authentication failed. This could mean you supplied an incorrect password.
        /// </summary>
        AuthenticationFailed,

        /// <summary>
        /// The server specified a pixel format RemoteViewing doesn't support.
        /// </summary>
        UnsupportedPixelFormat,

        /// <summary>
        /// A network error occured. The connection may have been lost.
        /// </summary>
        NetworkError,

        /// <summary>
        /// The server sent a value that seems unreasonable. This shouldn't happen in normal conditions.
        /// </summary>
        SanityCheckFailed,

        /// <summary>
        /// The server sent an unrecognized protocol element. This shouldn't happen in normal conditions.
        /// </summary>
        UnrecognizedProtocolElement
    }
}
