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
        /// <summary>
        /// Initializes a new instance of the <see cref="VncPixelFormat"/> class,
        /// with 8 bits each of red, green, and blue channels.
        /// </summary>
        public VncPixelFormat()
            : this(16, 16, 5, 11, 6, 5, 5, 0)
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
        public VncPixelFormat(
            int bitsPerPixel,
            int bitDepth,
            int redBits,
            int redShift,
            int greenBits,
            int greenShift,
            int blueBits,
            int blueShift,
            bool isLittleEndian = true,
            bool isPalettized = false)
        {
            if (bitsPerPixel != 8 && bitsPerPixel != 16 && bitsPerPixel != 32)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsPerPixel));
            }

            if (bitDepth != 6 && bitDepth != 16)
            {
                throw new ArgumentOutOfRangeException(nameof(bitDepth));
            }

            if (!(redBits >= 0 && redShift >= 0 && redBits <= bitDepth && redShift <= bitDepth))
            {
                throw new ArgumentOutOfRangeException(nameof(redBits));
            }

            if (!(greenBits >= 0 && greenShift >= 0 && greenBits <= bitDepth && greenShift <= bitDepth))
            {
                throw new ArgumentOutOfRangeException(nameof(greenBits));
            }

            if (!(blueBits >= 0 && blueShift >= 0 && blueBits <= bitDepth && blueShift <= bitDepth))
            {
                throw new ArgumentOutOfRangeException(nameof(blueBits));
            }

            this.BitsPerPixel = bitsPerPixel;
            this.BytesPerPixel = bitsPerPixel / 8;
            this.BitDepth = bitDepth;
            this.RedBits = redBits;
            this.RedShift = redShift;
            this.GreenBits = greenBits;
            this.GreenShift = greenShift;
            this.BlueBits = blueBits;
            this.BlueShift = blueShift;
            this.IsLittleEndian = isLittleEndian;
            this.IsPalettized = isPalettized;
        }

        /// <summary>
        /// Gets a <see cref="VncPixelFormat"/> with 8 bits of red, green and blue channels.
        /// </summary>
        public static VncPixelFormat RGB32 { get; } = new VncPixelFormat();

        /// <summary>
        /// Gets the number of bits used to store a pixel.
        /// </summary>
        public int BitsPerPixel { get; private set; }

        /// <summary>
        /// Gets the number of bytes used to store a pixel.
        /// </summary>
        public int BytesPerPixel { get; private set; }

        /// <summary>
        /// Gets the bit depth of the pixel.
        /// </summary>
        public int BitDepth { get; private set; }

        /// <summary>
        /// Gets the number of bits used to represent red.
        /// </summary>
        public int RedBits { get; private set; }

        /// <summary>
        /// Gets the number of bits left the red value is shifted.
        /// </summary>
        public int RedShift { get; private set; }

        /// <summary>
        /// Gets the number of bits used to represent green.
        /// </summary>
        public int GreenBits { get; private set; }

        /// <summary>
        /// Gets the maximum value of the red color.
        /// </summary>
        public ushort RedMax
        {
            get
            {
                return (ushort)((1 << this.RedBits) - 1);
            }
        }

        /// <summary>
        /// Gets the maximum value of the blue color.
        /// </summary>
        public ushort BlueMax
        {
            get
            {
                return (ushort)((1 << this.BlueBits) - 1);
            }
        }

        /// <summary>
        /// Gets the maximum value of the green color.
        /// </summary>
        public ushort GreenMax
        {
            get
            {
                return (ushort)((1 << this.GreenBits) - 1);
            }
        }

        /// <summary>
        /// Gets the number of bits left the green value is shifted.
        /// </summary>
        public int GreenShift { get; private set; }

        /// <summary>
        /// Gets the number of bits used to represent blue.
        /// </summary>
        public int BlueBits { get; private set; }

        /// <summary>
        /// Gets the number of bits left the blue value is shifted.
        /// </summary>
        public int BlueShift { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the pixel is little-endian.
        /// </summary>
        /// <value>
        /// <c>true</c> if the pixel is little-endian, or <c>false</c> if it is big-endian.
        /// </value>
        public bool IsLittleEndian { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the framebuffer stores palette indices.
        /// </summary>
        /// <value>
        /// <c>true</c> if the framebuffer stores palette indices, or <c>false</c> if it stores colors.
        /// </value>
        public bool IsPalettized { get; private set; }

        /// <summary>
        /// Gets the size of a <see cref="VncPixelFormat"/> when serialized to a <see cref="byte"/> array.
        /// </summary>
        internal static int Size
        {
            get { return 16; }
        }

        /// <summary>
        /// Copies pixels between two byte arrays. A format conversion is performed if necessary.
        ///
        /// Be sure to lock <see cref="VncFramebuffer.SyncRoot"/> first to avoid tearing,
        /// if the connection is active.
        /// </summary>
        /// <param name="source">A pointer to the upper-left corner of the source.</param>
        /// <param name="sourceWidth">The width of the source image.</param>
        /// <param name="sourceStride">The offset in the source between one Y coordinate and the next.</param>
        /// <param name="sourceFormat">The source pixel format.</param>
        /// <param name="sourceRectangle">The rectangle in the source to decode.</param>
        /// <param name="target">A pointer to the upper-left corner of the target.</param>
        /// <param name="targetWidth">The width of the target image.</param>
        /// <param name="targetStride">The offset in the target between one Y coordinate and the next.</param>
        /// <param name="targetFormat">The target pixel format.</param>
        /// <param name="targetX">The X coordinate in the target that the leftmost pixel should be placed into.</param>
        /// <param name="targetY">The Y coordinate in the target that the topmost pixel should be placed into.</param>
        public static unsafe void Copy(
            byte[] source,
            int sourceWidth,
            int sourceStride,
            VncPixelFormat sourceFormat,
            VncRectangle sourceRectangle,
            byte[] target,
            int targetWidth,
            int targetStride,
            VncPixelFormat targetFormat,
            int targetX = 0,
            int targetY = 0)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
                
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (sourceRectangle.IsEmpty)
            {
                return;
            }

            int x = sourceRectangle.X, w = sourceRectangle.Width;
            int y = sourceRectangle.Y, h = sourceRectangle.Height;

            if (sourceFormat.Equals(targetFormat))
            {
                if (sourceRectangle.Width == sourceWidth
                    && sourceWidth == targetWidth
                    && sourceStride == targetStride)
                {
                    int sourceStart = sourceStride * y;
                    int length = targetStride * h;

                    Buffer.BlockCopy(source, sourceStart, target, 0, length);
                }
                else
                {
                    for (int iy = 0; iy < h; iy++)
                    {
                        int sourceStart = (sourceStride * (iy + y)) + (x * sourceFormat.BitsPerPixel / 8);
                        int targetStart = targetStride * iy;

                        int length = w * sourceFormat.BitsPerPixel / 8;

                        Buffer.BlockCopy(source, sourceStart, target, targetStart, length);
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        /// Copies pixels. A format conversion is performed if necessary.
        /// </para>
        /// <para>
        /// Be sure to lock <see cref="VncFramebuffer.SyncRoot"/> first to avoid tearing,
        /// if the connection is active.
        /// </para>
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
        public static unsafe void Copy(
            IntPtr source,
            int sourceStride,
            VncPixelFormat sourceFormat,
            VncRectangle sourceRectangle,
            IntPtr target,
            int targetStride,
            VncPixelFormat targetFormat,
            int targetX = 0,
            int targetY = 0)
        {
            if (source == IntPtr.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(source));
            }

            if (target == IntPtr.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(target));
            }

            if (sourceFormat == null)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceFormat));
            }

            if (targetFormat == null)
            {
                throw new ArgumentNullException(nameof(targetFormat));
            }

            if (sourceRectangle.IsEmpty)
            {
                return;
            }

            int x = sourceRectangle.X, w = sourceRectangle.Width;
            int y = sourceRectangle.Y, h = sourceRectangle.Height;

            var sourceData = (byte*)(void*)source + (y * sourceStride) + (x * sourceFormat.BytesPerPixel);
            var targetData = (byte*)(void*)target + (targetY * targetStride) + (targetX * targetFormat.BytesPerPixel);

            if (sourceFormat.Equals(targetFormat))
            {
                for (int iy = 0; iy < h; iy++)
                {
                    if (sourceFormat.BytesPerPixel == 4)
                    {
                        uint* sourceDataX0 = (uint*)sourceData, targetDataX0 = (uint*)targetData;
                        for (int ix = 0; ix < w; ix++)
                        {
                            *targetDataX0++ = *sourceDataX0++;
                        }
                    }
                    else
                    {
                        int bytes = w * sourceFormat.BytesPerPixel;
                        byte* sourceDataX0 = (byte*)sourceData, targetDataX0 = (byte*)targetData;
                        for (int ib = 0; ib < bytes; ib++)
                        {
                            *targetDataX0++ = *sourceDataX0++;
                        }
                    }

                    sourceData += sourceStride;
                    targetData += targetStride;
                }
            }
        }

        /// <summary>
        /// Copies a region of the framebuffer into a bitmap.
        /// </summary>
        /// <param name="source">The framebuffer to read.</param>
        /// <param name="sourceRectangle">The framebuffer region to copy.</param>
        /// <param name="scan0">The bitmap buffer start address.</param>
        /// <param name="stride">The bitmap width stride.</param>
        /// <param name="targetX">The leftmost X coordinate of the bitmap to draw to.</param>
        /// <param name="targetY">The topmost Y coordinate of the bitmap to draw to.</param>
        public static unsafe void CopyFromFramebuffer(
            VncFramebuffer source,
            VncRectangle sourceRectangle,
            IntPtr scan0,
            int stride,
            int targetX,
            int targetY)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sourceRectangle.IsEmpty)
            {
                return;
            }

            fixed (byte* framebufferData = source.GetBuffer())
            {
                VncPixelFormat.Copy(
                    (IntPtr)framebufferData,
                    source.Stride,
                    source.PixelFormat,
                    sourceRectangle,
                    scan0,
                    stride,
                    source.PixelFormat);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var format = obj as VncPixelFormat;

            if (format != null)
            {
                if (this.BitsPerPixel == format.BitsPerPixel && this.BitDepth == format.BitDepth &&
                    this.RedBits == format.RedBits && this.RedShift == format.RedShift &&
                    this.GreenBits == format.GreenBits && this.GreenShift == format.GreenShift &&
                    this.BlueBits == format.BlueBits && this.BlueShift == format.BlueShift &&
                    this.IsLittleEndian == format.IsLittleEndian && this.IsPalettized == format.IsPalettized)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.BitsPerPixel ^ this.RedBits;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.BitsPerPixel} bpp; {this.BitDepth} depth: R {this.RedBits} {this.RedShift}; G {this.GreenBits} {this.GreenShift}; B {this.BlueBits} {this.BlueShift}; LE: {this.IsLittleEndian}; Palettized {this.IsPalettized}";
        }

        /// <summary>
        /// Decodes a <see cref="VncPixelFormat"/> from a <see cref="byte"/> array.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="byte"/> array which contains the <see cref="VncPixelFormat"/> data.
        /// </param>
        /// <param name="offset">
        /// The first index in the <paramref name="buffer"/> which contains the <see cref="VncPixelFormat"/>
        /// data.
        /// </param>
        /// <returns>
        /// A <see cref="VncPixelFormat"/> object.
        /// </returns>
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

            return new VncPixelFormat(
                bitsPerPixel,
                depth,
                redBits,
                redShift,
                greenBits,
                greenShift,
                blueBits,
                blueShift,
                isLittleEndian,
                isPalettized);
        }

        /// <summary>
        /// Serializes this <see cref="VncPixelFormat"/> to a <see cref="byte"/> array.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="byte"/> array to which to encode the <see cref="VncPixelFormat"/> object.
        /// </param>
        /// <param name="offset">
        /// The first <see cref="byte"/> at which to store the <see cref="VncPixelFormat"/> data.
        /// </param>
        internal void Encode(byte[] buffer, int offset)
        {
            buffer[offset + 0] = (byte)this.BitsPerPixel;
            buffer[offset + 1] = (byte)this.BitDepth;
            buffer[offset + 2] = (byte)(this.IsLittleEndian ? 0 : 1);
            buffer[offset + 3] = (byte)(this.IsPalettized ? 0 : 1);
            VncUtility.EncodeUInt16BE(buffer, offset + 4, this.RedMax);
            VncUtility.EncodeUInt16BE(buffer, offset + 6, this.GreenMax);
            VncUtility.EncodeUInt16BE(buffer, offset + 8, this.BlueMax);
            buffer[offset + 10] = (byte)this.RedShift;
            buffer[offset + 11] = (byte)this.GreenShift;
            buffer[offset + 12] = (byte)this.BlueShift;
        }

        private static int BitsFromMax(int max)
        {
            if (max == 0 || (max & (max + 1)) != 0)
            {
                throw new ArgumentException();
            }

            return (int)Math.Round(Math.Log(max + 1) / Math.Log(2));
        }
    }
}
