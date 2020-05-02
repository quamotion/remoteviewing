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

using System;
using System.Runtime.InteropServices;

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// Provides methods for working with native struct layouts.
    /// </summary>
    internal static class NativeLayout
    {
        /// <summary>
        /// The type sizes and alignments of the types used by libvncserver on 64-bit Windows platforms.
        /// </summary>
        private static readonly int[][] Win64 =
            new int[][]
            {
                // type sizes
                new int[]
                {
                    0, // Skip,
                    8, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    16, // PixelFormat,
                    16, // ColourMap,
                    255, // Char_255,
                    1, // Bool,
                    8, // Socket,
                    520, // FdSet,
                    4, // SocketState,
                    16, // SockAddrIn,
                    8, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    8, // Timeval,
                    30000, // Byte_30000,
                    112, // z_stream_s,
                    4 * 112, // zsStruct_4,
                    4, // Bool_4,
                    16, // Int_4,
                    24, // rfbFileTransferData,
                    8, // pthread_cond_t,
                    4 * 64 * 64, // int zywrleBuf[rfbZRLETileWidth * rfbZRLETileHeight],
                    8, // Int_2,
                    16, // Byte_CHALLENGESIZE,
                    8, // PthreadT,
                    40, // Win32Mutex
                },

                // alignments
                new int[]
                {
                    1, // Skip,
                    8, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    1, // PixelFormat,
                    4, // ColourMap,
                    1, // Char_255,
                    1, // Bool,
                    8, // Socket,
                    1, // FdSet,
                    4, // SocketState,
                    4, // SockAddrIn,
                    8, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    1, // Timeval,
                    1, // Byte_30000,
                    1, // z_stream_s,
                    8, // zsStruct_4,
                    1, // Bool_4,
                    1, // Int_4,
                    1, // rfbFileTransferData,
                    1, // pthread_cond_t,
                    1, // Int_ZRLE,
                    1, // Int_2,
                    1, // Byte_CHALLENGESIZE,
                    1, // PthreadT,
                    8, // Win32Mutex
                },
            };

        /// <summary>
        /// The type sizes and alignments of the types used by libvncserver on 32-bit Windows platforms.
        /// </summary>
        private static readonly int[][] Win32 =
            new int[][]
            {
                // type sizes
                new int[]
                {
                    0, // Skip,
                    4, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    16, // PixelFormat,
                    12, // ColourMap,
                    255, // Char_255,
                    1, // Bool,
                    4, // Socket,
                    260, // FdSet,
                    4, // SocketState,
                    16, // SockAddrIn,
                    4, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    8, // Timeval,
                    30000, // Byte_30000,
                    112, // z_stream_s,
                    4 * 112, // zsStruct_4,
                    4, // Bool_4,
                    16, // Int_4,
                    24, // rfbFileTransferData,
                    4, // pthread_cond_t,
                    4 * 64 * 64, // int zywrleBuf[rfbZRLETileWidth * rfbZRLETileHeight],
                    8, // Int_2,
                    16, // Byte_CHALLENGESIZE,
                    4, // PthreadT,
                    40, // Win32Mutex
                },

                // alignments
                new int[]
                {
                    1, // Skip,
                    4, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    1, // PixelFormat,
                    4, // ColourMap,
                    1, // Char_255,
                    1, // Bool,
                    4, // Socket,
                    1, // FdSet,
                    4, // SocketState,
                    4, // SockAddrIn,
                    4, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    1, // Timeval,
                    1, // Byte_30000,
                    1, // z_stream_s,
                    8, // zsStruct_4,
                    1, // Bool_4,
                    1, // Int_4,
                    1, // rfbFileTransferData,
                    1, // pthread_cond_t,
                    1, // Int_ZRLE,
                    1, // Int_2,
                    1, // Byte_CHALLENGESIZE,
                    1, // PthreadT,
                    4, // Win32Mutex
                },
            };

        /// <summary>
        /// The type sizes and alignments of the types used by libvncserver on 64-bit Linux platforms.
        /// </summary>
        private static readonly int[][] Linux64 =
            new int[][]
            {
                // type sizes
                new int[]
                {
                    0, // Skip,
                    8, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    16, // PixelFormat,
                    16, // ColourMap,
                    255, // Char_255,
                    1, // Bool,
                    4, // Socket,
                    128, // FdSet,
                    4, // SocketState,
                    16, // SockAddrIn,
                    40, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    16, // Timeval,
                    30000, // Byte_30000,
                    112, // z_stream_s,
                    4 * 112, // zsStruct_4,
                    4, // Bool_4,
                    16, // Int_4,
                    24, // rfbFileTransferData,
                    48, // pthread_cond_t,
                    4 * 64 * 64, // int zywrleBuf[rfbZRLETileWidth * rfbZRLETileHeight],
                    8, // Int_2,
                    16, // Byte_CHALLENGESIZE,
                    8, // PthreadT,
                    0, // Win32Mutex
                },

                // alignments
                new int[]
                {
                    1, // Skip,
                    8, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    1, // PixelFormat,
                    4, // ColourMap,
                    1, // Char_255,
                    1, // Bool,
                    4, // Socket,
                    1, // FdSet,
                    4, // SocketState,
                    4, // SockAddrIn,
                    8, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    1, // Timeval,
                    1, // Byte_30000,
                    1, // z_stream_s,
                    8, // zsStruct_4,
                    1, // Bool_4,
                    1, // Int_4,
                    1, // rfbFileTransferData,
                    1, // pthread_cond_t,
                    1, // Int_ZRLE,
                    1, // Int_2,
                    1, // Byte_CHALLENGESIZE,
                    1, // PthreadT,
                    1, // Win32Mutex
                },
            };

        /// <summary>
        /// The type sizes and alignments of the types used by libvncserver on 64-bit OSX platforms.
        /// </summary>
        private static readonly int[][] OSX64 =
            new int[][]
            {
                // type sizes
                new int[]
                {
                    0, // Skip,
                    8, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    16, // PixelFormat,
                    16, // ColourMap,
                    255, // Char_255,
                    1, // Bool,
                    4, // Socket,
                    128, // FdSet,
                    4, // SocketState,
                    16, // SockAddrIn,
                    64, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    16, // Timeval,
                    30000, // Byte_30000,
                    112, // z_stream_s,
                    4 * 112, // zsStruct_4,
                    4, // Bool_4,
                    16, // Int_4,
                    24, // rfbFileTransferData,
                    48, // pthread_cond_t,
                    4 * 64 * 64, // int zywrleBuf[rfbZRLETileWidth * rfbZRLETileHeight],
                    8, // Int_2,
                    16, // Byte_CHALLENGESIZE,
                    8, // PthreadT,
                    0, // Win32Mutex
                },

                // alignments
                new int[]
                {
                    1, // Skip,
                    8, // IntPtr,
                    4, // Int,
                    4, // Pixel,
                    1, // PixelFormat,
                    4, // ColourMap,
                    1, // Char_255,
                    1, // Bool,
                    4, // Socket,
                    1, // FdSet,
                    4, // SocketState,
                    4, // SockAddrIn,
                    8, // Mutex,
                    4, // InAddrT,
                    4, // Float,

                    1, // Timeval,
                    1, // Byte_30000,
                    1, // z_stream_s,
                    8, // zsStruct_4,
                    1, // Bool_4,
                    1, // Int_4,
                    1, // rfbFileTransferData,
                    1, // pthread_cond_t,
                    1, // Int_ZRLE,
                    1, // Int_2,
                    1, // Byte_CHALLENGESIZE,
                    1, // PthreadT,
                    1, // Win32Mutex
                },
            };

        /// <summary>
        /// Gets the current <see cref="OSPlatform"/>.
        /// </summary>
        /// <returns>
        /// The current <see cref="OSPlatform"/>.
        /// </returns>
        public static OSPlatform GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the offsets of a list of fields.
        /// </summary>
        /// <param name="fieldTypes">
        /// An array which contains the type of each field.
        /// </param>
        /// <param name="platform">
        /// The platform for which to determine the offset.
        /// </param>
        /// <param name="is64Bit">
        /// A value indicating whether the platform is 32-bit or 64-bit.
        /// </param>
        /// <returns>
        /// An array which contains the offsets of the fields.
        /// </returns>
        public static int[] GetFieldOffsets(RfbType[] fieldTypes, OSPlatform platform, bool is64Bit)
        {
            int[] value = new int[fieldTypes.Length];
            int[] typeAlignment;
            int[] typeSizes;

            if (platform == OSPlatform.Windows)
            {
                if (is64Bit)
                {
                    typeSizes = Win64[0];
                    typeAlignment = Win64[1];
                }
                else
                {
                    typeSizes = Win32[0];
                    typeAlignment = Win32[1];
                }
            }
            else if (platform == OSPlatform.Linux)
            {
                if (!is64Bit)
                {
                    throw new ArgumentOutOfRangeException(nameof(platform));
                }

                typeSizes = Linux64[0];
                typeAlignment = Linux64[1];
            }
            else if (platform == OSPlatform.OSX)
            {
                if (!is64Bit)
                {
                    throw new ArgumentOutOfRangeException(nameof(platform));
                }

                typeSizes = OSX64[0];
                typeAlignment = OSX64[1];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(platform));
            }

            int offset = 0;

            for (int i = 0; i < value.Length; i++)
            {
                // align
                int alignment = typeAlignment[(int)fieldTypes[i]];
                int ret = offset % alignment;

                if (ret != 0)
                {
                    offset += alignment - ret;
                }

                value[i] = offset;
                offset += typeSizes[(int)fieldTypes[i]];
            }

            return value;
        }
    }
}
