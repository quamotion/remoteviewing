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
using System.Drawing;
using System.Drawing.Imaging;
using RemoteViewing.Vnc;

namespace RemoteViewing.Windows.Forms
{
    /// <summary>
    /// Helps with Windows Forms bitmap conversion.
    /// </summary>
    public static class VncBitmap
    {
        /// <summary>
        /// Copies a region of a bitmap into the framebuffer.
        /// </summary>
        /// <param name="source">The bitmap to read.</param>
        /// <param name="sourceRectangle">The bitmap region to copy.</param>
        /// <param name="target">The framebuffer to copy into.</param>
        /// <param name="targetX">The leftmost X coordinate of the framebuffer to draw to.</param>
        /// <param name="targetY">The topmost Y coordinate of the framebuffer to draw to.</param>
        public unsafe static void CopyToFramebuffer(Bitmap source, VncRectangle sourceRectangle,
                                                    VncFramebuffer target, int targetX, int targetY)
        {
            Throw.If.Null(source, "source").Null(target, "target");
            if (sourceRectangle.IsEmpty) { return; }

            var winformsRect = new Rectangle(sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height);
            var data = source.LockBits(winformsRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            try
            {
                fixed (byte* framebufferData = target.GetBuffer())
                {
                    VncPixelFormat.Copy(data.Scan0, data.Stride, new VncPixelFormat(), sourceRectangle,
                                        (IntPtr)framebufferData, target.Stride, target.PixelFormat, targetX, targetY);
                }
            }
            finally
            {
                source.UnlockBits(data);
            }
        }

        /// <summary>
        /// Copies a region of the framebuffer into a bitmap.
        /// </summary>
        /// <param name="source">The framebuffer to read.</param>
        /// <param name="sourceRectangle">The framebuffer region to copy.</param>
        /// <param name="target">The bitmap to copy into.</param>
        /// <param name="targetX">The leftmost X coordinate of the bitmap to draw to.</param>
        /// <param name="targetY">The topmost Y coordinate of the bitmap to draw to.</param>
        public unsafe static void CopyFromFramebuffer(VncFramebuffer source, VncRectangle sourceRectangle,
                                                      Bitmap target, int targetX, int targetY)
        {
            Throw.If.Null(source, "source").Null(target, "target");
            if (sourceRectangle.IsEmpty) { return; }

            var winformsRect = new Rectangle(targetX, targetY, sourceRectangle.Width, sourceRectangle.Height);
            var data = target.LockBits(winformsRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                fixed (byte* framebufferData = source.GetBuffer())
                {
                    VncPixelFormat.Copy((IntPtr)framebufferData, source.Stride, source.PixelFormat, sourceRectangle,
                                        data.Scan0, data.Stride, new VncPixelFormat());
                }
            }
            finally
            {
                target.UnlockBits(data);
            }
        }
    }
}
