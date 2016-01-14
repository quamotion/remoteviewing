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

using System;

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Describes the low-level arrangement of a framebuffer pixel.
    /// </summary>
    public sealed class VncPixelFormat
    {
        int _bitsPerPixel, _bytesPerPixel, _bitDepth;
        int _redBits, _redShift, _greenBits, _greenShift, _blueBits, _blueShift;
        bool _isLittleEndian, _isPalettized;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncPixelFormat"/> class,
        /// with 8 bits each of red, green, and blue channels.
        /// </summary>
        public VncPixelFormat()
            : this(32, 24, 8, 16, 8, 8, 8, 0)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VncPixelFormat"/> class.
        /// </summary>
        /// <param name="bitsPerPixel">The number of bits used to store a pixel. Currently, this must be 8, 16, or 32.</param>
        /// <param name="bitDepth">The bit depth of the pixel. Currently, this must be 24.</param>
        /// <param name="redBits">The number of bits used to represent red.</param>
        /// <param name="redShift">The number of bits left the red value is shifted.</param>
        /// <param name="greenBits">The number of bits used to represent green.</param>
        /// <param name="greenShift">The number of bits left the green value is shifted.</param>
        /// <param name="blueBits">The number of bits used to represent blue.</param>
        /// <param name="blueShift">The number of bits left the blue value is shifted.</param>
        /// <param name="isLittleEndian"><c>true</c> if the pixel is little-endian, or <c>false</c> if it is big-endian.</param>
        /// <param name="isPalettized"><c>true</c> if the framebuffer stores palette indices, or <c>false</c> if it stores colors.</param>
        public VncPixelFormat(int bitsPerPixel, int bitDepth,
                              int redBits, int redShift, int greenBits, int greenShift, int blueBits, int blueShift,
                              bool isLittleEndian = true, bool isPalettized = false)
        {
            Throw.If.False(bitsPerPixel == 8 || bitsPerPixel == 16 || bitsPerPixel == 32, "bitsPerPixel");
            Throw.If.False(bitDepth == 24, "bitDepth");
            Throw.If.False(redBits >= 0 && redShift >= 0 && redBits <= bitDepth && redShift <= bitDepth, "redBits");
            Throw.If.False(greenBits >= 0 && greenShift >= 0 && greenBits <= bitDepth && greenShift <= bitDepth, "greenBits");
            Throw.If.False(blueBits >= 0 && blueShift >= 0 && blueBits <= bitDepth && blueShift <= bitDepth, "blueBits");

            _bitsPerPixel = bitsPerPixel; _bytesPerPixel = bitsPerPixel / 8; _bitDepth = bitDepth;
            _redBits = redBits; _redShift = redShift;
            _greenBits = greenBits; _greenShift = greenShift;
            _blueBits = blueBits; _blueShift = blueShift;
            _isLittleEndian = isLittleEndian; _isPalettized = isPalettized;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var format = obj as VncPixelFormat;

            if (format != null)
            {
                if (BitsPerPixel == format.BitsPerPixel && BitDepth == format.BitDepth &&
                    RedBits == format.RedBits && RedShift == format.RedShift &&
                    GreenBits == format.GreenBits && GreenShift == format.GreenShift &&
                    BlueBits == format.BlueBits && BlueShift == format.BlueShift &&
                    IsLittleEndian == format.IsLittleEndian && IsPalettized == format.IsPalettized)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _bitsPerPixel ^ _redBits;
        }

        /// <summary>
        /// Copies pixels between two byte arrays. A format conversion is performed if necessary.
        /// 
        /// Be sure to lock <see cref="VncFramebuffer.SyncRoot"/> first to avoid tearing,
        /// if the connection is active.
        /// </summary>
        /// <param name="source">A pointer to the upper-left corner of the source.</param>
        /// <param name="sourceStride">The offset in the source between one Y coordinate and the next.</param>
        /// <param name="sourceFormat">The source pixel format.</param>
        /// <param name="sourceRectangle">The rectangle in the source to decode.</param>
        /// <param name="target">A pointer to the upper-left corner of the target.</param>
        /// <param name="targetStride">The offset in the target between one Y coordinate and the next.</param>
        /// <param name="targetFormat">The target pixel format.</param>
        /// <param name="targetX">The X coordinate in the target that the leftmost pixel should be placed into.</param>
        /// <param name="targetY">The Y coordinate in the target that the topmost pixel should be placed into.</param>
        public static unsafe void Copy(byte[] source, int sourceStride, VncPixelFormat sourceFormat, VncRectangle sourceRectangle,
                                       byte[] target, int targetStride, VncPixelFormat targetFormat, int targetX = 0, int targetY = 0)
        {
            Throw.If.Null(source, "source").Null(target, "target");

            fixed (byte* sourcePtr = source)
            fixed (byte* targetPtr = target)
            {
                Copy((IntPtr)sourcePtr, sourceStride, sourceFormat, sourceRectangle,
                     (IntPtr)targetPtr, targetStride, targetFormat, targetX, targetY);
            }
        }

        // TODO: Support more (any? :-) pixel formats.
        /// <summary>
        /// Copies pixels. A format conversion is performed if necessary.
        /// 
        /// Be sure to lock <see cref="VncFramebuffer.SyncRoot"/> first to avoid tearing,
        /// if the connection is active.
        /// </summary>
        /// <param name="source">A pointer to the upper-left corner of the source.</param>
        /// <param name="sourceStride">The offset in the source between one Y coordinate and the next.</param>
        /// <param name="sourceFormat">The source pixel format.</param>
        /// <param name="sourceRectangle">The rectangle in the source to decode.</param>
        /// <param name="target">A pointer to the upper-left corner of the target.</param>
        /// <param name="targetStride">The offset in the target between one Y coordinate and the next.</param>
        /// <param name="targetFormat">The target pixel format.</param>
        /// <param name="targetX">The X coordinate in the target that the leftmost pixel should be placed into.</param>
        /// <param name="targetY">The Y coordinate in the target that the topmost pixel should be placed into.</param>
        public static unsafe void Copy(IntPtr source, int sourceStride, VncPixelFormat sourceFormat, VncRectangle sourceRectangle,
                                       IntPtr target, int targetStride, VncPixelFormat targetFormat, int targetX = 0, int targetY = 0)
        {
            Throw.If.True(source == IntPtr.Zero, "source").True(target == IntPtr.Zero, "target");
            Throw.If.Null(sourceFormat, "sourceFormat").Null(targetFormat, "targetFormat");

            if (sourceRectangle.IsEmpty) { return; }

            int x = sourceRectangle.X, w = sourceRectangle.Width;
            int y = sourceRectangle.Y, h = sourceRectangle.Height;

            var sourceData = (byte*)(void*)source + y * sourceStride + x * sourceFormat.BytesPerPixel;
            var targetData = (byte*)(void*)target + targetY * targetStride + targetX * targetFormat.BytesPerPixel;

            if (sourceFormat.Equals(targetFormat))
            {
                for (int iy = 0; iy < h; iy++)
                {
                    if (sourceFormat.BytesPerPixel == 4)
                    {
                        uint* sourceDataX0 = (uint*)sourceData, targetDataX0 = (uint*)targetData;
                        for (int ix = 0; ix < w; ix++) { *targetDataX0++ = (*sourceDataX0++); }
                    }
                    else
                    {
                        int bytes = w * sourceFormat.BytesPerPixel;
                        byte* sourceDataX0 = (byte*)sourceData, targetDataX0 = (byte*)targetData;
                        for (int ib = 0; ib < bytes; ib++) { *targetDataX0++ = (*sourceDataX0++); }
                    }

                    sourceData += sourceStride; targetData += targetStride;
                }
            }
        }

        static int BitsFromMax(int max)
        {
            if (max == 0 || (max & (max + 1)) != 0) { throw new ArgumentException(); }
            return (int)Math.Round(Math.Log(max + 1) / Math.Log(2));
        }

        internal static VncPixelFormat Decode(byte[] buffer, int offset)
        {
            var bitsPerPixel = buffer[offset + 0];
            var depth = buffer[offset + 1];
            var isLittleEndian = buffer[offset + 2] == 0;
            var isPalettized = buffer[offset + 3] == 0;
            var redBits = BitsFromMax(VncUtility.DecodeUInt16BE(buffer, offset + 4));
            var greenBits = BitsFromMax(VncUtility.DecodeUInt16BE(buffer, offset + 6));
            var blueBits = BitsFromMax(VncUtility.DecodeUInt16BE(buffer, offset + 8));
            var redShift = buffer[offset + 10];
            var greenShift = buffer[offset + 11];
            var blueShift = buffer[offset + 12];

            return new VncPixelFormat(bitsPerPixel, depth,
                                      redBits, redShift, greenBits, greenShift, blueBits, blueShift,
                                      isLittleEndian, isPalettized);
        }

        internal void Encode(byte[] buffer, int offset)
        {
            buffer[offset + 0] = (byte)BitsPerPixel;
            buffer[offset + 1] = (byte)BitDepth;
            buffer[offset + 2] = (byte)(IsLittleEndian ? 0 : 1);
            buffer[offset + 3] = (byte)(IsPalettized ? 0 : 1);
            VncUtility.EncodeUInt16BE(buffer, offset + 4, (ushort)((1 << RedBits) - 1));
            VncUtility.EncodeUInt16BE(buffer, offset + 6, (ushort)((1 << GreenBits) - 1));
            VncUtility.EncodeUInt16BE(buffer, offset + 8, (ushort)((1 << BlueBits) - 1));
            buffer[offset + 10] = (byte)RedShift;
            buffer[offset + 11] = (byte)GreenShift;
            buffer[offset + 12] = (byte)BlueShift;
        }

        /// <summary>
        /// The number of bits used to store a pixel.
        /// </summary>
        public int BitsPerPixel { get { return _bitsPerPixel; } }

        /// <summary>
        /// The number of bytes used to store a pixel.
        /// </summary>
        public int BytesPerPixel { get { return _bytesPerPixel; } }

        /// <summary>
        /// The bit depth of the pixel.
        /// </summary>
        public int BitDepth { get { return _bitDepth; } }

        /// <summary>
        /// The number of bits used to represent red.
        /// </summary>
        public int RedBits { get { return _redBits; } }

        /// <summary>
        /// The number of bits left the red value is shifted.
        /// </summary>
        public int RedShift { get { return _redShift; } } 

        /// <summary>
        /// The number of bits used to represent green.
        /// </summary>
        public int GreenBits { get { return _greenBits; } }

        /// <summary>
        /// The number of bits left the green value is shifted.
        /// </summary>
        public int GreenShift { get { return _greenShift; } }

        /// <summary>
        /// The number of bits used to represent blue.
        /// </summary>
        public int BlueBits { get { return _blueBits; } }

        /// <summary>
        /// The number of bits left the blue value is shifted.
        /// </summary>
        public int BlueShift { get { return _blueShift; } }

        /// <summary>
        /// <c>true</c> if the pixel is little-endian, or <c>false</c> if it is big-endian.
        /// </summary>
        public bool IsLittleEndian { get { return _isLittleEndian; } }

        /// <summary>
        /// <c>true</c> if the framebuffer stores palette indices, or <c>false</c> if it stores colors.
        /// </summary>
        public bool IsPalettized { get { return _isPalettized; } }

        internal static int Size
        {
            get { return 16; }
        }
    }
}
