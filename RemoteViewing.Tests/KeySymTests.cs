#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2017 Quamotion bvba
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

namespace RemoteViewing.Tests
{
    public class KeySymTests
    {
        [Fact]
        public void IsAsciiTest()
        {
            Assert.True(KeySymHelpers.IsAscii(KeySym.Space));
            Assert.True(KeySymHelpers.IsAscii(KeySym.A));
            Assert.True(KeySymHelpers.IsAscii(KeySym.a));
            Assert.True(KeySymHelpers.IsAscii(KeySym.AsciiTilde));

            Assert.False(KeySymHelpers.IsAscii(KeySym.AltLeft));
            Assert.False(KeySymHelpers.IsAscii(KeySym.F12));
            Assert.False(KeySymHelpers.IsAscii(KeySym.NumPad0));
        }

        [Fact]
        public void ToCharTest()
        {
            Assert.Equal(' ', KeySymHelpers.ToChar(KeySym.Space));
            Assert.Equal('A', KeySymHelpers.ToChar(KeySym.A));
            Assert.Equal('a', KeySymHelpers.ToChar(KeySym.a));
            Assert.Equal('~', KeySymHelpers.ToChar(KeySym.AsciiTilde));

            Assert.Equal('ç', KeySymHelpers.ToChar((KeySym)0x10000E7));

            Assert.Throws<ArgumentOutOfRangeException>(() => KeySymHelpers.ToChar(KeySym.F12));
        }

        [Fact]
        public void FromCharTest()
        {
            Assert.Equal(KeySym.Space, KeySymHelpers.FromChar(' '));
            Assert.Equal(KeySym.A, KeySymHelpers.FromChar('A'));
            Assert.Equal(KeySym.a, KeySymHelpers.FromChar('a'));
            Assert.Equal(KeySym.AsciiTilde, KeySymHelpers.FromChar('~'));
            Assert.Equal((KeySym)0x10000E7, KeySymHelpers.FromChar('ç'));
        }
    }
}
