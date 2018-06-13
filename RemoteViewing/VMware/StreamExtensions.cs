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
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteViewing.VMware
{
    /// <summary>
    /// Provides extensions to the <see cref="Stream"/> class.
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Serializes a struct and writes it to a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of struct to serialize.
        /// </typeparam>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which to write the struct.
        /// </param>
        /// <param name="value">
        /// The value of the struct.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        public static async Task WriteStructAsync<T>(this Stream stream, T value, CancellationToken cancellationToken)
            where T : struct
        {
            byte[] buffer = null;

            try
            {
                var size = Unsafe.SizeOf<T>();
                buffer = ArrayPool<byte>.Shared.Rent(size);
                MemoryMarshal.Write(buffer, ref value);
                await stream.WriteAsync(buffer, 0, size, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        /// <summary>
        /// Grows the size of a <see cref="Stream"/> by writing zeros.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to grow.
        /// </param>
        /// <param name="count">
        /// The number of zero bytes to write.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        public static async Task GrowAsync(this Stream stream, int count, CancellationToken cancellationToken)
        {
            byte[] buffer = null;

            try
            {
                buffer = ArrayPool<byte>.Shared.Rent(512);

                while (count > buffer.Length)
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                    count -= buffer.Length;
                }

                await stream.WriteAsync(buffer, 0, count, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

#if !NETCOREAPP2_1
        public static Task WriteAsync(this Stream stream, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken)
        {
            var array = memory.ToArray();
            return stream.WriteAsync(array, 0, array.Length, cancellationToken);
        }
#endif
    }
}
