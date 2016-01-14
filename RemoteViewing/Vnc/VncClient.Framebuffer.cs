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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace RemoteViewing.Vnc
{
    partial class VncClient
    {
        byte[] _framebufferScratch = new byte[0];
        byte[] _zlibScratch = new byte[0];
        Stream _zlibMemoryStream;
        DeflateStream _zlibInflater;

        void InitFramebufferDecoder()
        {
            _zlibMemoryStream = new MemoryStream();
            _zlibInflater = null; // Don't reuse the dictionary between sessions.
        }

        byte[] AllocateFramebufferScratch(int bytes)
        {
            return VncUtility.AllocateScratch(bytes, ref _framebufferScratch);
        }

        void HandleFramebufferUpdate()
        {
            _c.ReceiveByte(); // padding

            var numRects = _c.ReceiveUInt16BE();
            var rects = new List<VncRectangle>();

            for (int i = 0; i < numRects; i++)
            {
                var r = _c.ReceiveRectangle();
                int x = r.X, y = r.Y, w = r.Width, h = r.Height;
                VncStream.SanityCheck(w > 0 && w < 0x8000);
                VncStream.SanityCheck(h > 0 && h < 0x8000);

                int fbW = Framebuffer.Width, fbH = Framebuffer.Height, bpp = Framebuffer.PixelFormat.BytesPerPixel;
                var inRange = w <= fbW && h <= fbH && x <= fbW - w && y <= fbH - h; byte[] pixels;

                var encoding = (VncEncoding)_c.ReceiveUInt32BE();
                switch (encoding)
                {
                    case VncEncoding.Hextile: // KVM seems to avoid this now that I support Zlib.
                        var background = new byte[bpp];
                        var foreground = new byte[bpp];

                        for (int ty = 0; ty < h; ty += 16)
                        {
                            int th = Math.Min(16, h - ty);
                            for (int tx = 0; tx < w; tx += 16)
                            {
                                int tw = Math.Min(16, w - tx);

                                var subencoding = _c.ReceiveByte();
                                pixels = AllocateFramebufferScratch(tw * th * bpp);

                                if (0 != (subencoding & 1)) // raw
                                {
                                    _c.Receive(pixels, 0, tw * th * bpp);

                                    if (inRange)
                                    {
                                        lock (Framebuffer.SyncRoot) { CopyToFramebuffer(x + tx, y + ty, tw, th, pixels); }
                                    }
                                }
                                else
                                {
                                    pixels = AllocateFramebufferScratch(tw * th * bpp);
                                    if (0 != (subencoding & 2)) { background = _c.Receive(bpp); }
                                    if (0 != (subencoding & 4)) { foreground = _c.Receive(bpp); }

                                    int ptr = 0;
                                    for (int pp = 0; pp < tw * th; pp++)
                                    {
                                        for (int pe = 0; pe < bpp; pe++) { pixels[ptr++] = background[pe]; }
                                    }

                                    int nsubrects = 0 != (subencoding & 8) ? _c.ReceiveByte() : 0;
                                    if (nsubrects > 0)
                                    {
                                        var subrectsColored = 0 != (subencoding & 16);
                                        for (int subrect = 0; subrect < nsubrects; subrect++)
                                        {
                                            var color = subrectsColored ? _c.Receive(bpp) : foreground;
                                            var srxy = _c.ReceiveByte(); var srwh = _c.ReceiveByte();
                                            int srx = (srxy >> 4) & 0xf, srw = ((srwh >> 4) & 0xf) + 1;
                                            int sry = (srxy >> 0) & 0xf, srh = ((srwh >> 0) & 0xf) + 1;
                                            if (srx + srw > tw || sry + srh > th) { continue; }

                                            for (int py = 0; py < srh; py++)
                                            {
                                                for (int px = 0; px < srw; px++)
                                                {
                                                    int off = bpp * ((py + sry) * tw + (px + srx));
                                                    for (int pe = 0; pe < bpp; pe++)
                                                    {
                                                        pixels[off + pe] = color[pe];
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    lock (Framebuffer.SyncRoot) { CopyToFramebuffer(x + tx, y + ty, tw, th, pixels); }
                                }
                            }
                        }
                        break;

                    case VncEncoding.CopyRect:
                        var srcx = (int)_c.ReceiveUInt16BE();
                        var srcy = (int)_c.ReceiveUInt16BE();
                        if (srcx + w > fbW) { w = fbW - srcx; }
                        if (srcy + h > fbH) { h = fbH - srcy; }
                        if (w < 1 || h < 1) { continue; }

                        pixels = AllocateFramebufferScratch(w * h * bpp);
                        lock (Framebuffer.SyncRoot)
                        {
                            CopyToGeneral(0, 0, w, h, pixels, srcx, srcy, fbW, fbH, Framebuffer.GetBuffer(), w, h);
                            CopyToFramebuffer(x, y, w, h, pixels);
                        }
                        break;

                    case VncEncoding.Raw:
                        pixels = AllocateFramebufferScratch(w * h * bpp);
                        _c.Receive(pixels, 0, w * h * bpp);

                        if (inRange)
                        {
                            lock (Framebuffer.SyncRoot) { CopyToFramebuffer(x, y, w, h, pixels); }
                        }
                        break;

                    case VncEncoding.Zlib:
                        int bytesDesired = w * h * bpp;

                        int size = (int)_c.ReceiveUInt32BE(); VncStream.SanityCheck(size >= 0 && size < 0x10000000);
                        VncUtility.AllocateScratch(size, ref _zlibScratch);
                        _c.Receive(_zlibScratch, 0, size);

                        _zlibMemoryStream.Position = 0;
                        _zlibMemoryStream.Write(_zlibScratch, 0, size);
                        _zlibMemoryStream.SetLength(size);
                        _zlibMemoryStream.Position = 0;

                        if (_zlibInflater == null) // Zlib has a two-byte header.
                        {
                            VncStream.SanityCheck(size >= 2); _zlibMemoryStream.Position = 2;
                            _zlibInflater = new DeflateStream(_zlibMemoryStream, CompressionMode.Decompress, false);
                        }

                        pixels = AllocateFramebufferScratch(bytesDesired);
                        for (int j = 0; j < bytesDesired; )
                        {
                            int count = 0;

                            try
                            {
                                count = _zlibInflater.Read(pixels, j, bytesDesired - j);
                            }
                            catch (InvalidDataException)
                            {
                                VncStream.Require(false,
                                                      "Bad data compressed.",
                                                      VncFailureReason.UnrecognizedProtocolElement);
                            }

                            VncStream.Require(count > 0,
                                                  "No data compressed.",
                                                  VncFailureReason.UnrecognizedProtocolElement);
                            j += count;
                        }

                        if (inRange)
                        {
                            lock (Framebuffer.SyncRoot) { CopyToFramebuffer(x, y, w, h, pixels); }
                        }
                        break;

                    case VncEncoding.PseudoDesktopSize:
                        Framebuffer = new VncFramebuffer(Framebuffer.Name, w, h, Framebuffer.PixelFormat);
                        continue; // Don't call OnFramebufferChanged for this one.

                    default:
                        VncStream.Require(false,
                                              "Unsupported encoding.",
                                              VncFailureReason.UnrecognizedProtocolElement);
                        break;
                }

                rects.Add(new VncRectangle(x, y, w, h));
            }

            if (rects.Count > 0)
            {
                OnFramebufferChanged(new FramebufferChangedEventArgs(rects));
            }
        }
    }
}
