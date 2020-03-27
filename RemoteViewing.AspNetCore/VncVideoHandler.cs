#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2018 Quamotion bvba <http://www.quamotion.mobi/>
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

using Microsoft.AspNetCore.Http;
using RemoteViewing.VMware;
using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// Middleware which handles requests to return video recordings of VNC feeds.
    /// </summary>
    public class VncVideoHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VncVideoHandler"/> class.
        /// </summary>
        /// <param name="stream">
        ///  The <see cref="Stream"/> to which to write the video recording.
        /// </param>
        /// <param name="vncContext">
        /// The <see cref="VncContext"/> which defines the VNC data to transmit to the client.
        /// </param>
        public VncVideoHandler(Stream stream, VncContext vncContext)
        {
            if (vncContext == null)
            {
                throw new ArgumentNullException(nameof(vncContext));
            }

            if (vncContext.FramebufferSource == null)
            {
                throw new ArgumentOutOfRangeException(nameof(vncContext));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            this.FramebufferSource = vncContext.FramebufferSource;
            this.Stream = stream;
        }

        /// <summary>
        /// Gets the <see cref="VncContext"/> which defines which VNC data is shown to the client.
        /// </summary>
        public VncContext VncContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current <see cref="VncServerSession"/>.
        /// </summary>
        public IVncFramebufferSource FramebufferSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="Stream"/> to which to write the video recording.
        /// </summary>
        public Stream Stream
        {
            get;
            private set;
        }

        /// <summary>
        /// Listen to the given <see cref="HttpContext"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> corresponding to the listen process.
        /// </returns>
        public Task Listen(CancellationToken cancellation)
        {
            VncAviWriter writer = new VncAviWriter(this.FramebufferSource);
            return writer.WriteAsync(this.Stream, cancellation);
        }
    }
}
