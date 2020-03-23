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

#if NET461
using System;
using System.Drawing;
using System.Windows.Forms;
using RemoteViewing.Vnc;

namespace RemoteViewing.Windows.Forms.Server
{
    /// <summary>
    /// Called to determine the screen region to send.
    /// </summary>
    /// <returns>The screen region.</returns>
    public delegate Rectangle VncScreenFramebufferSourceGetBoundsCallback();

    /// <summary>
    /// Provides a framebuffer with pixels copied from the screen.
    /// </summary>
    public class VncScreenFramebufferSource : IVncFramebufferSource
    {
        private Bitmap bitmap;
        private VncFramebuffer framebuffer;
        private string name;
        private VncScreenFramebufferSourceGetBoundsCallback getScreenBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncScreenFramebufferSource"/> class.
        /// </summary>
        /// <param name="name">The framebuffer name. Many VNC clients set their titlebar to this name.</param>
        /// <param name="screen">The bounds of the screen region.</param>
        public VncScreenFramebufferSource(string name, Screen screen)
        {
            if (screen == null)
            {
                throw new ArgumentNullException(nameof(screen));
            }

            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.getScreenBounds = () => screen.Bounds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VncScreenFramebufferSource"/> class.
        /// Screen region bounds are determined by a callback.
        /// </summary>
        /// <param name="name">The framebuffer name. Many VNC clients set their titlebar to this name.</param>
        /// <param name="getBoundsCallback">A callback supplying the bounds of the screen region to copy.</param>
        public VncScreenFramebufferSource(string name, VncScreenFramebufferSourceGetBoundsCallback getBoundsCallback)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.getScreenBounds = getBoundsCallback ?? throw new ArgumentNullException(nameof(getBoundsCallback));
        }

        /// <summary>
        /// Captures the screen.
        /// </summary>
        /// <returns>A framebuffer corresponding to the screen.</returns>
        public VncFramebuffer Capture()
        {
            var bounds = this.getScreenBounds();
            int w = bounds.Width, h = bounds.Height;

            if (this.bitmap == null || this.bitmap.Width != w || this.bitmap.Height != h)
            {
                this.bitmap = new Bitmap(w, h);
                this.framebuffer = new VncFramebuffer(this.name, w, h, new VncPixelFormat());
            }

            using (var g = Graphics.FromImage(this.bitmap))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);

                lock (this.framebuffer.SyncRoot)
                {
                    VncBitmap.CopyToFramebuffer(
                        this.bitmap,
                        new VncRectangle(0, 0, w, h),
                        this.framebuffer,
                        0,
                        0);
                }
            }

            return this.framebuffer;
        }
    }
}
#endif
