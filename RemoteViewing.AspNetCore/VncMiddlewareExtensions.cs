using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RemoteViewing.Vnc.Server;
using System;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// Extension methods which make using the noVNC middleware easier.
    /// </summary>
    public static class VncMiddlewareExtensions
    {
        /// <summary>
        /// Adds the noVNC middleware which allows the web server to accept noVNC connections.
        /// </summary>
        /// <param name="app">
        /// The app for which to enable the noVNC middelware.
        /// </param>
        /// <param name="path">
        /// The path at which to listen for noVNC connections.
        /// </param>
        /// <param name="vncContextFactory">
        /// A function which returns the VNC context.
        /// </param>
        /// <returns>
        /// The <paramref name="app"/>.
        /// </returns>
        public static IApplicationBuilder UseVncServer(
            this IApplicationBuilder app,
            PathString path,
            Func<HttpContext, VncContext> vncContextFactory)
        {
            return app
                .UseWebSockets()
                .Map(path, a => a.UseMiddleware<VncMiddleware<VncServerSession>>(vncContextFactory));
        }
    }
}
