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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Defines the encoding format used by the client and the server.
    /// </summary>
    internal enum VncEncoding
    {
        /// <summary>
        /// The data is encoded in the raw encoding format.
        /// </summary>
        /// <remarks>
        /// In this case the data consists of width × height pixel values (where width and height
        /// are the width and height of the rectangle). The values simply represent each pixel in
        /// left-to-right scanline order
        /// </remarks>
        Raw = 0,

        /// <summary>
        /// The CopyRect (copy rectangle) encoding is a very simple and efficient encoding which
        /// can be used when the client already has the same pixel data elsewhere in its framebuffer.
        /// The encoding on the wire simply consists of an X,Y coordinate. This gives a
        /// position in the framebuffer from which the client can copy the rectangle of pixel data.
        /// </summary>
        CopyRect = 1,

        /// <summary>
        /// Rectangles are split up into 16x16 tiles, allowing the dimensions of the subrectangles to be
        /// specified in 4 bits each, 16 bits in total. The rectangle is split into tiles starting at the
        /// top left going in left-to-right, topto-bottom order. The encoded contents of the tiles simply
        /// follow one another in the predetermined order.
        /// </summary>
        Hextile = 5,

        /// <summary>
        /// Combines zlib compression, tiling, palettisation and run-length encoding. On the wire, the rectangle
        /// begins with a 4-byte length field, and is followed by that many bytes of zlib-compressed data.
        /// </summary>
        Zlib = 6,

        /// <summary>
        /// A client which requests the Cursor pseudo-encoding is declaring that it is capable of
        /// drawing a mouse cursor locally. This can significantly improve perceived performance
        /// over slow links
        /// </summary>
        PseudoCursor = -239, // TODO: KVM doesn't use this one for me... Find some way to test it...

        /// <summary>
        /// A client which requests the DesktopSize pseudo-encoding is declaring that it is capable
        /// of coping with a change in the framebuffer width and/or height.
        /// </summary>
        PseudoDesktopSize = -223
    }
}
