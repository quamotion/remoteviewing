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

using System.Runtime.InteropServices;

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// A structure used to specify the pixel format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RfbPixelFormat
    {
        /// <summary>
        /// The number of bits per pixel. Valid values are 8, 16, or 32.
        /// </summary>
        public byte BitsPerPixel;

        /// <summary>
        /// Gets the depth of the pixel. Value values are 8 to 32.
        /// </summary>
        public byte Depth;

        /// <summary>
        /// A value indicating whether multi-byte pixels are interpreted as big endian.
        /// </summary>
        public byte BigEndian;

        /// <summary>
        /// A value indicating whether the pixels are true colors (e.g. RGB values).
        /// </summary>
        /// <remarks>
        /// If the value is <see langword="false"/>, a colour map is used to convert
        /// pixels to RGB.
        /// </remarks>
        public byte TrueColour;

        /* the following fields are only meaningful if trueColour is true */

        /// <summary>
        /// The maximum red value (= <c>2^n - 1</c> where <c>n</c> is the number
        /// of bits used for red). This value is always in big endian order.
        /// </summary>
        public ushort RedMax;

        /// <summary>
        /// The maximum green value (= <c>2^n - 1</c> where <c>n</c> is the number
        /// of bits used for green). This value is always in big endian order.
        /// </summary>
        public ushort GreenMax;

        /// <summary>
        /// The maximum blue value (= <c>2^n - 1</c> where <c>n</c> is the number
        /// of bits used for blue). This value is always in big endian order.
        /// </summary>
        public ushort BlueMax;

        /// <summary>
        /// The number of shifts needed to get the red value in a pixel to the least
        /// significant bit.
        /// </summary>
        public byte RedShift;

        /// <summary>
        /// The number of shifts needed to get the green value in a pixel to the least
        /// significant bit.
        /// </summary>
        public byte GreenShift;

        /// <summary>
        /// The number of shifts needed to get the blue value in a pixel to the least
        /// significant bit.
        /// </summary>
        public byte BlueShift;

        /// <summary>
        /// Three bytes of padding to align the struct size.
        /// </summary>
        private byte pad1;
        private ushort pad2;
    }
}
