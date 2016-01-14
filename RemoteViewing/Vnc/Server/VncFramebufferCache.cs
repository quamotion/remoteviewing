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
using System.Linq;
using System.Security.Cryptography;

namespace RemoteViewing.Vnc.Server
{
    sealed class VncFramebufferCache
    {
        const int TileSize = 32;

        SHA1Managed _hash = new SHA1Managed();
        byte[,][] _hashes; // [y,x][hash]
        byte[] _pixelBuffer;

        public VncFramebufferCache(VncFramebuffer framebuffer)
        {
            Throw.If.Null(framebuffer, "framebuffer");
            Framebuffer = framebuffer;

            _hashes = new byte[(framebuffer.Height + TileSize - 1) / TileSize,
                               (framebuffer.Width  + TileSize - 1) / TileSize][];
            _pixelBuffer = new byte[TileSize * TileSize * Framebuffer.PixelFormat.BytesPerPixel];
        }

        public bool RespondToUpdateRequest(VncServerSession session)
        {
            var fb = Framebuffer; var fbr = session.FramebufferUpdateRequest;
            if (fb == null || fbr == null) { return false; }

            var incremental = fbr.Incremental; var region = fbr.Region;
            session.FramebufferManualBeginUpdate();

            var buffer = fb.GetBuffer();
            int bpp = fb.PixelFormat.BytesPerPixel;
            lock (fb.SyncRoot)
            {
                int ymax = Math.Min(region.Y + region.Height, fb.Height);
                int xmax = Math.Min(region.X + region.Width, fb.Width);

                for (int y = region.Y; y < ymax; y += TileSize)
                {
                    for (int x = region.X; x < xmax; x += TileSize)
                    {
                        int w = Math.Min(TileSize, xmax - x);
                        int h = Math.Min(TileSize, ymax - y);

                        var subregion = new VncRectangle(x, y, w, h);

                        VncPixelFormat.Copy(buffer, fb.Stride, fb.PixelFormat, subregion,
                                            _pixelBuffer, w * bpp, Framebuffer.PixelFormat);

                        int ix = x / TileSize, iy = y / TileSize;
                        var tileHash = _hash.ComputeHash(_pixelBuffer, 0, w * h * bpp);

                        if (_hashes[iy, ix] == null || !_hashes[iy, ix].SequenceEqual(tileHash))
                        {
                            _hashes[iy, ix] = tileHash;
                            if (incremental) { session.FramebufferManualInvalidate(subregion); }
                        }
                    }
                }
            }

            if (!incremental)
            {
                session.FramebufferManualInvalidate(region);
            }

            return session.FramebufferManualEndUpdate();
        }

        public VncFramebuffer Framebuffer
        {
            get;
            private set;
        }
    }
}
