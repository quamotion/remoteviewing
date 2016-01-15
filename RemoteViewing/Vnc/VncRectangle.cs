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

            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the number of pixels.
        /// </summary>
        public int Area
        {
            get { return this.Width * this.Height; }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the leftmost changed pixel.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of the topmost changed pixel.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the changed region.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the changed region.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets a value indicating whether the region is empty.
        /// </summary>
        /// <value>
        /// <c>true</c> if the region contains no pixels.
        /// </value>
        public bool IsEmpty
        {
            get { return this.Width == 0 || this.Height == 0; }
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

        /// <summary>
        /// Intersects two rectangles.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns>The intersection of the two.</returns>
        public static VncRectangle Intersect(VncRectangle rect1, VncRectangle rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect1;
            }
else if (rect2.IsEmpty)
{
    return rect2;
}

            int x = Math.Max(rect1.X, rect2.X), y = Math.Max(rect1.Y, rect2.Y);
            int w = Math.Min(rect1.X + rect1.Width, rect2.X + rect2.Width) - x;
            int h = Math.Min(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - y;
            return w > 0 && h > 0 ? new VncRectangle(x, y, w, h) : default(VncRectangle);
        }

        /// <summary>
        /// Finds a region that contains both rectangles.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns>The union of the two.</returns>
        public static VncRectangle Union(VncRectangle rect1, VncRectangle rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2;
            }
else if (rect2.IsEmpty)
{
    return rect1;
}

            int x = Math.Min(rect1.X, rect2.X), y = Math.Min(rect1.Y, rect2.Y);
            int w = Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width) - x;
            int h = Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height) - y;
            return new VncRectangle(x, y, w, h);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is VncRectangle && this.Equals((VncRectangle)obj);
        }

        /// <summary>
        /// Compares the rectangle with another rectangle for equality.
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        /// <returns><c>true</c> if the rectangles are equal.</returns>
        public bool Equals(VncRectangle other)
        {
            return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.X | this.Y << 16;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0}x{1} at {2}, {3}", this.Width, this.Height, this.X, this.Y);
        }
    }
}
