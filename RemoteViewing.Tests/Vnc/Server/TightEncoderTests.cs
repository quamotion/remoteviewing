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

using Moq;
using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            TightEncoder encoder = new TightEncoder(Mock.Of<IVncServerSession>())
            {
                Compression = TightCompression.Basic,
            };

            byte[] raw = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = Encoding.ASCII.GetBytes("hello, world\n");
                encoder.Send(output, new VncPixelFormat(32, 24, 8, 0, 8, 0, 8, 0, false, true), new VncRectangle(), contents);
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
            TightEncoder encoder = new TightEncoder(Mock.Of<IVncServerSession>())
            {
                Compression = TightCompression.Basic,
            };

            byte[] raw = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = new byte[]
                {
                    // A 2x2 framebuffer in
                    // BGRX format
                    0x01, 0x02, 0x03, 0x04,
                    0x01, 0x02, 0x03, 0x04,
                    0x01, 0x02, 0x03, 0x04,
                    0x01, 0x02, 0x03, 0x04,
                };

                encoder.Send(output, VncPixelFormat.RGB32, new VncRectangle(0, 0, 2, 2), contents);
                raw = output.ToArray();
            }

            // First byte:
            // - Basic compression
            // - Use stream 0
            // - Reset stream 0
            Assert.Equal(0b0000_0001, raw[0]);

            byte[] expectedZlibData = new byte[] { 0x78, 0x9c, 0x62, 0x66, 0x62, 0x64, 0x86, 0x20, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x62, 0x02, 0x00, 0x00, 0x00, 0xff, 0xff, 0x62, 0x04, 0x00, 0x00, 0x00, 0xff, 0xff };

            byte[] actualZlibData = new byte[raw.Length - 2];
            Array.Copy(raw, 2, actualZlibData, 0, raw.Length - 2);

            Assert.Equal((byte)expectedZlibData.Length, raw[1]);
            Assert.Equal(expectedZlibData, actualZlibData);

            // Decompress the data and make sure the pixel format is correct (RGB, the 'VncPixelFormat.RGB32' actually represents BGRX)
            using (var compressedStream = new MemoryStream(actualZlibData))
            using (var stream = new ZlibStream(compressedStream, CompressionMode.Decompress))
            {
                byte[] decompressed = new byte[0x09];
                Assert.Equal(9, stream.Read(decompressed, 0, 9));

                // Make sure all data was read
                Assert.Equal(0, stream.Read(decompressed, 0, 0));

                var expectedDecompressedData = new byte[]
                {
                    0x03, 0x02, 0x01,
                    0x03, 0x02, 0x01,
                    0x03, 0x02, 0x01,
                };

                Assert.Equal(expectedDecompressedData, decompressed);
            }
        }

        /// <summary>
        /// Tests the <see cref="TightEncoder.Send(Stream, VncPixelFormat, byte[])"/> method in a scenario where the
        /// uncompressed data is less than 12 bytes long, causing the encoder not to use any compression.
        /// </summary>
        [Fact]
        public void SendSmallRectangleFormat()
        {
            TightEncoder encoder = new TightEncoder(Mock.Of<IVncServerSession>())
            {
                Compression = TightCompression.Basic,
            };

            byte[] raw = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = new byte[] { 0x01, 0x02, 0x03, 0x04 };
                encoder.Send(output, new VncPixelFormat(), new VncRectangle(), contents);
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

        /// <summary>
        /// Tests the <see cref="TightEncoder.GetCompressionLevel(IVncServerSession)"/> method.
        /// </summary>
        /// <param name="expectedCompressionLevel">
        /// The expected compression level.
        /// </param>
        /// <param name="encoding">
        /// The pseudo-encoding which influences the compression level.
        /// </param>
        [Theory]
        [InlineData(CompressionLevel.Default, VncEncoding.RRE)]
        [InlineData(CompressionLevel.Level0, VncEncoding.TightCompressionLevel0)]
        [InlineData(CompressionLevel.Level1, VncEncoding.TightCompressionLevel1)]
        [InlineData(CompressionLevel.Level2, VncEncoding.TightCompressionLevel2)]
        [InlineData(CompressionLevel.Level3, VncEncoding.TightCompressionLevel3)]
        [InlineData(CompressionLevel.Level4, VncEncoding.TightCompressionLevel4)]
        [InlineData(CompressionLevel.Level5, VncEncoding.TightCompressionLevel5)]
        [InlineData(CompressionLevel.Level6, VncEncoding.TightCompressionLevel6)]
        [InlineData(CompressionLevel.Level7, VncEncoding.TightCompressionLevel7)]
        [InlineData(CompressionLevel.Level8, VncEncoding.TightCompressionLevel8)]
        [InlineData(CompressionLevel.Level9, VncEncoding.TightCompressionLevel9)]
        public void GetTightCompressionLevelTest(CompressionLevel expectedCompressionLevel, VncEncoding encoding)
        {
            Collection<VncEncoding> encodings = new Collection<VncEncoding>();
            encodings.Add(VncEncoding.Raw);
            encodings.Add(VncEncoding.Tight);
            encodings.Add(encoding);

            var mock = new Mock<IVncServerSession>();
            mock
                .Setup(m => m.ClientEncodings)
                .Returns(encodings);

            var compressionLevel = TightEncoder.GetCompressionLevel(mock.Object);
            Assert.Equal(expectedCompressionLevel, compressionLevel);
        }

        /// <summary>
        /// Tests the <see cref="TightEncoder.GetQualityLevel(IVncServerSession)"/> method.
        /// </summary>
        /// <param name="expectedQualityLevel">
        /// The expected quality level.
        /// </param>
        /// <param name="encoding">
        /// The pseudo-encoding which influences the quality level.
        /// </param>
        [Theory]
        [InlineData(0, VncEncoding.RRE)]
        [InlineData(5, VncEncoding.TightQualityLevel0)]
        [InlineData(10, VncEncoding.TightQualityLevel1)]
        [InlineData(15, VncEncoding.TightQualityLevel2)]
        [InlineData(25, VncEncoding.TightQualityLevel3)]
        [InlineData(37, VncEncoding.TightQualityLevel4)]
        [InlineData(50, VncEncoding.TightQualityLevel5)]
        [InlineData(60, VncEncoding.TightQualityLevel6)]
        [InlineData(70, VncEncoding.TightQualityLevel7)]
        [InlineData(75, VncEncoding.TightQualityLevel8)]
        [InlineData(80, VncEncoding.TightQualityLevel9)]
        public void GetTightQualityLevelTest(int expectedQualityLevel, VncEncoding encoding)
        {
            Collection<VncEncoding> encodings = new Collection<VncEncoding>();
            encodings.Add(VncEncoding.Raw);
            encodings.Add(VncEncoding.Tight);
            encodings.Add(encoding);

            var mock = new Mock<IVncServerSession>();
            mock
                .Setup(m => m.ClientEncodings)
                .Returns(encodings);

            var compressionLevel = TightEncoder.GetQualityLevel(mock.Object);
            Assert.Equal(expectedQualityLevel, compressionLevel);
        }

        /// <summary>
        /// Tests the <see cref="TightEncoder.Send(Stream, VncPixelFormat, VncRectangle, byte[])"/> method in a scenario
        /// where the data will be compressed using a JPEG encoder.
        /// </summary>
        [Fact]
        public void SendJpegFormat()
        {
            var serverSession = new Mock<IVncServerSession>();
            serverSession
                .Setup(s => s.ClientEncodings)
                .Returns(new List<VncEncoding>()
                {
                    VncEncoding.TightQualityLevel4,
                });

            TightEncoder encoder = new TightEncoder(serverSession.Object)
            {
                Compression = TightCompression.Jpeg
            };

            byte[] raw = null;

            using (MemoryStream output = new MemoryStream())
            {
                var contents = new byte[512];
                encoder.Send(output, VncPixelFormat.RGB32, new VncRectangle() { Width = 1, Height = 1 }, contents);
                raw = output.ToArray();
            }

            Assert.Equal((byte)TightCompressionControl.JpegCompression, raw[0]);

            // Make sure the compressed image is a valid JPEG image,
            // by checking for the JPEG magic.
            Assert.Equal(0xFF, raw[3]);
            Assert.Equal(0xD8, raw[4]);
        }
    }
}
