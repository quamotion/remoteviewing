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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Stores pixel data for a VNC session.
    /// </summary>
    public class VncFramebuffer : IVncFramebufferSource
    {
        byte[] _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncFramebuffer"/> class.
        /// </summary>
        /// <param name="name">The framebuffer name. Many VNC clients set their titlebar to this name.</param>
        /// <param name="width">The framebuffer width.</param>
        /// <param name="height">The framebuffer height.</param>
        /// <param name="pixelFormat">The framebuffer pixel format.</param>
        public VncFramebuffer(string name, int width, int height, VncPixelFormat pixelFormat)
        {
            Throw.If.Null(name, "name");
            Throw.If.Negative(width, "width").Negative(height, "height");
            Throw.If.Null(pixelFormat, "pixelFormat");

            Name = name; Width = width; Height = height; PixelFormat = pixelFormat;
            Stride = PixelFormat.BytesPerPixel * Width;
            SyncRoot = new object();

            _buffer = new byte[Width * Height * PixelFormat.BytesPerPixel];
        }

        /// <summary>
        /// Sets the color of a single pixel.
        /// </summary>
        /// <param name="x">The X coordinate of the pixel.</param>
        /// <param name="y">The Y coordinate of the pixel.</param>
        /// <param name="color">The RGB color of the pixel.</param>
        public void SetPixel(int x, int y, int color)
        {
            lock (SyncRoot)
            {
                Throw.If.False((uint)x < (uint)Width, "x");
                Throw.If.False((uint)y < (uint)Height, "y");

                if (PixelFormat.BytesPerPixel == 4)
                {
                    Array.Copy(BitConverter.GetBytes(color), 0, GetBuffer(), y * Stride + x * 4, 4);
                }
            }
        }

        /// <summary>
        /// Returns the memory underlying this framebuffer.
        /// </summary>
        /// <returns>The framebuffer bytes.</returns>
        public byte[] GetBuffer()
        {
            return _buffer;
        }

        /// <summary>
        /// The framebuffer name. Many VNC clients set their titlebar to this name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The framebuffer synchronization object.
        /// 
        /// Lock this before reading the framebuffer to avoid tearing artifacts.
        /// </summary>
        public object SyncRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// The framebuffer width.
        /// </summary>
        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// The framebuffer height.
        /// </summary>
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// The framebuffer stride. This is the number of bytes between one Y coordinate and the next.
        /// </summary>
        public int Stride
        {
            get;
            private set;
        }

        /// <summary>
        /// The framebuffer pixel format.
        /// </summary>
        public VncPixelFormat PixelFormat
        {
            get;
            private set;
        }

        VncFramebuffer IVncFramebufferSource.Capture()
        {
            return this;
        }
    }
}
