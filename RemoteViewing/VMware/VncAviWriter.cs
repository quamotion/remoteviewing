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

using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Messages;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteViewing.VMware
{
    /// <summary>
    /// The <see cref="VncAviWriter"/> stores a RFB session into an AVI file.
    /// </summary>
    /// <seealso href="https://cdn.hackaday.io/files/274271173436768/avi.pdf"/>
    /// <seealso href="http://www.jmcgowan.com/odmlff2.pdf"/>
    public class VncAviWriter
    {
        private readonly IVncFramebufferSource framebufferSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncAviWriter"/> class.
        /// </summary>
        /// <param name="framebufferSource">
        /// The <see cref="IVncFramebufferSource"/> which provides framebuffers which act as input for the
        /// session.
        /// </param>
        public VncAviWriter(IVncFramebufferSource framebufferSource)
        {
            this.framebufferSource = framebufferSource ?? throw new ArgumentNullException(nameof(framebufferSource));
        }

        /// <summary>
        /// Gets the amount of time, in microseconds, each frame is displayed.
        /// </summary>
        public int MicrosecondsPerFrame
        {
            get { return (int)(this.Rate / 1_000_000d * this.Scale); }
        }

        /// <summary>
        /// Gets or sets the expected size of the AVI file. Set to the maximum value by default,
        /// useful when streaming to FFmpeg.
        /// </summary>
        [CLSCompliant(false)]
        public uint ExpectedSize
        { get; set; } = uint.MaxValue;

        /// <summary>
        /// Gets or sets the expected number of frames in the AVI file. Set to the maximum value by default,
        /// useful when streaming to FFmpeg.
        /// </summary>
        public int ExpectedTotalFrames
        { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the rate, in frames per second, of the video stream. The rate
        /// is divided by <see cref="Scale"/>.
        /// </summary>
        public int Rate
        { get; set; } = 1_000_000;

        /// <summary>
        /// Gets or sets the scale to use for <see cref="Rate"/>.
        /// </summary>
        public int Scale
        { get; set; } = 200_000;

        /// <summary>
        /// Asynchronously writes framebuffer packages to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to which to write.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the asynchronous operation. This method
        /// will keep recording until cancellation is requested.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        public async Task WriteAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            VncFramebuffer framebuffer = this.framebufferSource.Capture();

            // The RIFF header, wraps the entire file.
            await stream.WriteStructAsync(
                new List()
                {
                    ListType = FourCC.Riff,
                    FourCC = FourCC.Avi,
                    Size = this.ExpectedSize
                },
                cancellationToken).ConfigureAwait(false);

            // HDRL list. Defines the format of the data and is the first required LIST chunk.
            await stream.WriteStructAsync(
                new List()
                {
                    ListType = FourCC.List,
                    FourCC = FourCC.Hdrl,
                    Size = 0x449c // TBC
                },
                cancellationToken).ConfigureAwait(false);

            // AVIH chunk. Contains the main AVI header.
            await stream.WriteStructAsync(
                new Chunk()
                {
                    FourCC = FourCC.Avih,
                    Size = Unsafe.SizeOf<AviMainHeader>()
                },
                cancellationToken).ConfigureAwait(false);

            await stream.WriteStructAsync(
                new AviMainHeader()
                {
                    Flags = MainHeaderFlags.HasIndex,
                    Height = framebuffer.Height,
                    Width = framebuffer.Width,
                    MaxBytesPerSec = 1448660, // TBC
                    MicroSecPerFrame = this.MicrosecondsPerFrame,
                    Streams = 1,
                    SuggestedBufferSize = 0x046bc4, // TBC
                    TotalFrames = this.ExpectedTotalFrames,
                },
                cancellationToken).ConfigureAwait(false);

            // STRL list. Contains informatio about a stream.
            await stream.WriteStructAsync(
                new List()
                {
                    ListType = FourCC.List,
                    FourCC = FourCC.Strl,
                    Size = 17220,
                },
                cancellationToken).ConfigureAwait(false);

            // STRH chunk
            await stream.WriteStructAsync(
                new Chunk()
                {
                    FourCC = FourCC.Strh,
                    Size = 0x38
                },
                cancellationToken).ConfigureAwait(false);

            await stream.WriteStructAsync(
                new AviStreamHeader()
                {
                    Type = FourCC.Vids,
                    Handler = FourCC.VMnc,
                    Length = this.ExpectedTotalFrames,
                    Quality = -1,
                    Rate = this.Rate,
                    Scale = this.Scale,
                    SuggestedBufferSize = 0x00046bc4,
                    Bottom = (short)framebuffer.Height,
                    Right = (short)framebuffer.Width
                },
                cancellationToken).ConfigureAwait(false);

            // STRF chunk
            await stream.WriteStructAsync(
                new Chunk()
                {
                    FourCC = FourCC.Strf,
                    Size = 0x28
                },
                cancellationToken).ConfigureAwait(false);

            // BitmapInfoHeader
            await stream.WriteStructAsync(
                new BitmapInfoHeader()
                {
                    BitCount = 0x20,
                    Compression = FourCC.VMnc,
                    Height = framebuffer.Height,
                    Planes = 1,
                    Size = 0x28, // TBC
                    Width = framebuffer.Width
                },
                cancellationToken).ConfigureAwait(false);

            // Index chunk
            var indexSize = 0x000042c8; // TBC
            await stream.WriteStructAsync(
                new AviIndexChunck()
                {
                    FourCC = FourCC.Indx,
                    Size = indexSize,
                    EntriesInUse = 1,
                    ChunkId = FourCC.Dc00,
                    LongsPerEntry = 4,
                    IndexType = IndexType.IndexOfIndexes,
                    IndexSubType = IndexSubType.None
                },
                cancellationToken).ConfigureAwait(false);

            // Super index chunk
            await stream.WriteStructAsync(
                new AviSuperIndexEntry()
                {
                    Duration = this.ExpectedTotalFrames,
                    Offset = 0x000000000004196a,
                    Size = 0x4000
                },
                cancellationToken);

            await stream.GrowAsync(indexSize - Unsafe.SizeOf<AviSuperIndexEntry>() - Unsafe.SizeOf<AviIndexChunck>() + Unsafe.SizeOf<Chunk>(), cancellationToken).ConfigureAwait(false);

            // ODML chunk
            await stream.WriteStructAsync(
                new List()
                {
                    ListType = FourCC.List,
                    FourCC = FourCC.Odml,
                    Size = 0x00000104
                },
                cancellationToken).ConfigureAwait(false);

            // DMLH chunk
            indexSize = 0xf8;
            await stream.WriteStructAsync(
                new AviIndexChunck()
                {
                    FourCC = FourCC.Dmlh,
                    Size = indexSize,
                    IndexType = IndexType.IndexOfIndexes,
                    IndexSubType = IndexSubType.None,
                    ChunkId = (FourCC)BinaryPrimitives.ReverseEndianness((uint)VncEncoding.Raw),
                    EntriesInUse = 0,
                    LongsPerEntry = 0x13e
                },
                cancellationToken);

            await stream.GrowAsync(indexSize - Unsafe.SizeOf<AviIndexChunck>() + Unsafe.SizeOf<Chunk>(), cancellationToken).ConfigureAwait(false);

            var junkSize = 0x00000348;
            await stream.WriteStructAsync(
                new Chunk()
                {
                    FourCC = FourCC.Junk,
                    Size = junkSize // TBC
                },
                cancellationToken).ConfigureAwait(false);

            var junk = "VMware Workstation";
            byte[] buffer = new byte[128];
            Encoding.ASCII.GetBytes(junk, 0, junk.Length, buffer, 0);
            stream.Write(buffer, 0, junk.Length);

            await stream.GrowAsync(junkSize - junk.Length, cancellationToken).ConfigureAwait(false);

            // The movie list
            await stream.WriteStructAsync(
                new List()
                {
                    FourCC = FourCC.Movi,
                    ListType = FourCC.List,
                    Size = this.ExpectedSize - 0x4800
                },
                cancellationToken);

            FramebufferUpdate framebufferUpdate = new FramebufferUpdate()
            {
                MessageType = 0,
                NumberOfRectangles = 2
            };

            FramebufferUpdateRectangle rectangle = new FramebufferUpdateRectangle();
            DisplayModeChange displayModeChange = new DisplayModeChange();

            Chunk dcChunk = new Chunk()
            {
                FourCC = FourCC.Dc00,
                Size =
                    4
                    + (2 * framebufferUpdate.Buffer.Length)
                    + displayModeChange.Buffer.Length
                    + (framebuffer.Stride * framebuffer.Width * framebuffer.PixelFormat.BytesPerPixel)
            };

            Stopwatch timer = new Stopwatch();

            while (!cancellationToken.IsCancellationRequested)
            {
                // Start a timer which will timeout after the timeframe allotted to each frame.
                timer.Restart();
                var interval = Task.Delay(this.MicrosecondsPerFrame / 1000, cancellationToken).ConfigureAwait(false);
                framebuffer = this.framebufferSource.Capture();

                if (framebuffer == null)
                {
                    break;
                }

                rectangle.Width = (ushort)framebuffer.Width;
                rectangle.Height = (ushort)framebuffer.Height;

                await stream.WriteStructAsync(dcChunk, cancellationToken).ConfigureAwait(false);

                // Framebuffer update message, with 2 rectangles
                await stream.WriteAsync(framebufferUpdate.Buffer, cancellationToken).ConfigureAwait(false);

                // Pseudo-rectangle: display mode change pseudo-encoding
                rectangle.EncodingType = VncEncoding.VMWi;
                await stream.WriteAsync(rectangle.Buffer, cancellationToken).ConfigureAwait(false);

                displayModeChange.BitsPerSample = (byte)framebuffer.PixelFormat.BitsPerPixel;
                displayModeChange.Depth = (byte)framebuffer.PixelFormat.BitDepth;
                displayModeChange.MaxBlue = framebuffer.PixelFormat.BlueMax;
                displayModeChange.MaxGreen = framebuffer.PixelFormat.GreenMax;
                displayModeChange.MaxRed = framebuffer.PixelFormat.RedMax;
                displayModeChange.BlueShift = (byte)framebuffer.PixelFormat.BlueShift;
                displayModeChange.GreenShift = (byte)framebuffer.PixelFormat.GreenShift;
                displayModeChange.RedShift = (byte)framebuffer.PixelFormat.RedShift;
                displayModeChange.TrueColor = !framebuffer.PixelFormat.IsPalettized;
                await stream.WriteAsync(displayModeChange.Buffer, cancellationToken).ConfigureAwait(false);

                // Rectangle: framebuffer
                rectangle.EncodingType = VncEncoding.Raw;
                await stream.WriteAsync(rectangle.Buffer, cancellationToken).ConfigureAwait(false);

                var rawFramebuffer = framebuffer.GetBuffer();
                await stream.WriteAsync(rawFramebuffer, 0, rawFramebuffer.Length, cancellationToken).ConfigureAwait(false);
                Debug.WriteLine($"Completed in {timer.ElapsedMilliseconds} ms out of allowed {this.MicrosecondsPerFrame / 1000} ms");

                // Wait for the timer to complete.
                await interval;
            }
        }
    }
}
