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

using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;
using System;
using System.Diagnostics;
using System.IO;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Tight encoding provides efficient compression for pixel data, using zlib compression (over 4 multiple zlib streams),
    /// JPEG compression or fill compression, and optionally applying filters to the raw pixel data.
    /// </summary>
    /// <remarks>
    /// The <see cref="TightEncoder"/> currently only implements zlib compression over a single zlib stream,
    /// which is reset after every rectangle.
    /// </remarks>
    /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#tight-encoding"/>
    /// <seealso href="https://virtualgl.org/pmwiki/uploads/About/tighttoturbo.pdf"/>
    public class TightEncoder : VncEncoder
    {
        /// <summary>
        /// Mapping of Tight quality levels to JPEG quality levels.
        /// </summary>
        /// <seealso href=""/>
        private static readonly int[] QualityLevels = new int[] { 5, 10, 15, 25, 37, 50, 60, 70, 75, 80, 0 };

        /// <summary>
        /// Initializes a new instance of the <see cref="TightEncoder"/> class.
        /// </summary>
        /// <param name="vncServerSession">
        /// The parent session to which this <see cref="TightEncoder"/> is linked.
        /// </param>
        public TightEncoder(IVncServerSession vncServerSession)
        {
            this.VncServerSession = vncServerSession ?? throw new ArgumentNullException(nameof(vncServerSession));
        }

        /// <inheritdoc/>
        public override VncEncoding Encoding => VncEncoding.Tight;

        /// <summary>
        /// Gets the <see cref="VncServerSession"/> to which this <see cref="TightEncoder"/> is linked.
        /// </summary>
        protected IVncServerSession VncServerSession { get; private set; }

        /// <summary>Encode a value into a byte array</summary>
        /// <param name="buffer">The byte array to encode the 7-bit integer into</param>
        /// <param name="startIndex">The index of the first byte for the 7-bit integer</param>
        /// <param name="value">The value to encode into <paramref name="buffer"/></param>
        /// <returns>Index of the first byte after the encoded value in <paramref name="buffer"/></returns>
        public static int WriteEncodedValue(byte[] buffer, int startIndex, int value)
        {
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value;
            for (; v >= 0x80; v >>= 7, startIndex++)
            {
                buffer[startIndex] = (byte)(v | 0x80);
            }

            buffer[startIndex++] = (byte)v;

            return startIndex;
        }

        /// <summary>
        /// Gets the zlib compression level to use.
        /// </summary>
        /// <param name="vncServerSession">
        /// The current VNC server session.
        /// </param>
        /// <returns>
        /// The <see cref="CompressionLevel"/> to use for zlib streams for this session.
        /// </returns>
        public static CompressionLevel GetCompressionLevel(IVncServerSession vncServerSession)
        {
            return (CompressionLevel)GetLevel(vncServerSession, VncEncoding.TightCompressionLevel0, VncEncoding.TightCompressionLevel9, (int)CompressionLevel.Default);
        }

        /// <summary>
        /// Gets the JPEG quality level to use.
        /// </summary>
        /// <param name="vncServerSession">
        /// The current VNC server session.
        /// </param>
        /// <returns>
        /// The JPEG quality level to use, or 0 when JPEG compression should be disabled.
        /// </returns>
        public static int GetQualityLevel(IVncServerSession vncServerSession)
        {
            return QualityLevels[GetLevel(vncServerSession, VncEncoding.TightQualityLevel0, VncEncoding.TightQualityLevel9, 10)];
        }

        /// <inheritdoc/>
        public override void Send(Stream stream, VncPixelFormat pixelFormat, byte[] contents)
        {
            if (contents.Length < 12)
            {
                var compressionControl = TightCompressionControl.BasicCompression;
                stream.WriteByte((byte)compressionControl);

                // If the data size after applying the filter but before the compression is less then 12, then the data is sent as is, uncompressed.
                byte[] encodedBuffer = new byte[4];
                var length = WriteEncodedValue(encodedBuffer, 0, (int)contents.Length);
                stream.Write(encodedBuffer, 0, length);

                stream.Write(contents, 0, contents.Length);
            }
            else
            {
                var compressionControl = TightCompressionControl.BasicCompression
                    | TightCompressionControl.ResetStream0
                    | TightCompressionControl.UseStream0;

                var compressionLevel = GetCompressionLevel(this.VncServerSession);

                stream.WriteByte((byte)compressionControl);

                using (var buffer = new MemoryStream())
                using (var deflater = new ZlibStream(buffer, CompressionMode.Compress, compressionLevel))
                {
                    // The Tight encoding makes use of a new type TPIXEL (Tight pixel). This is the same as a PIXEL for the agreed
                    // pixel format, except where true-colour-flag is non-zero, bits-per-pixel is 32, depth is 24 and all of the bits
                    // making up the red, green and blue intensities are exactly 8 bits wide.
                    // In this case a TPIXEL is only 3 bytes long, where the first byte is the red component, the second byte is the
                    // green component, and the third byte is the blue component of the pixel color value.
                    if (pixelFormat.BitsPerPixel == 32
                        && pixelFormat.BitDepth == 24
                        && pixelFormat.BlueBits == 8
                        && pixelFormat.RedBits == 8
                        && pixelFormat.GreenBits == 8
                        && !pixelFormat.IsPalettized)
                    {
                        Debug.Assert(contents.Length % 4 == 0, "The size of the raw pixel data must be a multiple of 4 when using a 32bpp pixel format.");

                        for (int i = 0; i < contents.Length; i += 4)
                        {
                            if (i == contents.Length - 4)
                            {
                                deflater.FlushMode = FlushType.Full;
                            }

                            deflater.Write(contents, i, 3);
                        }
                    }
                    else
                    {
                        deflater.FlushMode = FlushType.Finish;
                        deflater.Write(contents, 0, contents.Length);
                    }

                    deflater.Flush();

                    byte[] encodedBuffer = new byte[4];
                    var length = WriteEncodedValue(encodedBuffer, 0, (int)buffer.Length);
                    stream.Write(encodedBuffer, 0, length);

                    buffer.Position = 0;
                    buffer.CopyTo(stream);
                }
            }
        }

        private static int GetLevel(IVncServerSession vncServerSession, VncEncoding lower, VncEncoding upper, int defaultValue)
        {
            if (vncServerSession?.ClientEncodings == null)
            {
                return defaultValue;
            }

            foreach (var encoding in vncServerSession.ClientEncodings)
            {
                if (encoding >= lower && encoding <= upper)
                {
                    return encoding - lower;
                }
            }

            return defaultValue;
        }
    }
}
