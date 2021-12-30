#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2017 Quamotion bvba <http://www.quamotion.mobi/>
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
using RemoteViewing.Vnc.Server;
using System;
using System.Threading.Tasks;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// The middleware which handles noVNC connections.
    /// </summary>
    public class VncMiddleware<T>
        where T : VncServerSession, new()
    {
        private readonly RequestDelegate next;
        private Func<HttpContext, VncContext> vncContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncMiddleware{T}"/> class.
        /// </summary>
        /// <param name="next">
        /// The next request handler. This handler is invoked if the request is not a WebSocket request.
        /// </param>
        /// <param name="vncContextFactory">
        /// A factory which creates the <see cref="VncContext"/> required to handle the request.
        /// </param>
        public VncMiddleware(RequestDelegate next, Func<HttpContext, VncContext> vncContextFactory)
        {
            this.next = next;
            this.vncContextFactory = vncContextFactory;
        }

        /// <summary>
        /// Invokes the middleware for a request.
        /// </summary>
        /// <param name="context">
        /// The current <see cref="HttpContext"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            var vncContext = this.vncContextFactory(context);

            if (vncContext == null)
            {
                return;
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                var protocol = context.WebSockets.WebSocketRequestedProtocols.Count > 0 ? "binary" : null;
                var socket = await context.WebSockets.AcceptWebSocketAsync(protocol).ConfigureAwait(false);

                VncHandler<T> sockethandler = new VncHandler<T>(socket, vncContext);
                await sockethandler.Listen().ConfigureAwait(false);
            }
        }
    }
}
