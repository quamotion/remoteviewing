#region License
/*
RemoteViewing VNC Client Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com>
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
        /// Decodes a region of the framebuffer into a bitmap.
        /// </summary>
        /// <param name="framebuffer">The framebuffer to read.</param>
        /// <param name="rect">The region to decode.</param>
        /// <param name="bitmap">The bitmap to decode into.</param>
        public unsafe static void DecodeFramebufferRegion(VncFramebuffer framebuffer, VncRectangle rect,
                                                 Bitmap bitmap)
        {
            if (bitmap == null) { throw new ArgumentNullException("bitmap"); }
            if (framebuffer == null) { throw new ArgumentNullException("framebuffer"); }
            if (rect.IsEmpty) { return; }

            var winformsRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            var data = bitmap.LockBits(winformsRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                fixed (byte* framebufferData = framebuffer.GetBuffer())
                {
                    framebuffer.PixelFormat.DecodeTo32bpp((IntPtr)framebufferData, framebuffer.Stride, rect,
                                                          data.Scan0, data.Stride);
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }
    }
}
