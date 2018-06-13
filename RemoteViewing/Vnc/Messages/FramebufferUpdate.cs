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
    /// A message from the server to the client which notifies the client of an update to the framebuffer.
    /// </summary>
    internal class FramebufferUpdate
    {
        /// <summary>
        /// The underlying buffer.
        /// </summary>
        private readonly byte[] buffer = new byte[4];

        /// <summary>
        /// Gets or sets the message type. This should always be 0.
        /// </summary>
        public byte MessageType
        {
            get { return this.buffer[0]; }
            set { this.buffer[0] = value; }
        }

        /// <summary>
        /// Gets or sets the number of rectangles which have been updated.
        /// </summary>
        public ushort NumberOfRectangles
        {
            get { return BinaryPrimitives.ReadUInt16BigEndian(this.buffer.AsSpan(2, 2)); }
            set { BinaryPrimitives.WriteUInt16BigEndian(this.buffer.AsSpan(2, 2), value); }
        }

        /// <summary>
        /// Gets or sets the underlying buffer.
        /// </summary>
        public ReadOnlyMemory<byte> Buffer
        {
            get { return this.buffer.AsMemory(); }
        }
    }
}
