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
            var result = this.WebSocket.ReceiveAsync(segment, CancellationToken.None).GetAwaiter().GetResult();
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
