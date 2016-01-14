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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Bounds the changed pixels of a framebuffer.
    /// </summary>
    public struct VncRectangle : IEquatable<VncRectangle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VncRectangle"/> structure.
        /// </summary>
        /// <param name="x">The X coordinate of the leftmost changed pixel.</param>
        /// <param name="y">The Y coordinate of the topmost changed pixel.</param>
        /// <param name="width">The width of the changed region.</param>
        /// <param name="height">The height of the changed region.</param>
        public VncRectangle(int x, int y, int width, int height)
            : this()
        {
            Throw.If.Negative(x, "x").Negative(y, "y").Negative(width, "width").Negative(height, "height");

            X = x; Y = y; Width = width; Height = height;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is VncRectangle && Equals((VncRectangle)obj);
        }

        /// <summary>
        /// Compares the rectangle with another rectangle for equality.
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        /// <returns><c>true</c> if the rectangles are equal.</returns>
        public bool Equals(VncRectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return X | Y << 16;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0}x{1} at {2}, {3}", Width, Height, X, Y);
        }

        /// <summary>
        /// The X coordinate of the leftmost changed pixel.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The Y coordinate of the topmost changed pixel.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The width of the changed region.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the changed region.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// <c>true</c> if the region contains no pixels.
        /// </summary>
        public bool IsEmpty
        {
            get { return Width == 0 || Height == 0; }
        }

        /// <summary>
        /// Compares two rectangles for equality.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns><c>true</c> if the rectangles are equal.</returns>
        public static bool operator ==(VncRectangle rect1, VncRectangle rect2)
        {
            return rect1.Equals(rect2);
        }

        /// <summary>
        /// Compares two rectangles for inequality.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns><c>true</c> if the rectangles are not equal.</returns>
        public static bool operator !=(VncRectangle rect1, VncRectangle rect2)
        {
            return !rect1.Equals(rect2);
        }
    }
}
