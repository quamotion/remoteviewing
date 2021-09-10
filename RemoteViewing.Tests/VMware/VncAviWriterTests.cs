#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2018 Quamotion bvba <http://quamotion.mobi>
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
using RemoteViewing.VMware;
using RemoteViewing.Vnc;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RemoteViewing.Tests.VMware
{
    /// <summary>
    /// Tests the <see cref="VncAviWriter"/> class.
    /// </summary>
    public class VncAviWriterTests
    {
        /// <summary>
        /// Test writing the header of an AVI file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task WriteAviHeaderTest()
        {
            byte[] expected = new byte[0x480c];

            using (Stream stream = File.OpenRead("VMware/VS2k5DebugDemo-01.avi"))
            {
                stream.Read(expected, 0, expected.Length);
            }

            bool isFirst = true;

            using (MemoryStream output = new MemoryStream())
            {
                var framebufferSource = new Mock<IVncFramebufferSource>();
                framebufferSource
                    .Setup(f => f.Capture())
                    .Returns(
                    () =>
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                            return new VncFramebuffer("My Framebuffer", width: 0x4f4, height: 0x3c1, pixelFormat: VncPixelFormat.RGB32);
                        }
                        else
                        {
                            return null;
                        }
                    });

                VncAviWriter aviWriter = new VncAviWriter(framebufferSource.Object);
                aviWriter.ExpectedSize = 0x0031dfe4;
                aviWriter.ExpectedTotalFrames = 0x13e;
                Assert.Equal(200_000, aviWriter.MicrosecondsPerFrame);
                await aviWriter.WriteAsync(output, CancellationToken.None).ConfigureAwait(false);

                byte[] actual = output.ToArray();
                File.WriteAllBytes("actual.bin", actual);

                Assert.Equal(expected, actual);
            }
        }
    }
}
