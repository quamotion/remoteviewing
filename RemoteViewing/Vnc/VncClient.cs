#region License
/*
RemoteViewing VNC Client Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com>
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
using System.Text;
using System.Text.RegularExpressions;
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
        /// Occurs during authentication if a password is required and was not supplied in
        /// the connection options.
        /// </summary>
        public event EventHandler<PasswordRequiredEventArgs> PasswordRequired;

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

        VncClientConnectOptions _options;
        double _maxUpdateRate;
        Stream _stream;
        object _specialSync = new object();
        Thread _threadMain;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncClient"/> class.
        /// </summary>
        public VncClient()
        {
            MaxUpdateRate = 15;
            SynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Closes the connection with the remote server.
        /// </summary>
        public void Close()
        {
            var thread = _threadMain; var stream = _stream;
            if (stream != null) { stream.Close(); }
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

            Close();

            lock (_specialSync)
            {
                var client = new TcpClient();
                client.Connect(hostname, port);
                Connect(client.GetStream(), options);
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

            lock (_specialSync)
            {
                Close();

                _options = options ?? new VncClientConnectOptions();
                _stream = stream;

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
                    throw new VncException("IO error.", VncFailureReason.NetworkError, e);
                }
                catch (ObjectDisposedException e)
                {
                    throw new VncException("Connection closed.", VncFailureReason.NetworkError, e);
                }
                catch (SocketException e)
                {
                    throw new VncException("Connection failed.", VncFailureReason.NetworkError, e);
                }

                IsConnected = true; OnConnected();
                _threadMain = new Thread(ThreadMain);
                _threadMain.IsBackground = true;
                _threadMain.Start();
            }
        }

        void ThreadMain()
        {
            var requestExit = new ManualResetEvent(false);
            var requestUpdate = new AutoResetEvent(false);
            var requestThread = new Thread(() =>
            {
                var waitHandles = new WaitHandle[] { requestUpdate, requestExit };

                while (true)
                {
                    int startTime = Environment.TickCount;
                    if (WaitHandle.WaitAny(waitHandles) == 1) { return; }

                    try { SendFramebufferUpdateRequest(true); } catch (Exception) { return; }

                    int elapsedTime = Math.Max(0, Environment.TickCount - startTime);
                    int timeout = Math.Max(0, Math.Min(60000, (int)Math.Round(1000.0 / MaxUpdateRate) - elapsedTime));
                    if (timeout > 0) { requestExit.WaitOne(timeout); }
                }
            });
            requestThread.IsBackground = true;
            requestThread.Start();

            try
            {
                while (true)
                {
                    var command = ReceiveByte();
                    Require(command <= 3, "Unsupported command.",
                            VncFailureReason.UnrecognizedProtocolElement);

                    if (command == 0)
                    {
                        requestUpdate.Set();
                        HandleFramebufferUpdate();
                    }
                    else if (command == 1)
                    {
                        HandleSetColorMapEntries();
                    }
                    else if (command == 2)
                    {
                        HandleBell();
                    }
                    else if (command == 3)
                    {
                        HandleReceiveClipboardData();
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

            requestExit.Set();
            requestThread.Join();

            _stream = null; IsConnected = false; OnClosed();
        }

        void NegotiateVersion()
        {
            var version = Encoding.ASCII.GetString(Receive(12));
            var versionRegex = Regex.Match(version, @"^RFB (?<maj>[0-9]{3})\.(?<min>[0-9]{3})\n",
                               RegexOptions.Singleline | RegexOptions.CultureInvariant);
            Require(versionRegex.Success, "Not a VNC server.",
                    VncFailureReason.WrongKindOfServer);

            int serverMajor = int.Parse(versionRegex.Groups["maj"].Value);
            int serverMinor = int.Parse(versionRegex.Groups["min"].Value);
            Require(serverMajor > 3 || (serverMajor == 3 && serverMinor >= 8),
                    "RFB 3.8 not supported by server.",
                    VncFailureReason.UnsupportedProtocolVersion);

            Send(Encoding.ASCII.GetBytes("RFB 003.008\n"));
        }

        void NegotiateDesktop()
        {
            Send(new[] { (byte)(_options.ShareDesktop ? 1 : 0) });

            var width = VncUtility.DecodeUInt16BE(Receive(2), 0); SanityCheck(width > 0 && width < 0x8000);
            var height = VncUtility.DecodeUInt16BE(Receive(2), 0); SanityCheck(height > 0 && height < 0x8000);

            VncPixelFormat pixelFormat;
            try
            {
                pixelFormat = VncPixelFormat.Decode(Receive(VncPixelFormat.Size), 0);
            }
            catch (ArgumentException e)
            {
                throw new VncException("Unsupported pixel format.",
                                               VncFailureReason.UnsupportedPixelFormat, e);
            }

            var name = ReceiveString();
            Framebuffer = new VncFramebuffer(name, width, height, pixelFormat);
        }

        void NegotiateEncodings()
        {
            var encodings = new VncEncoding[]
            {
                VncEncoding.Zlib,
                VncEncoding.Hextile,
                VncEncoding.Copyrect,
                VncEncoding.Raw,
                VncEncoding.PseudoDesktopSize
            };

            Send(new[] { (byte)2, (byte)0 });
            Send(VncUtility.EncodeUInt16BE((ushort)encodings.Length));
            foreach (var encoding in encodings) { Send(VncUtility.EncodeUInt32BE((uint)encoding)); }
        }

        void SendFramebufferUpdateRequest(bool incremental)
        {
            var p = new byte[10];

            p[0] = (byte)3; p[1] = (byte)(incremental ? 1 : 0);
            VncUtility.EncodeUInt16BE(p, 2, (ushort)0);
            VncUtility.EncodeUInt16BE(p, 4, (ushort)0);
            VncUtility.EncodeUInt16BE(p, 6, (ushort)Framebuffer.Width);
            VncUtility.EncodeUInt16BE(p, 8, (ushort)Framebuffer.Height);

            Send(p);
        }

        /// <summary>
        /// Notifies the server that the local clipboard has changed.
        /// If you are implementing clipboard integration, use this to set the remote clipboard.
        /// </summary>
        /// <param name="data">The contents of the local clipboard.</param>
        public void SendLocalClipboardChange(string data)
        {
            Throw.If.Null(data, "data");

            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(data);

            var p = new byte[8 + bytes.Length];

            p[0] = (byte)6;
            VncUtility.EncodeUInt32BE(p, 4, (uint)bytes.Length);
            Array.Copy(bytes, 0, p, 8, bytes.Length);

            SendUser(bytes);
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

            SendUser(p);
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

            SendUser(p);
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
            Receive(1); // padding

            var firstColor = VncUtility.DecodeUInt16BE(Receive(2), 0);
            var numColors = VncUtility.DecodeUInt16BE(Receive(2), 0);

            for (int i = 0; i < numColors; i++)
            {
                var r = VncUtility.DecodeUInt16BE(Receive(2), 0);
                var g = VncUtility.DecodeUInt16BE(Receive(2), 0);
                var b = VncUtility.DecodeUInt16BE(Receive(2), 0);
            }
        }

        void HandleBell()
        {
            OnBell();
        }

        void HandleReceiveClipboardData()
        {
            Receive(3); // padding

            var clipboard = ReceiveString(0xffffff);

            OnRemoteClipboardChanged(new RemoteClipboardChangedEventArgs(clipboard));
        }

        static void Require(bool condition, string message, VncFailureReason reason)
        {
            if (condition) { return; }
            throw new VncException(message, reason);
        }

        static void SanityCheck(bool condition)
        {
            Require(condition, "Sanity check failed.", VncFailureReason.SanityCheckFailed);
        }

        byte[] Receive(int count)
        {
            var buffer = new byte[count];
            Receive(buffer, 0, buffer.Length);
            return buffer;
        }

        void Receive(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; )
            {
                int bytes = _stream.Read(buffer, offset + i, count - i);
                Require(bytes > 0, "Lost connection.", VncFailureReason.NetworkError);
                i += bytes;
            }
        }

        byte ReceiveByte()
        {
            int value = _stream.ReadByte();
            Require(value >= 0, "Lost connection.", VncFailureReason.NetworkError);
            return (byte)value;
        }


        string ReceiveString(int maxLength = 0xfff)
        {
            var length = VncUtility.DecodeUInt32BE(Receive(4), 0); SanityCheck(length <= maxLength);
            var value = Encoding.GetEncoding("iso-8859-1").GetString(Receive((int)length));
            return value;
        }

        void SendUser(byte[] buffer)
        {
            if (IsConnected) { Send(buffer); }
        }

        void Send(byte[] buffer)
        {
            Send(buffer, 0, buffer.Length);
        }

        void Send(byte[] buffer, int offset, int count)
        {
            if (_stream == null) { return; }

            lock (_specialSync)
            {
                var stream = _stream;
                if (stream == null) { return; }

                try
                {
                    stream.Write(buffer, offset, count);
                }
                catch (ObjectDisposedException)
                {

                }
                catch (IOException)
                {

                }
            }
        }

        protected virtual void OnBell()
        {
            RaiseBell();
        }

        protected void RaiseBell()
        {
            var ev = Bell;
            if (ev != null)
            {
                var sync = SynchronizationContext;
                if (sync != null) { sync.Post(sender => ev(sender, EventArgs.Empty), this); } else { ev(this, EventArgs.Empty); }
            }
        }

        protected virtual void OnConnected()
        {
            RaiseConnected();
        }

        protected void RaiseConnected()
        {
            var ev = Connected;
            if (ev != null)
            {
                var sync = SynchronizationContext;
                if (sync != null) { sync.Post(sender => ev(sender, EventArgs.Empty), this); } else { ev(this, EventArgs.Empty); }
            }
        }

        protected virtual void OnPasswordRequired(PasswordRequiredEventArgs e)
        {
            RaisePasswordRequired(e);
        }

        protected void RaisePasswordRequired(PasswordRequiredEventArgs e)
        {
            var ev = PasswordRequired;
            if (ev != null)
            {
                var sync = SynchronizationContext;
                if (sync != null) { sync.Post(sender => ev(sender, e), this); } else { ev(this, e); }
            }
        }

        protected virtual void OnClosed()
        {
            RaiseClosed();
        }

        protected void RaiseClosed()
        {
            var ev = Closed;
            if (ev != null)
            {
                var sync = SynchronizationContext;
                if (sync != null) { sync.Post(sender => ev(sender, EventArgs.Empty), this); } else { ev(this, EventArgs.Empty); }
            }
        }

        protected virtual void OnFramebufferChanged(FramebufferChangedEventArgs e)
        {
            RaiseFramebufferChanged(e);
        }

        protected void RaiseFramebufferChanged(FramebufferChangedEventArgs e)
        {
            var ev = FramebufferChanged;
            if (ev != null)
            {
                var sync = SynchronizationContext;
                if (sync != null) { sync.Post(sender => ev(sender, e), this); } else { ev(this, e); }
            }
        }

        protected virtual void OnRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            RaiseRemoteClipboardChanged(e);
        }

        protected void RaiseRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            var ev = RemoteClipboardChanged;
            if (ev != null)
            {
                var sync = SynchronizationContext;
                if (sync != null) { sync.Post(sender => ev(sender, e), this); } else { ev(this, e); }
            }
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
        /// The synchronization context, if any, to marshal events on.
        /// 
        /// The default is the current synchronization context when the <see cref="VncClient"/>
        /// is constructed. If you create it in a Windows Forms thread, for instance, events
        /// will be called on the Windows Forms thread.
        /// 
        /// <c>null</c> will call events on an undefined thread.
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get;
            set;
        }
    }
}
