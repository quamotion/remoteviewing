using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;

namespace RemoteViewing.AspNetCore
{
    /// <summary>
    /// A <see cref="Stream"/> which sends and receives data using a <see cref="WebSocket"/>.
    /// </summary>
    public class WebSocketStream : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketStream"/> class.
        /// </summary>
        /// <param name="webSocket">
        /// The <see cref="WebSocket"/> through which to send and receive data.
        /// </param>
        public WebSocketStream(WebSocket webSocket)
        {
            this.WebSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        }

        /// <summary>
        /// Gets the <see cref="WebSocket"/> through which data is sent and received.
        /// </summary>
        public WebSocket WebSocket
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, count);
            var result = this.WebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
            return result.Count;
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, count);
            this.WebSocket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
