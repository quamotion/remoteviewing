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

        /// <summary>
        /// Adds the VNC video middleware, which allows the web server to stream video recordings
        /// of a framebuffer source to a client.
        /// </summary>
        /// <param name="app">
        /// The app for which to enable the VNC video middleware.
        /// </param>
        /// <param name="path">
        /// The path at which to listen for VNC video requests.
        /// </param>
        /// <param name="vncContextFactory">
        /// A function which returns the VNC context.
        /// </param>
        /// <returns>
        /// The <paramref name="app"/>.
        /// </returns>
        public static IApplicationBuilder UseVncVideoRecording(
            this IApplicationBuilder app,
            PathString path,
            Func<HttpContext, VncContext> vncContextFactory)
        {
            return app
                .Map(path, a => a.UseMiddleware<VncVideoMiddleware>(vncContextFactory));
        }
    }
}
