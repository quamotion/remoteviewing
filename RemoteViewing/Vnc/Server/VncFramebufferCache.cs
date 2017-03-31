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
using System.Runtime.InteropServices;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Caches the <see cref="VncFramebuffer"/> pixel data and updates them as new
    /// <see cref="VncFramebuffer"/> commands are received.
    /// </summary>
    internal sealed class VncFramebufferCache
    {
        // The size of the tiles which will be invalidated. So we're basically
        // dividing the framebuffer in blocks of 32x32 and are invalidating them one at a time.
        private const int TileSize = 64;

        private readonly bool[] isLineInvalid;

        // We cache the latest framebuffer data as it was sent to the client. When looking for changes,
        // we compare with the framebuffer which is cached here and send the deltas (for each time
        // which was invalidate) to the client.
        private VncFramebuffer cachedFramebuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncFramebufferCache"/> class.
        /// </summary>
        /// <param name="framebuffer">
        /// The <see cref="VncFramebuffer"/> to cache.
        /// </param>
        public VncFramebufferCache(VncFramebuffer framebuffer)
        {
            Throw.If.Null(framebuffer, "framebuffer");
            this.Framebuffer = framebuffer;
            this.cachedFramebuffer = new VncFramebuffer(framebuffer.Name, framebuffer.Width, framebuffer.Height, framebuffer.PixelFormat);

            this.isLineInvalid = new bool[this.Framebuffer.Height];
        }

        /// <summary>
        /// Gets an up-to-date and complete <see cref="VncFramebuffer"/>.
        /// </summary>
        public VncFramebuffer Framebuffer
        {
            get;
            private set;
        }

        /// <summary>
        /// Responds to a <see cref="VncServerSession"/> update request.
        /// </summary>
        /// <param name="session">
        /// The session on which the update request was received.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the operation completed successfully; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public unsafe bool RespondToUpdateRequest(VncServerSession session)
        {
            VncRectangle subregion = default(VncRectangle);

            var fb = this.Framebuffer;
            var fbr = session.FramebufferUpdateRequest;
            if (fb == null || fbr == null)
            {
                return false;
            }

            var incremental = fbr.Incremental;
            var region = fbr.Region;
            int bpp = fb.PixelFormat.BytesPerPixel;

            session.FramebufferManualBeginUpdate();

            // Take a lock here, as we will modify
            // both buffers heavily in the next block.
            lock (fb.SyncRoot)
            {
                lock (this.cachedFramebuffer.SyncRoot)
                {
                    // Get the buffers (byte arrays) of data. We'll compare the data one pixel at a time.
                    var actualBuffer = this.Framebuffer.GetBuffer();
                    var bufferedBuffer = this.cachedFramebuffer.GetBuffer();

                    for (int y = region.Y; y < region.Y + region.Height; y++)
                    {
                        subregion.X = region.X;
                        subregion.Y = y;
                        subregion.Width = region.Width;
                        subregion.Height = 1;

                        bool isValid = true;

                        // For a given y, the x pixels are stored sequentially in the array
                        // starting at y * stride (number of bytes per row); for each x
                        // value there are bpp bytes of data (4 for a 32-bit integer); we are looking
                        // for pixels between x and x + w so this translates to
                        // y * stride + bpp * x and y * stride + bpp * (x + w)
                        int srcOffset = (y * this.Framebuffer.Stride) + (bpp * region.X);
                        int length = bpp * region.Width;

                        fixed (byte* actualLinePtr = actualBuffer, bufferedLinePtr = bufferedBuffer)
                        {
                            isValid = NativeMethods.memcmp(actualLinePtr + srcOffset, bufferedLinePtr + srcOffset, (uint)length) == 0;
                        }

                        if (!isValid)
                        {
                            try
                            {
                                Buffer.BlockCopy(actualBuffer, srcOffset, bufferedBuffer, srcOffset, length);
                            }
                            catch
                            {
                                throw;
                            }
                        }

                        this.isLineInvalid[y - region.Y] = !isValid;
                    }
                } // lock
            } // lock

            if (incremental)
            {
                int? y = null;

                for (int line = 0; line < region.Height; line++)
                {
                    if (y == null && this.isLineInvalid[line])
                    {
                        y = region.Y + line;
                    }

                    if (y != null && (!this.isLineInvalid[line] || line == region.Height))
                    {
                        // Flush
                        subregion.X = region.X;
                        subregion.Y = region.Y + y.Value;
                        subregion.Width = region.Width;
                        subregion.Height = line - y.Value;
                        session.FramebufferManualInvalidate(subregion);
                        y = null;
                    }
                }
            }
            else
            {
                session.FramebufferManualInvalidate(region);
            }

            return session.FramebufferManualEndUpdate();
        }
    }
}
