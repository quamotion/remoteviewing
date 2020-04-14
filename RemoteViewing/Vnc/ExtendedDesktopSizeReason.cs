#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
Copyright (c) 2020 Quamotion bvba <http://quamotion.mobi>
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
    /// Indicates the reason for a change in screen layout.
    /// </summary>
    public enum ExtendedDesktopSizeReason : int
    {
        /// <summary>
        /// The screen layout was changed via non-RFB means on the server. For example the server may have provided means for server-side
        /// applications to manipulate the screen layout. This code is also used when the client sends a non-incremental FrameBufferUpdateRequest
        /// to learn the server's current state.
        /// </summary>
        External = 0,

        /// <summary>
        /// The client receiving this message requested a change of the screen layout. The change may or may not have happened depending on server
        /// policy or available resources. The status code in the y-position field must be used to determine which.
        /// </summary>
        Client = 1,

        /// <summary>
        /// Another client requested a change of the screen layout and the server approved it. A rectangle with this code is never sent if
        /// the server denied the request.
        /// </summary>
        OtherClient = 2,
    }
}
