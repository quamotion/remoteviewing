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

using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System.IO;
using System.Text;
using Xunit;

namespace RemoteViewing.Tests.Vnc.Server
{
    /// <summary>
    /// Tests the <see cref="ZlibEncoder"/> class.
    /// </summary>
    public class ZlibEncoderTests
    {
        /// <summary>
        /// Tests the <see cref="ZlibEncoder.Encoding"/> property.
        /// </summary>
        [Fact]
        public void EncodingTest()
        {
            var zlib = new ZlibEncoder();
            Assert.Equal(VncEncoding.Zlib, zlib.Encoding);
        }

        /// <summary>
        /// Tests the <see cref="ZlibEncoder.Send(Stream, VncPixelFormat, byte[])"/> method.
        /// </summary>
        [Fact]
        public void SendTest()
        {
            var encoder = new ZlibEncoder();

            byte[] raw1 = null;
            byte[] raw2 = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = Encoding.ASCII.GetBytes("hello, world    ");

                // Individual rectangles are compressed using the _same_ zlib stream. Let's send two
                // rectangles to make sure this is the case.
                encoder.Send(output, new VncPixelFormat(), default(VncRectangle), contents);
                raw1 = output.ToArray();

                output.SetLength(0);
                encoder.Send(output, new VncPixelFormat(), default(VncRectangle), contents);
                raw2 = output.ToArray();
            }

            byte[] expected1 = new byte[] { 0x00, 0x00, 0x00, 0x17, 0x78, 0x9c, 0xca, 0x48, 0xcd, 0xc9, 0xc9, 0xd7, 0x51, 0x28, 0xcf, 0x2f, 0xca, 0x49, 0x51, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff };
            byte[] expected2 = new byte[] { 0x00, 0x00, 0x00, 0x15, 0xca, 0x48, 0xcd, 0xc9, 0xc9, 0xd7, 0x51, 0x28, 0xcf, 0x2f, 0xca, 0x49, 0x51, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff };

            Assert.Equal(expected1, raw1);
            Assert.Equal(expected2, raw2);
        }
    }
}
