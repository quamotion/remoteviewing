#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Provides utility methods.
    /// </summary>
    internal static class VncUtility
    {
        /// <summary>
        /// Allocates a byte array of a given size.
        /// </summary>
        /// <param name="bytes">
        /// The minimum required length of the byte array.
        /// </param>
        /// <param name="scratch">
        /// A current byte array which can be re-used, if has enough size.
        /// </param>
        /// <returns>
        /// A byte array with at least <paramref name="bytes"/> of space.
        /// </returns>
        public static byte[] AllocateScratch(int bytes, ref byte[] scratch)
        {
            if (scratch.Length < bytes)
            {
                scratch = new byte[bytes];
            }

            return scratch;
        }

        /// <summary>
        /// Decodes a <see cref="ushort"/> from a byte-array, in big-endian encoding.
        /// </summary>
        /// <param name="buffer">
        /// A <see cref="byte"/> array which contains the <see cref="ushort"/>.
        /// </param>
        /// <param name="offset">
        /// The index in <paramref name="buffer"/> at which the <see cref="ushort"/> starts.
        /// </param>
        /// <returns>
        /// The requested <see cref="ushort"/>.
        /// </returns>
        public static ushort DecodeUInt16BE(byte[] buffer, int offset)
        {
            return (ushort)(buffer[offset + 0] << 8 | buffer[offset + 1]);
        }

        /// <summary>
        /// Encodes a <see cref="ushort"/> as a <see cref="byte"/> array in big-endian
        /// encoding.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ushort"/> to encode.
        /// </param>
        /// <returns>
        /// A <see cref="byte"/> array which represents the <paramref name="value"/>.
        /// </returns>
        public static byte[] EncodeUInt16BE(ushort value)
        {
            var buffer = new byte[2];
            EncodeUInt16BE(buffer, 0, value);
            return buffer;
        }

        /// <summary>
        /// Encodes a <see cref="ushort"/> as a <see cref="byte"/> array in big-endian
        /// encoding.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="byte"/> array in which to store the <paramref name="value"/>.
        /// </param>
        /// <param name="offset">
        /// The index of the first byte in <paramref name="buffer"/> at which the
        /// <paramref name="value"/> should be stored.
        /// </param>
        /// <param name="value">
        /// The <see cref="ushort"/> to encode.
        /// </param>
        public static void EncodeUInt16BE(byte[] buffer, int offset, ushort value)
        {
            buffer[offset + 0] = (byte)(value >> 8);
            buffer[offset + 1] = (byte)value;
        }

        /// <summary>
        /// Decodes a <see cref="uint"/> from a byte-array, in big-endian encoding.
        /// </summary>
        /// <param name="buffer">
        /// A <see cref="byte"/> array which contains the <see cref="uint"/>.
        /// </param>
        /// <param name="offset">
        /// The index in <paramref name="buffer"/> at which the <see cref="uint"/> starts.
        /// </param>
        /// <returns>
        /// The requested <see cref="uint"/>.
        /// </returns>
        public static uint DecodeUInt32BE(byte[] buffer, int offset)
        {
            return (uint)(buffer[offset + 0] << 24 | buffer[offset + 1] << 16 | buffer[offset + 2] << 8 | buffer[offset + 3]);
        }

        /// <summary>
        /// Encodes a <see cref="uint"/> as a <see cref="byte"/> array in big-endian
        /// encoding.
        /// </summary>
        /// <param name="value">
        /// The <see cref="uint"/> to encode.
        /// </param>
        /// <returns>
        /// A <see cref="byte"/> array which represents the <paramref name="value"/>.
        /// </returns>
        public static byte[] EncodeUInt32BE(uint value)
        {
            var buffer = new byte[4];
            EncodeUInt32BE(buffer, 0, value);
            return buffer;
        }

        /// <summary>
        /// Encodes a <see cref="uint"/> as a <see cref="byte"/> array in big-endian
        /// encoding.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="byte"/> array in which to store the <paramref name="value"/>.
        /// </param>
        /// <param name="offset">
        /// The index of the first byte in <paramref name="buffer"/> at which the
        /// <paramref name="value"/> should be stored.
        /// </param>
        /// <param name="value">
        /// The <see cref="uint"/> to encode.
        /// </param>
        public static void EncodeUInt32BE(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)value;
        }
    }
}
