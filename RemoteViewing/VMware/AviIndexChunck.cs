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

using System.Runtime.InteropServices;

namespace RemoteViewing.VMware
{
    /// <summary>
    /// Contains an AVI 2.0 standard index.
    /// </summary>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/ff625869(v=vs.85).aspx"/>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AviIndexChunck
    {
        /// <summary>
        /// A <see cref="FourCC"/> code. The value is either <see cref="FourCC.Indx"/> or <c>nnix</c>,
        /// where nn is the stream number.
        /// </summary>
        public FourCC FourCC;

        /// <summary>
        /// The size of the structure, not including the initial 8 bytes.
        /// </summary>
        public int Size;

        /// <summary>
        /// The size of each index entry, in 4-byte units. The value must be 2.
        /// </summary>
        public short LongsPerEntry;

        /// <summary>
        /// The index subtype. The value must be <see cref="IndexSubType.None"/>.
        /// </summary>
        public IndexSubType IndexSubType;

        /// <summary>
        /// The index type. The value must be <see cref="IndexType.IndexOfChunks"/>.
        /// </summary>
        public IndexType IndexType;

        /// <summary>
        /// The number of valid entries in the adwIndex array.
        /// </summary>
        public int EntriesInUse;

        /// <summary>
        /// Specifies a <see cref="FourCC"/> that identifies a stream in the AVI file.
        /// The FOURCC must have the form 'xxyy' where xx is the stream number and yy is a two-character
        /// code that identifies the contents of the stream.
        /// </summary>
        public FourCC ChunkId;

        private int reserved1;
        private int reserved2;
        private int reserved3;
    }
}
