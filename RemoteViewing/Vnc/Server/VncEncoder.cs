#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
Copyright (c) 2020 Quamotion bvba <http://quamotion.mobi>
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

using System.IO;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// A common base class for VNC encoders.
    /// </summary>
    public abstract class VncEncoder
    {
        /// <summary>
        /// Gets the <see cref="VncEncoder"/> protocol implemented by this <see cref="VncEncoder"/>.
        /// </summary>
        public abstract VncEncoding Encoding { get; }

        /// <summary>
        /// Sends the contents of a rectangle to the client.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> which represents the connection to the VNC client.
        /// </param>
        /// <param name="pixelFormat">
        /// The <see cref="VncPixelFormat"/> being used.
        /// </param>
        /// <param name="region">
        /// The dimesions of the rectangle.
        /// </param>
        /// <param name="contents">
        /// The contents of the rectangle, in raw pixel format.
        /// </param>
        /// <returns>
        /// The total number of bytes written to the wire. Used for bookkeeping.
        /// </returns>
        public abstract int Send(Stream stream, VncPixelFormat pixelFormat, VncRectangle region, byte[] contents);
    }
}
