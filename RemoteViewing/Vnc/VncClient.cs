#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
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
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RemoteViewing.Utility;

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Connects to a remote VNC server and interacts with it.
    /// </summary>
    public partial class VncClient : IDisposable
    {
        private readonly IVncPasswordChallenge passwordChallenge;
        private VncStream c = new VncStream();
        private VncClientConnectOptions options;
        private double maxUpdateRate;
        private Version serverVersion;
        private Thread threadMain;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncClient"/> class.
        /// </summary>
        public VncClient()
            : this(new VncPasswordChallenge())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VncClient"/> class.
        /// </summary>
        /// <param name="passwordChallenge">
        /// A <see cref="IVncPasswordChallenge"/> which can generate password challenges.
        /// </param>
        public VncClient(IVncPasswordChallenge passwordChallenge)
        {
            if (passwordChallenge == null)
            {
                throw new ArgumentNullException(nameof(passwordChallenge));
            }

            this.passwordChallenge = passwordChallenge;
            this.MaxUpdateRate = 15;
        }

        /// <summary>
        /// Occurs when a bell occurs on the remote server.
        /// </summary>
        public event EventHandler Bell;

        /// <summary>
        /// Occurs when the VNC client has successfully connected to the remote server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Occurs when the VNC client has failed to connect to the server.
        /// </summary>
        public event EventHandler ConnectionFailed;

        /// <summary>
        /// Occurs when the VNC client is disconnected.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when the framebuffer changes.
        /// </summary>
        public event EventHandler<FramebufferChangedEventArgs> FramebufferChanged;

        /// <summary>
        /// Occurs when the clipboard changes on the remote server.
        /// If you are implementing clipboard integration, use this to set the local clipboard.
        /// </summary>
        public event EventHandler<RemoteClipboardChangedEventArgs> RemoteClipboardChanged;

        private enum ResponseType : byte
        {
            FramebufferUpdate = 0,
            SetColorMapEntries = 1,
            Bell = 2,
            ReceiveClipboardData = 3,
            DesktopSize = 200,
        }

        /// <summary>
        /// Gets the framebuffer for the VNC session.
        /// </summary>
        public VncFramebuffer Framebuffer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the client is connected to a server.
        /// </summary>
        /// <value>
        /// <c>true</c> if the client is connected to a server.
        /// </value>
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the max rate to request framebuffer updates at, in frames per second.
        /// </summary>
        /// <remarks>
        /// The default is 15.
        /// </remarks>
        public double MaxUpdateRate
        {
            get
            {
                return this.maxUpdateRate;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "Max update rate must be positive.",
                        (Exception)null);
                }

                this.maxUpdateRate = value;
            }
        }

        /// <summary>
        /// Gets the protocol version of the server.
        /// </summary>
        public Version ServerVersion
        {
            get { return this.serverVersion; }
        }

        /// <summary>
        /// Gets or sets user-specific data.
        /// </summary>
        /// <remarks>
        /// Store anything you want here.
        /// </remarks>
        public object UserData
        {
            get;
            set;
        }

        /// <summary>
        /// Closes the connection with the remote server.
        /// </summary>
        public void Close()
        {
            var thread = this.threadMain;
            this.c.Close();
            thread?.Join();

            if (this.options?.OnDemandMode ?? false)
            {
                this.IsConnected = false;
                this.OnClosed();
            }
        }

        /// <summary>
        /// Connects to a VNC server with the specified hostname and port.
        /// </summary>
        /// <param name="hostname">The name of the host to connect to.</param>
        /// <param name="port">The port to connect on. 5900 is the usual for VNC.</param>
        /// <param name="options">Connection options, if any. You can specify a password here.</param>
        public void Connect(string hostname, int port = 5900, VncClientConnectOptions options = null)
        {
            Throw.If.Null(hostname, "hostname").Negative(port, "port");

            lock (this.c.SyncRoot)
            {
                var client = new TcpClient();

                try
                {
                    client.ConnectAsync(hostname, port).Wait();
                }
                catch (Exception)
                {
                    this.OnConnectionFailed();
                    throw;
                }

                try
                {
                    this.Connect(client.GetStream(), options);
                }
                catch (Exception)
                {
                    ((IDisposable)client).Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// Connects to a VNC server.
        /// </summary>
        /// <param name="stream">The stream containing the connection.</param>
        /// <param name="options">Connection options, if any. You can specify a password here.</param>
        public void Connect(Stream stream, VncClientConnectOptions options = null)
        {
            Throw.If.Null(stream, "stream");

            lock (this.c.SyncRoot)
            {
                this.Close();

                this.options = options ?? new VncClientConnectOptions();
                this.c.Stream = stream;

                try
                {
                    this.NegotiateVersion();
                    this.NegotiateSecurity();
                    this.NegotiateDesktop();
                    this.NegotiateEncodings();
                    this.InitFramebufferDecoder();

                    if (this.options.OnDemandMode)
                    {
                        this.IsConnected = true;
                    }
                    else
                    {
                        this.SendFramebufferUpdateRequest(false);
                        this.threadMain = new Thread(this.ThreadMain);
                        this.threadMain.IsBackground = true;
                        this.threadMain.Start();
                    }
                }
                catch (IOException e)
                {
                    this.OnConnectionFailed();
                    throw new VncException("IO error.", VncFailureReason.NetworkError, e);
                }
                catch (ObjectDisposedException e)
                {
                    this.OnConnectionFailed();
                    throw new VncException("Connection closed.", VncFailureReason.NetworkError, e);
                }
                catch (SocketException e)
                {
                    this.OnConnectionFailed();
                    throw new VncException("Connection failed.", VncFailureReason.NetworkError, e);
                }
            }
        }

        /// <summary>
        /// Notifies the server that the local clipboard has changed.
        /// If you are implementing clipboard integration, use this to set the remote clipboard.
        /// </summary>
        /// <param name="data">The contents of the local clipboard.</param>
        public void SendLocalClipboardChange(string data)
        {
            Throw.If.Null(data, "data");

            var bytes = VncStream.EncodeString(data);

            var p = new byte[8 + bytes.Length];

            p[0] = (byte)6;
            VncUtility.EncodeUInt32BE(p, 4, (uint)bytes.Length);
            Array.Copy(bytes, 0, p, 8, bytes.Length);

            if (this.IsConnected)
            {
                this.c.Send(p);
            }
        }

        /// <summary>
        /// Sends a key event to the VNC server to indicate a key has been pressed or released.
        /// </summary>
        /// <param name="keysym">The X11 keysym of the key. For many keys this is the ASCII value.</param>
        /// <param name="pressed"><c>true</c> for a key press event, or <c>false</c> for a key release event.</param>
        [CLSCompliant(false)]
        public void SendKeyEvent(KeySym keysym, bool pressed)
        {
            var p = new byte[8];

            p[0] = (byte)4;
            p[1] = (byte)(pressed ? 1 : 0);
            VncUtility.EncodeUInt32BE(p, 4, (uint)keysym);

            if (this.IsConnected)
            {
                this.c.Send(p);
            }
        }

        /// <summary>
        /// Sends a pointer event to the VNC server to indicate mouse motion, a button click, etc.
        /// </summary>
        /// <param name="x">The X coordinate of the mouse.</param>
        /// <param name="y">The Y coordinate of the mouse.</param>
        /// <param name="pressedButtons">
        ///     A bit mask of pressed mouse buttons, in X11 convention: 1 is left, 2 is middle, and 4 is right.
        ///     Mouse wheel scrolling is treated as a button event: 8 for up and 16 for down.
        /// </param>
        public void SendPointerEvent(int x, int y, int pressedButtons)
        {
            var p = new byte[6];

            p[0] = (byte)5;
            p[1] = (byte)pressedButtons;
            VncUtility.EncodeUInt16BE(p, 2, (ushort)x);
            VncUtility.EncodeUInt16BE(p, 4, (ushort)y);

            if (this.IsConnected)
            {
                this.c.Send(p);
            }
        }

        /// <summary>
        /// Loads the framebuffer from the specified location and returns a function that can be used to copy the data to a bitmap.
        /// </summary>
        /// <param name="x">The x offset. (0 if null).</param>
        /// <param name="y">The y offset. (0 if null).</param>
        /// <param name="width">The width. (Framebuffer.Width if null).</param>
        /// <param name="height">The height (Framebuffer.Height if null).</param>
        /// <returns>An action with 2 parameters (BitmapData.Scan0 and BitmapData.Stride) that copies the framebuffer data to the specfied address.</returns>
        public Action<IntPtr, int> GetFramebuffer(int? x = null, int? y = null, int? width = null, int? height = null)
        {
            var xValue = x ?? 0;
            var yValue = y ?? 0;
            var widthValue = width ?? this.Framebuffer.Width;
            var heightValue = height ?? this.Framebuffer.Height;

            this.SendFramebufferUpdateRequest(false, xValue, yValue, widthValue, heightValue);

            ResponseType handledResponse;
            do
            {
                handledResponse = this.HandleResponse();
                if (handledResponse == ResponseType.DesktopSize)
                {
                    this.SendFramebufferUpdateRequest(false, xValue, yValue, widthValue, heightValue);
                }
            }
            while (handledResponse != ResponseType.FramebufferUpdate);

            return (scan0, stride) => VncPixelFormat.CopyFromFramebuffer(
                this.Framebuffer, new VncRectangle(xValue, yValue, widthValue, heightValue), scan0, stride, 0, 0);
        }

        /// <summary>
        /// Disposes the client.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Raises the <see cref="Bell"/> event.
        /// </summary>
        protected virtual void OnBell()
        {
            this.Bell?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="Connected"/> event.
        /// </summary>
        protected virtual void OnConnected()
        {
            this.Connected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ConnectionFailed"/> event.
        /// </summary>
        protected virtual void OnConnectionFailed()
        {
            this.ConnectionFailed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="Close"/> event.
        /// </summary>
        protected virtual void OnClosed()
        {
            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="FramebufferChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="FramebufferChangedEventArgs"/> that describes the changes
        /// in the framebuffer.
        /// </param>
        protected virtual void OnFramebufferChanged(FramebufferChangedEventArgs e)
        {
            this.FramebufferChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="RemoteClipboardChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="RemoteClipboardChangedEventArgs"/> that contains information on the
        /// clipboard changes.
        /// </param>
        protected virtual void OnRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            this.RemoteClipboardChanged?.Invoke(this, e);
        }

        // Assumes we are already locked.
        private void CopyToFramebuffer(int tx, int ty, int w, int h, byte[] pixels)
        {
            var fb = this.Framebuffer;
            this.CopyToGeneral(tx, ty, fb.Width, fb.Height, fb.GetBuffer(), 0, 0, w, h, pixels, w, h);
        }

        private void ThreadMain()
        {
            var requester = new Utility.PeriodicThread();
            this.IsConnected = true;
            this.OnConnected();

            try
            {
                requester.Start(
                    () =>
                    {
                        this.SendFramebufferUpdateRequest(true);
                    },
                    () => this.MaxUpdateRate,
                    true);

                while (true)
                {
                    this.HandleResponse(requester);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
            catch (VncException)
            {
            }

            requester.Stop();

            this.c.Stream = null;
            this.IsConnected = false;
            this.OnClosed();
        }

        private ResponseType HandleResponse(PeriodicThread requester = null)
        {
            var command = (ResponseType)this.c.ReceiveByte();

            switch (command)
            {
                case ResponseType.FramebufferUpdate:
                    requester?.Signal();
                    if (!this.HandleFramebufferUpdate())
                    {
                        command = ResponseType.DesktopSize;
                    }

                    break;

                case ResponseType.SetColorMapEntries:
                    this.HandleSetColorMapEntries();
                    break;

                case ResponseType.Bell:
                    this.HandleBell();
                    break;

                case ResponseType.ReceiveClipboardData:
                    this.HandleReceiveClipboardData();
                    break;

                default:
                    VncStream.Require(
                        false,
                        "Unsupported command.",
                        VncFailureReason.UnrecognizedProtocolElement);
                    break;
            }

            return command;
        }

        private void NegotiateVersion()
        {
            this.serverVersion = this.c.ReceiveVersion();
            VncStream.Require(
                this.serverVersion >= new Version(3, 8),
                "RFB 3.8 not supported by server.",
                VncFailureReason.UnsupportedProtocolVersion);

            this.c.SendVersion(new Version(3, 8));
        }

        private void NegotiateSecurity()
        {
            int count = this.c.ReceiveByte();
            if (count == 0)
            {
                string message = this.c.ReceiveString().Trim('\0');
                VncStream.Require(false, message, VncFailureReason.ServerOfferedNoAuthenticationMethods);
            }

            var types = new List<AuthenticationMethod>();
            for (int i = 0; i < count; i++)
            {
                types.Add((AuthenticationMethod)this.c.ReceiveByte());
            }

            if (types.Contains(AuthenticationMethod.None))
            {
                this.c.SendByte((byte)AuthenticationMethod.None);
            }
            else if (types.Contains(AuthenticationMethod.Password))
            {
                if (this.options.Password == null)
                {
                    var callback = this.options.PasswordRequiredCallback;
                    if (callback != null)
                    {
                        this.options.Password = callback(this);
                    }

                    VncStream.Require(
                        this.options.Password != null,
                        "Password required.",
                        VncFailureReason.PasswordRequired);
                }

                this.c.SendByte((byte)AuthenticationMethod.Password);

                var challenge = this.c.Receive(16);
                var password = this.options.Password;
                var response = new byte[16];

                using (new Utility.AutoClear(challenge))
                using (new Utility.AutoClear(response))
                {
                    this.passwordChallenge.GetChallengeResponse(challenge, this.options.Password, response);
                    this.c.Send(response);
                }
            }
            else
            {
                VncStream.Require(
                    false,
                    "No supported authentication methods.",
                    VncFailureReason.NoSupportedAuthenticationMethods);
            }

            uint status = this.c.ReceiveUInt32BE();
            if (status != 0)
            {
                string message = this.c.ReceiveString().Trim('\0');
                VncStream.Require(false, message, VncFailureReason.AuthenticationFailed);
            }
        }

        private void NegotiateDesktop()
        {
            this.c.SendByte((byte)(this.options.ShareDesktop ? 1 : 0));

            var width = this.c.ReceiveUInt16BE();
            VncStream.SanityCheck(width > 0 && width < 0x8000);
            var height = this.c.ReceiveUInt16BE();
            VncStream.SanityCheck(height > 0 && height < 0x8000);

            VncPixelFormat pixelFormat;
            try
            {
                pixelFormat = VncPixelFormat.Decode(this.c.Receive(VncPixelFormat.Size), 0);
            }
            catch (ArgumentException e)
            {
                throw new VncException(
                    "Unsupported pixel format.",
                    VncFailureReason.UnsupportedPixelFormat,
                    e);
            }

            var name = this.c.ReceiveString();
            this.Framebuffer = new VncFramebuffer(name, width, height, pixelFormat);
        }

        private void NegotiateEncodings()
        {
            var encodings = new VncEncoding[]
            {
                VncEncoding.Zlib,
                VncEncoding.Hextile,
                VncEncoding.CopyRect,
                VncEncoding.Raw,
                VncEncoding.PseudoDesktopSize,
            };

            this.c.Send(new[] { (byte)2, (byte)0 });
            this.c.SendUInt16BE((ushort)encodings.Length);
            foreach (var encoding in encodings)
            {
                this.c.SendUInt32BE((uint)encoding);
            }
        }

        private void SendFramebufferUpdateRequest(bool incremental)
        {
            this.SendFramebufferUpdateRequest(incremental, 0, 0, this.Framebuffer.Width, this.Framebuffer.Height);
        }

        private void SendFramebufferUpdateRequest(bool incremental, int x, int y, int width, int height)
        {
            var p = new byte[10];

            p[0] = (byte)3;
            p[1] = (byte)(incremental ? 1 : 0);
            VncUtility.EncodeUInt16BE(p, 2, (ushort)x);
            VncUtility.EncodeUInt16BE(p, 4, (ushort)y);
            VncUtility.EncodeUInt16BE(p, 6, (ushort)width);
            VncUtility.EncodeUInt16BE(p, 8, (ushort)height);

            this.c.Send(p);
        }

        private void CopyToGeneral(
            int tx,
            int ty,
            int tw,
            int th,
            byte[] outPixels,
            int sx,
            int sy,
            int sw,
            int sh,
            byte[] inPixels,
            int w,
            int h)
        {
            int bpp = this.Framebuffer.PixelFormat.BytesPerPixel;

            for (int iy = 0; iy < h; iy++)
            {
                int inOffset = bpp * (((iy + sy) * sw) + sx);
                int outOffset = bpp * (((iy + ty) * tw) + tx);
                Array.Copy(inPixels, inOffset, outPixels, outOffset, w * bpp);
            }
        }

        private void HandleSetColorMapEntries()
        {
            this.c.ReceiveByte(); // padding

            var firstColor = this.c.ReceiveUInt16BE();
            var numColors = this.c.ReceiveUInt16BE();

            for (int i = 0; i < numColors; i++)
            {
                var r = this.c.ReceiveUInt16BE();
                var g = this.c.ReceiveUInt16BE();
                var b = this.c.ReceiveUInt16BE();
            }
        }

        private void HandleBell()
        {
            this.OnBell();
        }

        private void HandleReceiveClipboardData()
        {
            this.c.Receive(3); // padding

            var clipboard = this.c.ReceiveString(0xffffff);

            this.OnRemoteClipboardChanged(new RemoteClipboardChangedEventArgs(clipboard));
        }
    }
}
