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

namespace RemoteViewing.Vnc.Messages
{
    /// <summary>
    /// A request to update an rectangle in a framebuffer.
    /// </summary>
    internal class FramebufferUpdateRectangle
    {
        /// <summary>
        /// The underlying buffer.
        /// </summary>
        private readonly byte[] buffer = new byte[12];

        /// <summary>
        /// Gets or sets the x-coordinate of the upper left corner of the rectangle.
        /// </summary>
        public ushort X
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(0, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(0, 2), value); }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the upper left corner of the rectangle.
        /// </summary>
        public ushort Y
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(2, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(2, 2), value); }
        }

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        public ushort Width
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(4, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(4, 2), value); }
        }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        public ushort Height
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(6, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(6, 2), value); }
        }

        /// <summary>
        /// Gets or sets the encoding used to encode the pixels in the rectangle.
        /// </summary>
        public VncEncoding EncodingType
        {
            get { return (VncEncoding)BinaryPrimitives.ReadInt32BigEndian(this.buffer.AsSpan(8, 4)); }
            set { BinaryPrimitives.WriteInt32BigEndian(this.buffer.AsSpan(8, 4), (int)value); }
        }

        /// <summary>
        /// Gets the underlying memory.
        /// </summary>
        public ReadOnlyMemory<byte> Buffer
        {
            get { return this.buffer.AsMemory(); }
        }
    }
}
