﻿using Microsoft.Extensions.Hosting;
using RemoteViewing.Vnc.Server;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteViewing.Hosting
{
    /// <summary>
    /// A <see cref="IHostedService"/> which starts a VNC server.
    /// </summary>
    internal class VncServerWorker : IHostedService
    {
        private readonly VncServerOptions options;
        private IVncServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncServerWorker"/> class.
        /// </summary>
        /// <param name="server">
        /// The <see cref="IVncServer"/> around which the <see cref="VncServerWorker"/> wraps.
        /// </param>
        /// <param name="options">
        /// Options which control how the service should be started.
        /// </param>
        public VncServerWorker(IVncServer server, VncServerOptions options)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var endPoint =
                new IPEndPoint(
                    IPAddress.Parse(this.options.Address),
                    this.options.Port);

            if (!this.options.Reverse)
            {
                await this.server.StartAsync(endPoint, cancellationToken);
            }
            else
            {
                await this.server.StartReverseAsync(endPoint, cancellationToken);
            }

            if (this.options.Password != null)
            {
                this.server.PasswordProvided += this.HandlePasswordProvided;
            }
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.server.StopAsync(cancellationToken);
        }

        private void HandlePasswordProvided(object sender, PasswordProvidedEventArgs e)
        {
            e.Accept(this.options.Password.ToCharArray());
        }
    }
}
