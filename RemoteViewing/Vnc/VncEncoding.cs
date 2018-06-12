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
    /// <seealso href="http://www.iana.org/assignments/rfb/rfb.xml#rfb-4"/>
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
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#raw-encoding"/>
        Raw = 0,

        /// <summary>
        /// The CopyRect (copy rectangle) encoding is a very simple and efficient encoding which
        /// can be used when the client already has the same pixel data elsewhere in its framebuffer.
        /// The encoding on the wire simply consists of an X,Y coordinate. This gives a
        /// position in the framebuffer from which the client can copy the rectangle of pixel data.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#copyrect-encoding"/>
        CopyRect = 1,

        /// <summary>
        /// RRE stands for rise-and-run-length encoding and as its name implies, it is essentially a
        /// two-dimensional analogue of run-length encoding. RRE-encoded rectangles arrive at the client
        /// in a form which can be rendered immediately and efficiently by the simplest of graphics engines.
        /// RRE is not appropriate for complex desktops, but can be useful in some situations.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#rre-encoding"/>
        RRE = 2,

        /// <summary>
        /// CoRRE stands for compressed rise-and-run-length encoding and as its name implies, it is a variant
        /// of the above RRE Encoding and as such essentially a two-dimensional analogue of run-length encoding.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#corre-encoding"/>
        CoRRE = 4,

        /// <summary>
        /// Rectangles are split up into 16x16 tiles, allowing the dimensions of the subrectangles to be
        /// specified in 4 bits each, 16 bits in total. The rectangle is split into tiles starting at the
        /// top left going in left-to-right, topto-bottom order. The encoded contents of the tiles simply
        /// follow one another in the predetermined order.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#hextile-encoding"/>
        Hextile = 5,

        /// <summary>
        /// Combines zlib compression, tiling, palettisation and run-length encoding. On the wire, the rectangle
        /// begins with a 4-byte length field, and is followed by that many bytes of zlib-compressed data.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#zlib-encoding"/>
        Zlib = 6,

        /// <summary>
        /// Tight encoding provides efficient compression for pixel data. To reduce implementation complexity, the
        /// width of any Tight-encoded rectangle cannot exceed 2048 pixels. If a wider rectangle is desired, it must
        /// be split into several rectangles and each one should be encoded separately.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#tight-encoding"/>
        Tight = 7,

        /// <summary>
        /// The zlibhex encoding uses zlib [3] to optionally compress subrectangles according to the Hextile Encoding.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#zlibhex-encoding"/>
        ZlibHex = 8,

        /// <summary>
        /// TRLE stands for Tiled Run-Length Encoding, and combines tiling, palettization, and run-length encoding.
        /// </summary>
        /// <seealso href="http://tools.ietf.org/html/rfc6143#page-27"/>
        TRLE = 15,

        /// <summary>
        /// ZRLE stands for Zlib [3] Run-Length Encoding, and combines zlib compression, tiling, palettisation and
        /// run-length encoding. On the wire, the rectangle begins with a 4-byte length field, and is followed by
        /// that many bytes of zlib-compressed data.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#zrle-encoding"/>
        Zrle = 16,

        /// <summary>
        /// JPEG encoding.
        /// </summary>
        Jpeg = 21,

        /// <summary>
        /// A hybrid of lossy and lossless encodings, in particular JPEG and a variation of RLE (Run Length Encoding).
        /// </summary>
        /// <seealso href="http://www.google.com/patents/WO2012080713A1?cl=en"/>
        JRLE = 22,

        /// <summary>
        /// A client which requests the <see cref="Fence"/> pseudo-encoding is declaring that it supports and/or wishes to use
        /// the Fence extension.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#fence-pseudo-encoding"/>
        Fence = -312,

        /// <summary>
        /// A client which requests the <see langword="ContinuousUpdates"/> pseudo-encoding is declaring that it wishes to use the
        /// EnableContinuousUpdates extension.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#continuousupdates-pseudo-encoding"/>
        ContinuousUpdates = -313,

        /// <summary>
        /// A client which requests the <see cref="CursorWithAlpha"/> psuedo-encoding is declaring that it wishes
        /// to use the CursorWithAlpha extension.
        /// </summary>
        CursorWithAlpha = -314,

        /// <summary>
        /// A client which requests the <see cref="PseudoCursor"/> pseudo-encoding is declaring that it is capable of
        /// drawing a mouse cursor locally. This can significantly improve perceived performance
        /// over slow links
        /// </summary>
        PseudoCursor = -239, // TODO: KVM doesn't use this one for me... Find some way to test it...

        /// <summary>
        /// A client which requests the <see cref="PseudoDesktopSize"/> pseudo-encoding is declaring that it is capable
        /// of coping with a change in the framebuffer width and/or height.
        /// </summary>
        PseudoDesktopSize = -223,

        /// <summary>
        /// A client which requests the X Cursor pseudo-encoding is declaring that it is capable
        /// of drawing a mouse cursor locally.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#x-cursor-pseudo-encoding"/>
        XCursor = -240,

        /// <summary>
        /// A client which requests the Cursor pseudo-encoding is declaring that it is capable
        /// of drawing a mouse cursor locally.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#cursor-pseudo-encoding"/>
        RichCursor = -239,

        /// <summary>
        /// A TightVnc-specific extension.
        /// </summary>
        PointerPox = -232,

        /// <summary>
        /// A client which requests the LastRect pseudo-encoding is declaring that it does not need
        /// the exact number of rectangles in a FramebufferUpdate message. Instead, it will stop parsing
        /// when it reaches a LastRect rectangle.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#lastrect-pseudo-encoding"/>
        LastRect = -224,

        /// <summary>
        /// A client which requests the DesktopSize pseudo-encoding is declaring that it is capable of
        /// coping with a change in the framebuffer width and/or height.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#desktopsize-pseudo-encoding"/>
        DesktopSize = -223,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        TightCompressionLevel9 = -247,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel8 = -248,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel7 = -249,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel6 = -250,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel5 = -251,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel4 = -252,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel3 = -252,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel2 = -254,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel1 = -255,

        /// <summary>
        /// Specifies the desired compression level, where level 9 is high compression and level 0
        /// is low compression.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#compression-level-pseudo-encoding"/>
        TightCompressionLevel0 = -256,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel9 = -23,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel8 = -24,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel7 = -25,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel6 = -26,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel5 = -27,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel4 = -28,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel3 = -29,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel2 = -30,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel1 = -31,

        /// <summary>
        /// Specifies the desired quality from the JPEG encoder, where level 9 is high JPEG quality and
        /// level 0 is low JPEG quality.
        /// </summary>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#jpeg-quality-level-pseudo-encoding"/>
        TightQualityLevel0 = -32,

        /// <summary>
        /// VMWare Cursor Data Pseudo-encoding
        /// </summary>
        VMWd = 0x574d5664,

        /// <summary>
        /// VMWare Cursor State Pseudo-encoding
        /// </summary>
        VMWe = 0x574d5665,

        /// <summary>
        /// VMWare Cursor Position Pseudo-encoding
        /// /// </summary>
        VMWf = 0x574d5666,

        /// <summary>
        /// VMWare Keyboard typematic info Pseudo-encoding
        /// </summary>
        VMWg = 0x574d5667,

        /// <summary>
        /// VMWare Keyboard LED state Pseudo-encoding
        /// </summary>
        VMWh = 0x574d5668,

        /// <summary>
        /// VMWare Display Mode Change Pseudo-encoding
        /// </summary>
        VMWi = 0x574d5669,

        /// <summary>
        /// VMWare Virtual Machine Errata State Pseudo-encoding
        /// </summary>
        VMWj = 0x574d566a,
    }
}
