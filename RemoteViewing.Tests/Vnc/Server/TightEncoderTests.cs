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
using System;
using System.IO;
using System.Text;
using Xunit;

namespace RemoteViewing.Tests.Vnc.Server
{
    /// <summary>
    /// Tests the <see cref="TightEncoder"/> class.
    /// </summary>
    public class TightEncoderTests
    {
        /// <summary>
        /// Tests the <see cref="TightEncoder.WriteEncodedValue(byte[], int, int)"/> method.
        /// </summary>
        [Fact]
        public void WriteEncodedValueTest()
        {
            byte[] buffer = new byte[4];

            for (int i = 0; i < 64; i++)
            {
                Assert.Equal(1, TightEncoder.WriteEncodedValue(buffer, 0, i));
                Assert.Equal((byte)i, buffer[0]);
            }

            Assert.Equal(2, TightEncoder.WriteEncodedValue(buffer, 0, 10_000));
            Assert.Equal(0x90, buffer[0]);
            Assert.Equal(0x4E, buffer[1]);
        }

        /// <summary>
        /// Tests the <see cref="TightEncoder.Send(Stream, VncPixelFormat, byte[])"/> method in a scenario where the
        /// standard (non-tight) pixel format is used.
        /// </summary>
        [Fact]
        public void SendTestStandardPixelFormat()
        {
            TightEncoder encoder = new TightEncoder();

            byte[] raw = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = Encoding.ASCII.GetBytes("hello, world\n");
                encoder.Send(output, new VncPixelFormat(32, 24, 8, 0, 8, 0, 8, 0, false, true), contents);
                raw = output.ToArray();
            }

            // First byte:
            // - Basic compression
            // - Use stream 0
            // - Reset stream 0
            Assert.Equal(0b0000_0001, raw[0]);

            byte[] expectedZlibData = new byte[] { 0x78, 0x9c, 0xcb, 0x48, 0xcd, 0xc9, 0xc9, 0xd7, 0x51, 0x28, 0xcf, 0x2f, 0xca, 0x49, 0xe1, 0x02, 0x00, 0x21, 0xe7, 0x04, 0x93 };
            byte[] actualZlibData = new byte[raw.Length - 2];
            Array.Copy(raw, 2, actualZlibData, 0, raw.Length - 2);

            Assert.Equal((byte)expectedZlibData.Length, raw[1]);
            Assert.Equal(expectedZlibData, actualZlibData);
        }

        /// <summary>
        /// Tests the <see cref="TightEncoder.Send(Stream, VncPixelFormat, byte[])"/> method in a scenario where the
        /// tight pixel format is used.
        /// </summary>
        [Fact]
        public void SendTightPixelFormat()
        {
            TightEncoder encoder = new TightEncoder();

            byte[] raw = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = Encoding.ASCII.GetBytes("hello, world    ");
                encoder.Send(output, new VncPixelFormat(), contents);
                raw = output.ToArray();
            }

            // First byte:
            // - Basic compression
            // - Use stream 0
            // - Reset stream 0
            Assert.Equal(0b0000_0001, raw[0]);

            byte[] expectedZlibData = new byte[] { 0x78, 0x9c, 0xcb, 0x48, 0xcd, 0xc9, 0xd7, 0x51, 0xc8, 0x2f, 0xca, 0x51, 0x50, 0x50, 0x00, 0x00, 0x1a, 0xe6, 0x03, 0xa2 };

            byte[] actualZlibData = new byte[raw.Length - 2];
            Array.Copy(raw, 2, actualZlibData, 0, raw.Length - 2);

            Assert.Equal((byte)expectedZlibData.Length, raw[1]);
            Assert.Equal(expectedZlibData, actualZlibData);
        }

        /// <summary>
        /// Tests the <see cref="TightEncoder.Send(Stream, VncPixelFormat, byte[])"/> method in a scenario where the
        /// uncompressed data is less than 12 bytes long, causing the encoder not to use any compression.
        /// </summary>
        [Fact]
        public void SendSmallRectangleFormat()
        {
            TightEncoder encoder = new TightEncoder();

            byte[] raw = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = new byte[] { 0x01, 0x02, 0x03, 0x04 };
                encoder.Send(output, new VncPixelFormat(), contents);
                raw = output.ToArray();
            }

            // First byte:
            // - Basic compression
            // - Use stream 0
            // - Reset stream 0
            Assert.Equal(0b0000_0000, raw[0]);

            byte[] expectedZlibData = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            byte[] actualZlibData = new byte[raw.Length - 2];
            Array.Copy(raw, 2, actualZlibData, 0, raw.Length - 2);

            Assert.Equal((byte)expectedZlibData.Length, raw[1]);
            Assert.Equal(expectedZlibData, actualZlibData);
        }
    }
}
