#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
Copyright (c) 2020 Quamotion bvba <https://github.com/quamotion/remoteviewing>
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

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// A <see cref="IVncServer"/> which uses a purely managed implementation.
    /// </summary>
    public class VncServer : IVncServer
    {
        private readonly IVncFramebufferSource framebufferSource;
        private readonly IVncRemoteKeyboard keyboard;
        private readonly IVncRemoteController controller;
        private readonly ILogger logger;
        private readonly List<IVncServerSession> sessions = new List<IVncServerSession>();
        private TcpListener listener;
        private Task runLoop;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncServer"/> class.
        /// </summary>
        /// <param name="framebufferSource">
        /// The framebuffer source for all sessions which connect to this server.
        /// </param>
        /// <param name="keyboard">
        /// The keyboard for all sessions which connect to this server.
        /// </param>
        /// <param name="controller">
        /// The controller for all sessions which connect to this server.
        /// </param>
        /// <param name="logger">
        /// The logger to use.
        /// </param>
        public VncServer(IVncFramebufferSource framebufferSource, IVncRemoteKeyboard keyboard, IVncRemoteController controller, ILogger<VncServer> logger)
            : this(framebufferSource, keyboard, controller, (ILogger)logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VncServer"/> class.
        /// </summary>
        /// <param name="framebufferSource">
        /// The framebuffer source for all sessions which connect to this server.
        /// </param>
        /// <param name="keyboard">
        /// The keyboard for all sessions which connect to this server.
        /// </param>
        /// <param name="controller">
        /// The controller for all sessions which connect to this server.
        /// </param>
        /// <param name="logger">
        /// The logger to use.
        /// </param>
        public VncServer(IVncFramebufferSource framebufferSource, IVncRemoteKeyboard keyboard, IVncRemoteController controller, ILogger logger)
        {
            this.framebufferSource = framebufferSource ?? throw new ArgumentNullException(nameof(framebufferSource));
            this.keyboard = keyboard;
            this.controller = controller;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public event EventHandler Connected;

        /// <inheritdoc/>
        public event EventHandler Closed;

        /// <inheritdoc/>
        public event EventHandler<PasswordProvidedEventArgs> PasswordProvided;

        /// <inheritdoc/>
        public IReadOnlyList<IVncServerSession> Sessions => this.sessions;

        /// <inheritdoc/>
        public void Start(IPEndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            this.listener = new TcpListener(endPoint);
            this.listener.Start();

            this.runLoop = this.Run();
        }

        /// <inheritdoc/>
        public void StartReverse(IPEndPoint endPoint)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Stop()
        {
            this.Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.listener?.Stop();
            this.listener = null;

            foreach (var session in this.Sessions)
            {
                session.Close();
            }
        }

        /// <summary>
        /// The main loop of the VNC server. Listens for new connections, and spawns new <see cref="VncServerSession"/>s.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous operation.
        /// </returns>
        protected async Task Run()
        {
            while (true)
            {
                var client = await this.listener.AcceptTcpClientAsync();

                // Set up a framebuffer and options.
                var options = new VncServerSessionOptions();
                options.AuthenticationMethod = AuthenticationMethod.Password;

                // Create a session.
                var session = new VncServerSession(
                    new VncPasswordChallenge(),
                    this.logger);

                session.Connected += this.OnConnected;
                session.Closed += this.OnClosed;
                session.PasswordProvided += this.OnPasswordProvided;
                session.SetFramebufferSource(this.framebufferSource);
                session.Connect(client.GetStream(), options);

                if (this.keyboard != null)
                {
                    session.KeyChanged += this.keyboard.HandleKeyEvent;
                }

                if (this.controller != null)
                {
                    session.PointerChanged += this.controller.HandleTouchEvent;
                }

                this.sessions.Add(session);
            }
        }

        /// <summary>
        /// Raises the <see cref="Connected"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnConnected(object sender, EventArgs e)
        {
            this.Connected?.Invoke(sender, e);
        }

        /// <summary>
        /// Raises the <see cref="Closed"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnClosed(object sender, EventArgs e)
        {
            this.Closed?.Invoke(sender, e);
        }

        /// <summary>
        /// Raises the <see cref="PasswordProvided"/> event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnPasswordProvided(object sender, PasswordProvidedEventArgs e)
        {
            this.PasswordProvided?.Invoke(sender, e);
        }
    }
}
