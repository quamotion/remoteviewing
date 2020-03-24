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

using System;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Control flags used by the Tight compression protocol.
    /// </summary>
    [Flags]
    internal enum TightCompressionControl : byte
    {
        /// <summary>
        /// Informs the client zlib compression stream at index 0 should be reset before decoding the rectangle.
        /// </summary>
        ResetStream0 = 0b_0001,

        /// <summary>
        /// Informs the client zlib compression stream at index 1 should be reset before decoding the rectangle.
        /// </summary>
        ResetStream1 = 0b_0010,

        /// <summary>
        /// Informs the client zlib compression stream at index 2 should be reset before decoding the rectangle.
        /// </summary>
        ResetStream2 = 0b_0100,

        /// <summary>
        /// Informs the client zlib compression stream at index 3 should be reset before decoding the rectangle.
        /// </summary>
        ResetStream3 = 0b_10100,

        /// <summary>
        /// Informs the client zlib compression stream at index 0 should be used to decode the rectangle.
        /// </summary>
        UseStream0 = 0b_0000_0000,

        /// <summary>
        /// Informs the client zlib compression stream at index 1 should be used to decode the rectangle.
        /// </summary>
        UseStream1 = 0b_0001_0000,

        /// <summary>
        /// Informs the client zlib compression stream at index 2 should be used to decode the rectangle.
        /// </summary>
        UseStream2 = 0b_0010_0000,

        /// <summary>
        /// Informs the client zlib compression stream at index 3 should be used to decode the rectangle.
        /// </summary>
        UseStream3 = 0b_0011_0000,

        /// <summary>
        /// Informs the client the next byte specifies the <c>filter-id</c> value which tells the decoder
        /// what filter type was used by the encoder to pre-process pixel data before the compression.
        /// </summary>
        ReadFilterId = 0b_0100_0000,

        /// <summary>
        /// Informs the client basic compression has been used.
        /// </summary>
        BasicCompression = 0b_0000_0000,

        /// <summary>
        /// Informs the client fill compression has been used.
        /// </summary>
        FillCompression = 0b_1000_0000,

        /// <summary>
        /// Informs the client JPEG compression has been used.
        /// </summary>
        JpegCompression = 0b_1001_0000,
    }
}
