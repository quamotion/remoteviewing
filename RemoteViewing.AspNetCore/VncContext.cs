using RemoteViewing.Logging;
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
        /// Gets or sets the <see cref="IVncRemoteController"/> which allows you to remotely control the VNC server.
        /// </summary>
        public IVncRemoteController RemoteController
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

        /// <summary>
        /// Gets or sets the logger to use when logging diagnostic messages. No logging will happen when
        /// set to <see langword="null"/>.
        /// </summary>
        public ILog Logger
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IVncPasswordChallenge"/> to use when creating or validating VNC passwords.
        /// The default <see cref="VncPasswordChallenge"/> will be used when set to <see langword="null"/>.
        /// </summary>
        public IVncPasswordChallenge PasswordChallenge
        {
            get;
            set;
        }
    }
}
