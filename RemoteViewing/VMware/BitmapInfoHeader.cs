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
    /// The <see cref="BitmapInfoHeader"/> structure contains information about the dimensions and color forma
    /// t of a device-independent bitmap (DIB).
    /// </summary>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd318229(v=vs.85).aspx"/>
    [StructLayout(LayoutKind.Sequential)]
    internal struct BitmapInfoHeader
    {
        /// <summary>
        /// Specifies the number of bytes required by the structure. This value does not include the size of
        /// the color table or the size of the color masks, if they are appended to the end of structure
        /// </summary>
        public uint Size;

        /// <summary>
        /// Specifies the width of the bitmap, in pixels. For information about calculating the stride of
        /// the bitmap, see Remarks.
        /// </summary>
        public int Width;

        /// <summary>
        /// Specifies the height of the bitmap, in pixels.
        /// </summary>
        public int Height;

        /// <summary>
        /// Specifies the number of planes for the target device. This value must be set to 1.
        /// </summary>
        public ushort Planes;

        /// <summary>
        /// Specifies the number of bits per pixel (bpp). For uncompressed formats, this value is the
        /// average number of bits per pixel. For compressed formats, this value is the implied bit
        /// depth of the uncompressed image, after the image has been decoded.
        /// </summary>
        public ushort BitCount;

        /// <summary>
        /// For compressed video and YUV formats, this member is a FOURCC code, specified as a DWORD in
        /// little-endian order.
        /// </summary>
        public FourCC Compression;

        /// <summary>
        /// Specifies the size, in bytes, of the image. This can be set to 0 for uncompressed RGB bitmaps.
        /// </summary>
        public uint SizeImage;

        /// <summary>
        /// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap.
        /// </summary>
        public int XPelsPerMeter;

        /// <summary>
        /// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap.
        /// </summary>
        public int YPelsPerMeter;

        /// <summary>
        /// Specifies the number of color indices in the color table that are actually used by the bitmap.
        /// </summary>
        public uint ClrUsed;

        /// <summary>
        /// Specifies the number of color indices that are considered important for displaying the bitmap.
        /// If this value is zero, all colors are important.
        /// </summary>
        public uint ClrImportant;
    }
}
