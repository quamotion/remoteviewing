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

using System.IO;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Represents the raw VNC encoding protocol.
    /// </summary>
    /// <remarks>
    /// The simplest encoding type is raw pixel data. In this case the data consists of width * height pixel values
    /// (where width and height are the width and height of the rectangle). The values simply represent each pixel
    /// in left-to-right scanline order.
    /// </remarks>
    /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#raw-encoding"/>.
    internal class RawEncoder : VncEncoder
    {
        /// <inheritdoc/>
        public override VncEncoding Encoding => VncEncoding.Raw;

        /// <inheritdoc/>
        public override void Send(Stream stream, VncPixelFormat pixelFormat, VncRectangle region, byte[] contents)
        {
            stream.Write(contents, 0, contents.Length);
        }
    }
}
