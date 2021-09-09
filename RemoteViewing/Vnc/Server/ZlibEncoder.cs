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

using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;
using System.IO;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Implements the zlib encoding protocol.
    /// </summary>
    /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#zlib-encoding"/>
    internal class ZlibEncoder : VncEncoder
    {
        private MemoryStream buffer;
        private ZlibStream deflater;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZlibEncoder"/> class.
        /// </summary>
        public ZlibEncoder()
        {
            this.buffer = new MemoryStream();
            this.deflater = new ZlibStream(this.buffer, CompressionMode.Compress);
            this.deflater.FlushMode = FlushType.Full;
        }

        /// <inheritdoc/>
        public override VncEncoding Encoding => VncEncoding.Zlib;

        /// <inheritdoc/>
        public override int Send(Stream stream, VncPixelFormat pixelFormat, VncRectangle region, byte[] contents)
        {
            this.buffer.SetLength(0);
            this.deflater.Write(contents, 0, contents.Length);
            this.deflater.Flush();
            this.buffer.Position = 0;

            byte[] length = new byte[4];
            VncUtility.EncodeUInt32BE(length, 0, (uint)this.buffer.Length);
            stream.Write(length, 0, 4);

            this.buffer.CopyTo(stream);

            return (int)this.buffer.Length + 4;
        }
    }
}