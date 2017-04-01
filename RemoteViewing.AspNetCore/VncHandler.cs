using Microsoft.AspNetCore.Http;
using Nito.AsyncEx;
using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// Handles noVNC connections.
    /// </summary>
    public class VncHandler
    {
        /// <summary>
        /// This event will be reset when the conection with the client is lost, either because the client logged
        /// of or because of an error.
        /// </summary>
        private readonly AsyncManualResetEvent closed = new AsyncManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="VncHandler"/> class.
        /// </summary>
        /// <param name="socket">
        ///  The socket to which the notifications should be send.
        /// </param>
        /// <param name="vncContext">
        /// The <see cref="VncContext"/> which defines the VNC data to transmit to the client.
        /// </param>
        public VncHandler(WebSocket socket, VncContext vncContext)
        {
            if (vncContext == null)
            {
                throw new ArgumentNullException(nameof(vncContext));
            }

            if (vncContext.FramebufferSource == null || vncContext.Password == null)
            {
                throw new ArgumentOutOfRangeException(nameof(vncContext));
            }

            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            this.VncContext = vncContext;
            this.Socket = socket;
            this.Vnc = new VncServerSession();

            this.VncContext.SessionConfiguration?.Invoke(this.Vnc);

            this.Vnc.Connected += this.OnConnected;
            this.Vnc.ConnectionFailed += this.OnConnectionFailed;
            this.Vnc.Closed += this.OnClosed;
            this.Vnc.PasswordProvided += this.OnPasswordProvided;
            this.Vnc.SetFramebufferSource(vncContext.FramebufferSource);
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
        public VncServerSession Vnc
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the socket to which the notifications should be send.
        /// </summary>
        public WebSocket Socket
        {
            get;
            private set;
        }

        /// <summary>
        /// Listen to the given <see cref="HttpContext"/>
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> corresponding to the listen process.
        /// </returns>
        public async Task Listen()
        {
            var stream = new WebSocketStream(this.Socket);

            var options = new VncServerSessionOptions();
            options.AuthenticationMethod = AuthenticationMethod.Password;

            this.Vnc.Connect(stream, options);

            await this.closed.WaitAsync().ConfigureAwait(false);

            this.Socket.Dispose();
        }

        /// <summary>
        /// Handles a new client connection.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnConnected(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles a client which failed to connect.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnConnectionFailed(object sender, EventArgs e)
        {
            this.closed.Set();
        }

        /// <summary>
        /// Handles a client which disconnected.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnClosed(object sender, EventArgs e)
        {
            this.closed.Set();
        }

        /// <param name="sender">
        /// Handles password authentication.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnPasswordProvided(object sender, PasswordProvidedEventArgs e)
        {
            e.Accept(this.VncContext.Password.ToCharArray());
        }
    }
}
