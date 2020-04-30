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

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// Enumerates possible client states.
    /// </summary>
    public enum RfbClientRecState
    {
        /// <summary>
        /// The client is establishing the protocol version.
        /// </summary>
        RFB_PROTOCOL_VERSION,

        /// <summary>
        /// The client is negotiating security.
        /// </summary>
        RFB_SECURITY_TYPE,

        /// <summary>
        /// The client is authenticating.
        /// </summary>
        RFB_AUTHENTICATION,

        /// <summary>
        /// The client is sending initialisation messages.
        /// </summary>
        RFB_INITIALISATION,

        /// <summary>
        /// The client is sending normal protocol messages.
        /// </summary>
        RFB_NORMAL,

        /* Ephemeral internal-use states that will never be seen by software
         * using LibVNCServer to provide services: */

        /// <summary>
        /// The client is sending initialisation messages with implicit shared-flag already true
        /// </summary>
        RFB_INITIALISATION_SHARED,
    }
}
