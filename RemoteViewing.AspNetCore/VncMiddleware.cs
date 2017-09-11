using Microsoft.AspNetCore.Http;
using RemoteViewing.Vnc.Server;
using System;
using System.Threading.Tasks;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// The middleware which handles noVNC connections.
    /// </summary>
    public class VncMiddleware<T> where T : VncServerSession, new()
    {
        private readonly RequestDelegate next;
        private Func<HttpContext, VncContext> vncContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncMiddleware"/> class.
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
                var socket = await context.WebSockets.AcceptWebSocketAsync("binary").ConfigureAwait(false);

                VncHandler<T> sockethandler = new VncHandler<T>(socket, vncContext);
                await sockethandler.Listen().ConfigureAwait(false);
            }
        }
    }
}
