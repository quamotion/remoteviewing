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
    /// Tests the <see cref="VncFramebuffer"/> class.
    /// </summary>
    public class VncFramebufferTests
    {
        /// <summary>
        /// Tests the <see cref="VncFramebuffer.VncFramebuffer(string, int, int, VncPixelFormat)"/> constructor, when passed
        /// invalid arguments.
        /// </summary>
        [Fact]
        public void ConstructorInvalidValueTest()
        {
            Assert.Throws<ArgumentNullException>(() => new VncFramebuffer(null, 1, 1, VncPixelFormat.RGB32));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncFramebuffer("fb", -1, 1, VncPixelFormat.RGB32));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VncFramebuffer("fb", 1, -1, VncPixelFormat.RGB32));
            Assert.Throws<ArgumentNullException>(() => new VncFramebuffer("fb", 1, 1, null));
        }

        /// <summary>
        /// Tests the <see cref="VncFramebuffer.SetPixel(int, int, byte[])"/> method.
        /// </summary>
        [Fact]
        public void SetPixelInvalidValueTest()
        {
            var fb = new VncFramebuffer("test", 10, 10, VncPixelFormat.RGB32);

            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(0, 10, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(10, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(0, 11, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(11, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(0, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(-1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(10, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => fb.SetPixel(-1, 10, 0));
        }
    }
}
