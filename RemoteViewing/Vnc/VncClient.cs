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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Connects to a remote VNC server and interacts with it.
    /// </summary>
    public partial class VncClient
    {
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

        VncStream _c = new VncStream();
        VncClientConnectOptions _options;
        double _maxUpdateRate;
        Version _serverVersion = new Version();
        Thread _threadMain;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncClient"/> class.
        /// </summary>
        public VncClient()
        {
            MaxUpdateRate = 15;
        }

        /// <summary>
        /// Closes the connection with the remote server.
        /// </summary>
        public void Close()
        {
            var thread = _threadMain; _c.Close();
            if (thread != null) { thread.Join(); }
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

            lock (_c.SyncRoot)
            {
                var client = new TcpClient();

                try
                {
                    client.Connect(hostname, port);
                }
                catch (Exception)
                {
                    OnConnectionFailed(); throw;
                }

                try
                {
                    Connect(client.GetStream(), options);
                }
                catch (Exception)
                {
                    client.Close(); throw;
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

            lock (_c.SyncRoot)
            {
                Close();

                _options = options ?? new VncClientConnectOptions();
                _c.Stream = stream;

                try
                {
                    NegotiateVersion();
                    NegotiateSecurity();
                    NegotiateDesktop();
                    NegotiateEncodings();
                    InitFramebufferDecoder();
                    SendFramebufferUpdateRequest(false);
                }
                catch (IOException e)
                {
                    OnConnectionFailed();
                    throw new VncException("IO error.", VncFailureReason.NetworkError, e);
                }
                catch (ObjectDisposedException e)
                {
                    OnConnectionFailed();
                    throw new VncException("Connection closed.", VncFailureReason.NetworkError, e);
                }
                catch (SocketException e)
                {
                    OnConnectionFailed();
                    throw new VncException("Connection failed.", VncFailureReason.NetworkError, e);
                }

                _threadMain = new Thread(ThreadMain);
                _threadMain.IsBackground = true;
                _threadMain.Start();
            }
        }

        void ThreadMain()
        {
            var requester = new Utility.PeriodicThread();
            IsConnected = true; OnConnected();

            try
            {
                requester.Start(() =>
                    {
                        SendFramebufferUpdateRequest(true);
                        return true;
                    }, () => MaxUpdateRate, true);

                while (true)
                {
                    var command = _c.ReceiveByte();

                    switch (command)
                    {
                        case 0:
                            requester.Signal();
                            HandleFramebufferUpdate();
                            break;

                        case 1:
                            HandleSetColorMapEntries();
                            break;

                        case 2:
                            HandleBell();
                            break;

                        case 3:
                            HandleReceiveClipboardData();
                            break;

                        default:
                            VncStream.Require(false, "Unsupported command.",
                                VncFailureReason.UnrecognizedProtocolElement);
                            break;
                    }
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

            _c.Stream = null;
            IsConnected = false; OnClosed();
        }

        void NegotiateVersion()
        {
            _serverVersion = _c.ReceiveVersion();
            VncStream.Require(_serverVersion >= new Version(3, 8),
                                  "RFB 3.8 not supported by server.",
                                  VncFailureReason.UnsupportedProtocolVersion);

            _c.SendVersion(new Version(3, 8));
        }

        void NegotiateSecurity()
        {
            int count = _c.ReceiveByte();
            if (count == 0)
            {
                string message = _c.ReceiveString().Trim('\0');
                VncStream.Require(false, message, VncFailureReason.ServerOfferedNoAuthenticationMethods);
            }

            var types = new List<AuthenticationMethod>();
            for (int i = 0; i < count; i++) { types.Add((AuthenticationMethod)_c.ReceiveByte()); }

            if (types.Contains(AuthenticationMethod.None))
            {
                _c.SendByte((byte)AuthenticationMethod.None);
            }
            else if (types.Contains(AuthenticationMethod.Password))
            {
                if (_options.Password == null)
                {
                    var callback = _options.PasswordRequiredCallback;
                    if (callback != null) { _options.Password = callback(this); }

                    VncStream.Require(_options.Password != null,
                                          "Password required.",
                                          VncFailureReason.PasswordRequired);
                }

                _c.SendByte((byte)AuthenticationMethod.Password);

                var challenge = _c.Receive(16);
                var password = _options.Password;
                var response = new byte[16];

                using (new Utility.AutoClear(challenge))
                using (new Utility.AutoClear(response))
                {
                    VncPasswordChallenge.GetChallengeResponse(challenge, _options.Password, response);
                    _c.Send(response);
                }
            }
            else
            {
                VncStream.Require(false,
                                      "No supported authentication methods.",
                                      VncFailureReason.NoSupportedAuthenticationMethods);
            }

            uint status = _c.ReceiveUInt32BE();
            if (status != 0)
            {
                string message = _c.ReceiveString().Trim('\0');
                VncStream.Require(false, message, VncFailureReason.AuthenticationFailed);
            }
        }

        void NegotiateDesktop()
        {
            _c.SendByte((byte)(_options.ShareDesktop ? 1 : 0));

            var width = _c.ReceiveUInt16BE(); VncStream.SanityCheck(width > 0 && width < 0x8000);
            var height = _c.ReceiveUInt16BE(); VncStream.SanityCheck(height > 0 && height < 0x8000);

            VncPixelFormat pixelFormat;
            try
            {
                pixelFormat = VncPixelFormat.Decode(_c.Receive(VncPixelFormat.Size), 0);
            }
            catch (ArgumentException e)
            {
                throw new VncException("Unsupported pixel format.",
                                               VncFailureReason.UnsupportedPixelFormat, e);
            }

            var name = _c.ReceiveString();
            Framebuffer = new VncFramebuffer(name, width, height, pixelFormat);
        }

        void NegotiateEncodings()
        {
            var encodings = new VncEncoding[]
            {
                VncEncoding.Zlib,
                VncEncoding.Hextile,
                VncEncoding.CopyRect,
                VncEncoding.Raw,
                VncEncoding.PseudoDesktopSize
            };

            _c.Send(new[] { (byte)2, (byte)0 });
            _c.SendUInt16BE((ushort)encodings.Length);
            foreach (var encoding in encodings) { _c.SendUInt32BE((uint)encoding); }
        }

        void SendFramebufferUpdateRequest(bool incremental)
        {
            var p = new byte[10];

            p[0] = (byte)3; p[1] = (byte)(incremental ? 1 : 0);
            VncUtility.EncodeUInt16BE(p, 2, (ushort)0);
            VncUtility.EncodeUInt16BE(p, 4, (ushort)0);
            VncUtility.EncodeUInt16BE(p, 6, (ushort)Framebuffer.Width);
            VncUtility.EncodeUInt16BE(p, 8, (ushort)Framebuffer.Height);

            _c.Send(p);
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

            if (IsConnected) { _c.Send(p); }
        }

        /// <summary>
        /// Sends a key event to the VNC server to indicate a key has been pressed or released.
        /// </summary>
        /// <param name="keysym">The X11 keysym of the key. For many keys this is the ASCII value.</param>
        /// <param name="pressed"><c>true</c> for a key press event, or <c>false</c> for a key release event.</param>
        public void SendKeyEvent(int keysym, bool pressed)
        {
            var p = new byte[8];

            p[0] = (byte)4; p[1] = (byte)(pressed ? 1 : 0);
            VncUtility.EncodeUInt32BE(p, 4, (uint)keysym);

            if (IsConnected) { _c.Send(p); }
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

            p[0] = (byte)5; p[1] = (byte)pressedButtons;
            VncUtility.EncodeUInt16BE(p, 2, (ushort)x);
            VncUtility.EncodeUInt16BE(p, 4, (ushort)y);

            if (IsConnected) { _c.Send(p); }
        }

        // Assumes we are already locked.
        void CopyToFramebuffer(int tx, int ty, int w, int h, byte[] pixels)
        {
            var fb = Framebuffer;
            CopyToGeneral(tx, ty, fb.Width, fb.Height, fb.GetBuffer(), 0, 0, w, h, pixels, w, h);
        }

        void CopyToGeneral(int tx, int ty, int tw, int th, byte[] outPixels,
                           int sx, int sy, int sw, int sh, byte[] inPixels,
                           int w, int h)
        {
            int bpp = Framebuffer.PixelFormat.BytesPerPixel;

            for (int iy = 0; iy < h; iy++)
            {
                int inOffset = bpp * ((iy + sy) * sw + sx);
                int outOffset = bpp * ((iy + ty) * tw + tx);
                Array.Copy(inPixels, inOffset, outPixels, outOffset, w * bpp);
            }
        }

        void HandleSetColorMapEntries()
        {
            _c.ReceiveByte(); // padding

            var firstColor = _c.ReceiveUInt16BE();
            var numColors = _c.ReceiveUInt16BE();

            for (int i = 0; i < numColors; i++)
            {
                var r = _c.ReceiveUInt16BE();
                var g = _c.ReceiveUInt16BE();
                var b = _c.ReceiveUInt16BE();
            }
        }

        void HandleBell()
        {
            OnBell();
        }

        void HandleReceiveClipboardData()
        {
            _c.Receive(3); // padding

            var clipboard = _c.ReceiveString(0xffffff);

            OnRemoteClipboardChanged(new RemoteClipboardChangedEventArgs(clipboard));
        }

        protected virtual void OnBell()
        {
            RaiseBell();
        }

        protected void RaiseBell()
        {
            var ev = Bell;
            if (ev != null) { ev(this, EventArgs.Empty); }
        }

        protected virtual void OnConnected()
        {
            RaiseConnected();
        }

        protected void RaiseConnected()
        {
            var ev = Connected;
            if (ev != null) { ev(this, EventArgs.Empty); }
        }

        protected virtual void OnConnectionFailed()
        {
            RaiseConnectionFailed();
        }

        protected void RaiseConnectionFailed()
        {
            var ev = ConnectionFailed;
            if (ev != null) { ev(this, EventArgs.Empty); }
        }

        protected virtual void OnClosed()
        {
            RaiseClosed();
        }

        protected void RaiseClosed()
        {
            var ev = Closed;
            if (ev != null) { ev(this, EventArgs.Empty); }
        }

        protected virtual void OnFramebufferChanged(FramebufferChangedEventArgs e)
        {
            RaiseFramebufferChanged(e);
        }

        protected void RaiseFramebufferChanged(FramebufferChangedEventArgs e)
        {
            var ev = FramebufferChanged;
            if (ev != null) { ev(this, e); }
        }

        protected virtual void OnRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            RaiseRemoteClipboardChanged(e);
        }

        protected void RaiseRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            var ev = RemoteClipboardChanged;
            if (ev != null) { ev(this, e); }
        }

        /// <summary>
        /// The framebuffer for the VNC session.
        /// </summary>
        public VncFramebuffer Framebuffer
        {
            get;
            private set;
        }

        /// <summary>
        /// <c>true</c> if the client is connected to a server.
        /// </summary>
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// The max rate to request framebuffer updates at, in frames per second.
        /// 
        /// The default is 15.
        /// </summary>
        public double MaxUpdateRate
        {
            get { return _maxUpdateRate; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Max update rate must be positive.",
                                                          (Exception)null);
                }

                _maxUpdateRate = value;
            }
        }

        /// <summary>
        /// The protocol version of the server.
        /// </summary>
        public Version ServerVersion
        {
            get { return _serverVersion; }
        }

        /// <summary>
        /// Store anything you want here.
        /// </summary>
        public object UserData
        {
            get;
            set;
        }
    }
}
