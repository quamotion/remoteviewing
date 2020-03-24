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

using RemoteViewing.Vnc;
using System;
using Xunit;

namespace RemoteViewing.Tests.Vnc
{
    /// <summary>
    /// Tests the <see cref="VncPixelFormat"/> class.
    /// </summary>
    public class VncPixelFormatTests
    {
        /// <summary>
        /// Tests the <see cref="VncPixelFormat"/> constructors.
        /// </summary>
        [Fact]
        public void ConstructorTest()
        {
            // Default pixel format should be RGB32.
            var pixelFormat = new VncPixelFormat();
            Assert.Equal(24, pixelFormat.BitDepth);
            Assert.Equal(32, pixelFormat.BitsPerPixel);
            Assert.Equal(4, pixelFormat.BytesPerPixel);

            Assert.True(pixelFormat.IsLittleEndian);
            Assert.False(pixelFormat.IsPalettized);

            Assert.Equal(8, pixelFormat.BlueBits);
            Assert.Equal(255, pixelFormat.BlueMax);
            Assert.Equal(0, pixelFormat.BlueShift);

            Assert.Equal(8, pixelFormat.GreenBits);
            Assert.Equal(255, pixelFormat.GreenMax);
            Assert.Equal(8, pixelFormat.GreenShift);

            Assert.Equal(8, pixelFormat.RedBits);
            Assert.Equal(255, pixelFormat.RedMax);
            Assert.Equal(16, pixelFormat.RedShift);
        }

        /// <summary>
        /// Tests the <see cref="VncPixelFormat"/> constructor's argument validation.
        /// </summary>
        [Fact]
        public void ConstuctorInvalidValuesTest()
        {
            // Only 8, 16 or 32 bits per pixel
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(4, 1, 1, 0, 1, 0, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(24, 1, 1, 0, 1, 0, 1, 0));

            // Only bit depth of 8 or 24
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 2, 1, 0, 1, 0, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(16, 16, 1, 0, 1, 0, 1, 0));

            // Red: negative bits or shift, or bits or shift > bit depth
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, -1, 0, 8, 0, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, -1, 8, 0, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 25, 0, 8, 0, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 25, 8, 0, 8, 0));

            // Blue: negative bits or shift, or bits or shift > bit depth
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, -1, 0, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, 8, -1, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, 25, 0, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, 8, 25, 8, 0));

            // Green: negative bits or shift, or bits or shift > bit depth
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, 8, 0, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, 8, 0, 8, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, 8, 0, 25, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncPixelFormat(8, 24, 8, 0, 8, 0, 8, 25));
        }

        /// <summary>
        /// Tests the <see cref="VncPixelFormat.Encode(byte[], int)"/> method.
        /// </summary>
        [Fact]
        public void EncodeTest()
        {
            var pixelFormat = new VncPixelFormat();
            var buffer = new byte[VncPixelFormat.Size];
            pixelFormat.Encode(buffer, 0);

            var expected = new byte[] { 32, 24, 0, 1, 0, 255, 0, 255, 0, 255, 16, 8, 0, 0, 0, 0 };
            Assert.Equal(expected, buffer);
        }

        /// <summary>
        /// Tests the <see cref="VncPixelFormat.Decode(byte[], int)"/> method.
        /// </summary>
        [Fact]
        public void DecodeTest()
        {
            var buffer = new byte[] { 32, 24, 0, 1, 0, 255, 0, 255, 0, 255, 16, 8, 0, 0, 0, 0 };
            var pixelFormat = VncPixelFormat.Decode(buffer, 0);
            Assert.Equal(VncPixelFormat.RGB32, pixelFormat);
        }
    }
}
