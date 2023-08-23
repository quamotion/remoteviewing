﻿#region License
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
using RemoteViewing.Vnc.Server;
using System.IO;
using Xunit;

namespace RemoteViewing.Tests.Vnc.Server
{
    /// <summary>
    /// Tests the <see cref="RawEncoder"/> class.
    /// </summary>
    public class RawEncoderTests
    {
        /// <summary>
        /// Tests the <see cref="RawEncoder.Encoding"/> property.
        /// </summary>
        [Fact]
        public void EncodingTest()
        {
            var encoder = new RawEncoder();
            Assert.Equal(VncEncoding.Raw, encoder.Encoding);
        }

        /// <summary>
        /// Tests the <see cref="RawEncoder.Send"/> method.
        /// </summary>
        [Fact]
        public void SendTest()
        {
            RawEncoder encoder = new RawEncoder();
            byte[] content = { 0x01, 0x02, 0x03, 0x04 };

            using (MemoryStream stream = new MemoryStream())
            {
                // The encoder should write the content 'as is' to the stream.
                encoder.Send(stream, VncPixelFormat.RGB32, default(VncRectangle), content);

                Assert.Equal(content, stream.ToArray());
            }
        }
    }
}
