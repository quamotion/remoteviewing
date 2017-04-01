using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// Contains all the input required to host a noVNC endpoint.
    /// </summary>
    public class VncContext
    {
        /// <summary>
        /// Gets or sets the <see cref="IVncFramebufferSource"/> which provides the framebuffer data.
        /// </summary>
        public IVncFramebufferSource FramebufferSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password required to connect to the noVNC endpoint.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an optional delegate which configures a <see cref="VncServerSession"/> after the session
        /// has been created.
        /// </summary>
        public Action<VncServerSession> SessionConfiguration
        {
            get;
            set;
        }
    }
}
