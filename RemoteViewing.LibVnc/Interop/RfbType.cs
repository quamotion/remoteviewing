#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2020 Quamotion bvba
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
    /// Represents a type used by libvnc.
    /// </summary>
    internal enum RfbType
    {
        /// <summary>
        /// The field is not used and shoudl be skipped. Can be used to 'undefine' fields.
        /// </summary>
        Skip,

        /// <summary>
        /// The field is a pointer.
        /// </summary>
        Pointer,

        /// <summary>
        /// The field is a 32-bit integer.
        /// </summary>
        Int,

        /// <summary>
        /// The field is of type <c>rfbPixel</c>.
        /// </summary>
        Pixel,

        /// <summary>
        /// The field is of type <c>rfbPixelFormat</c>.
        /// </summary>
        PixelFormat,

        /// <summary>
        /// The field is of type <c>rfbColourMap</c>.
        /// </summary>
        ColourMap,

        /// <summary>
        /// The field is a character array of 255 chars.
        /// </summary>
        Char_255,

        /// <summary>
        /// The field is of type <c>rfbBool</c>.
        /// </summary>
        Bool,

        /// <summary>
        /// The field is of type <c>rfbSocket</c>.
        /// </summary>
        Socket,

        /// <summary>
        /// The field is of type <c>fd_set</c>.
        /// </summary>
        FdSet,

        /// <summary>
        /// The field is of type <c>rfbSocketState</c>.
        /// </summary>
        SocketState,

        /// <summary>
        /// The field is of type <c>sockaddr_in</c>.
        /// </summary>
        SockAddrIn,

        /// <summary>
        /// The field is of type <c>MUTEX()</c>.
        /// </summary>
        Mutex,

        /// <summary>
        /// The field is of type <c>in_addr_t</c>.
        /// </summary>
        InAddrT,

        /// <summary>
        /// The field is of type float.
        /// </summary>
        Float,

        /// <summary>
        /// The field is of type <c>timeval</c>.
        /// </summary>
        Timeval,

        /// <summary>
        /// The field consists of 30000 bytes.
        /// </summary>
        Byte_30000,

        /// <summary>
        /// The field is of type <c>z_stream</c>.
        /// </summary>
        Z_stream_s,

        /// <summary>
        /// The field is an array of 4 <c>z_stream</c> objects.
        /// </summary>
        ZsStruct_4,

        /// <summary>
        /// The field is an array of 4 booleans.
        /// </summary>
        Bool_4,

        /// <summary>
        /// The field is an array of 4 integers.
        /// </summary>
        Int_4,

        /// <summary>
        /// The field is of type <c>rfbFileTransferData</c>.
        /// </summary>
        RfbFileTransferData,

        /// <summary>
        /// The field is of type <c>pthread_cond_t</c>.
        /// </summary>
        Pthread_cond_t,

        /// <summary>
        /// The field is an array of [rfbZRLETileWidth * rfbZRLETileHeight] integers.
        /// </summary>
        Int_ZRLE,

        /// <summary>
        /// The field is an array of 2 integers.
        /// </summary>
        Int_2,

        /// <summary>
        /// The field is an array of CHALLENGESIZE bytes.
        /// </summary>
        Byte_CHALLENGESIZE,

        /// <summary>
        /// The field is of type <c>pthread_t</c>.
        /// </summary>
        PthreadT,
    }
}
