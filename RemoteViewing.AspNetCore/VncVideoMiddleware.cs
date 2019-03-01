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
using System;
using System.Threading.Tasks;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// Middleware for recording VNC videos.
    /// </summary>
    public class VncVideoMiddleware
    {
        private readonly RequestDelegate next;
        private Func<HttpContext, VncContext> vncContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncVideoMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next request handler. This handler is invoked if the request is not a GET request.
        /// </param>
        /// <param name="vncContextFactory">
        /// A factory which creates the <see cref="VncContext"/> required to handle the request.
        /// </param>
        public VncVideoMiddleware(RequestDelegate next, Func<HttpContext, VncContext> vncContextFactory)
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
        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Method != "GET")
            {
                return this.next.Invoke(context);
            }

            var vncContext = this.vncContextFactory(context);

            if (vncContext == null)
            {
                return this.next.Invoke(context);
            }

            VncVideoHandler videoHandler = new VncVideoHandler(context.Response.Body, vncContext);
            return videoHandler.Listen(context.RequestAborted);
        }
    }
}
