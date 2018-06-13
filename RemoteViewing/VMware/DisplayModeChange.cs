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

using System;
using System.Buffers.Binary;

namespace RemoteViewing.VMware
{
    /// <summary>
    /// A request to update the display mode.
    /// </summary>
    internal class DisplayModeChange
    {
        /// <summary>
        /// The underlying buffer.
        /// </summary>
        private readonly byte[] buffer = new byte[16];

        /// <summary>
        /// Gets or sets the number of bits used to store a pixel.
        /// </summary>
        public byte BitsPerSample
        {
            get { return this.buffer[0]; }
            set { this.buffer[0] = value; }
        }

        /// <summary>
        /// Gets or sets the bit depth of a pixel.
        /// </summary>
        public byte Depth
        {
            get { return this.buffer[1]; }
            set { this.buffer[1] = value; }
        }

        public byte Color
        {
            get { return this.buffer[2]; }
            set { this.buffer[2] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the colors are true colors (or indexes into a palette).
        /// </summary>
        public bool TrueColor
        {
            get { return this.buffer[3] == 1; }
            set { this.buffer[3] = value ? (byte)1 : (byte)0; }
        }

        /// <summary>
        /// Gets or sets the maximum value for the color red.
        /// </summary>
        public ushort MaxRed
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(4, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(4, 2), value); }
        }

        /// <summary>
        /// Gets or sets the maximum value for the color green.
        /// </summary>
        public ushort MaxGreen
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(6, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(6, 2), value); }
        }

        /// <summary>
        /// Gets or sets the maximum value for the color blue.
        /// </summary>
        public ushort MaxBlue
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(8, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(8, 2), value); }
        }

        /// <summary>
        /// Gets or sets the number of bits left the color red is shifted.
        /// </summary>
        public byte RedShift
        {
            get { return this.buffer[10]; }
            set { this.buffer[10] = value; }
        }

        /// <summary>
        /// Gets or sets the number of bits left the color green is shifted.
        /// </summary>
        public byte GreenShift
        {
            get { return this.buffer[11]; }
            set { this.buffer[11] = value; }
        }

        /// <summary>
        /// Gets or sets the number of bits left the color blue is shifted.
        /// </summary>
        public byte BlueShift
        {
            get { return this.buffer[12]; }
            set { this.buffer[12] = value; }
        }

        /// <summary>
        /// Gets the underlying buffer.
        /// </summary>
        public ReadOnlyMemory<byte> Buffer
        {
            get { return this.buffer.AsMemory(); }
        }
    }
}
