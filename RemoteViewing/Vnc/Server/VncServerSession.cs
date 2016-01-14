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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Serves a VNC client with framebuffer information and receives keyboard and mouse interactions.
    /// </summary>
    public class VncServerSession
    {
        /// <summary>
        /// Occurs when the VNC client provides a password.
        /// Respond to this event by accepting or rejecting the password.
        /// </summary>
        public event EventHandler<PasswordProvidedEventArgs> PasswordProvided;

        /// <summary>
        /// Occurs when the client requests access to the desktop.
        /// It may request exclusive or shared access -- this event will relay that information.
        /// </summary>
        public event EventHandler<CreatingDesktopEventArgs> CreatingDesktop;

        /// <summary>
        /// Occurs when the VNC client has successfully connected to the server.
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
        /// Occurs when the framebuffer needs to be captured.
        /// If you have not called <see cref="VncServerSession.SetFramebufferSource"/>, alter the framebuffer
        /// in response to this event.
        ///
        /// <see cref="VncServerSession.FramebufferUpdateRequestLock"/> is held automatically while this event is raised.
        /// </summary>
        public event EventHandler FramebufferCapturing;

        /// <summary>
        /// Occurs when the framebuffer needs to be updated.
        /// If you do not set <see cref="HandledEventArgs.Handled"/> on <see cref="FramebufferUpdatingEventArgs"/>,
        /// <see cref="VncServerSession"/> will determine the updated regions itself.
        ///
        /// <see cref="VncServerSession.FramebufferUpdateRequestLock"/> is held automatically while this event is raised.
        /// </summary>
        public event EventHandler<FramebufferUpdatingEventArgs> FramebufferUpdating;

        /// <summary>
        /// Occurs when a key has been pressed or released.
        /// </summary>
        public event EventHandler<KeyChangedEventArgs> KeyChanged;

        /// <summary>
        /// Occurs on a mouse movement, button click, etc.
        /// </summary>
        public event EventHandler<PointerChangedEventArgs> PointerChanged;

        /// <summary>
        /// Occurs when the clipboard changes on the remote client.
        /// If you are implementing clipboard integration, use this to set the local clipboard.
        /// </summary>
        public event EventHandler<RemoteClipboardChangedEventArgs> RemoteClipboardChanged;

        private struct Rectangle
        {
            public VncRectangle Region;
            public VncEncoding Encoding;
            public byte[] Contents;
        }

        private VncStream _c = new VncStream();
        private VncEncoding[] _clientEncoding = new VncEncoding[0];
        private VncPixelFormat _clientPixelFormat;
        private int _clientWidth;
        private int _clientHeight;
        private Version _clientVersion = new Version();
        private VncServerSessionOptions _options;
        private VncFramebufferCache _fbuAutoCache;
        private List<Rectangle> _fbuRectangles = new List<Rectangle>();
        private object _fbuSync = new object();
        private IVncFramebufferSource _fbSource;
        private double _maxUpdateRate;
        private Utility.PeriodicThread _requester;
        private object _specialSync = new object();
        private Thread _threadMain;
#if DEFLATESTREAM_FLUSH_WORKS
        MemoryStream _zlibMemoryStream;
        DeflateStream _zlibDeflater;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="VncServerSession"/> class.
        /// </summary>
        public VncServerSession()
        {
            this.MaxUpdateRate = 15;
        }

        /// <summary>
        /// Closes the connection with the remote client.
        /// </summary>
        public void Close()
        {
            var thread = this._threadMain;
            this._c.Close();
            if (thread != null)
            {
                thread.Join();
            }
        }

        /// <summary>
        /// Starts a session with a VNC client.
        /// </summary>
        /// <param name="stream">The stream containing the connection.</param>
        /// <param name="options">Session options, if any.</param>
        public void Connect(Stream stream, VncServerSessionOptions options = null)
        {
            Throw.If.Null(stream, "stream");

            lock (this._c.SyncRoot)
            {
                this.Close();

                this._options = options ?? new VncServerSessionOptions();
                this._c.Stream = stream;

                this._threadMain = new Thread(this.ThreadMain);
                this._threadMain.IsBackground = true;
                this._threadMain.Start();
            }
        }

        private void ThreadMain()
        {
            this._requester = new Utility.PeriodicThread();

            try
            {
                this.InitFramebufferEncoder();

                AuthenticationMethod[] methods;
                this.NegotiateVersion(out methods);
                this.NegotiateSecurity(methods);
                this.NegotiateDesktop();
                this.NegotiateEncodings();

                this._requester.Start(this.FramebufferSendChanges, () => this.MaxUpdateRate, false);

                this.IsConnected = true;
                this.OnConnected();

                while (true)
                {
                    var command = this._c.ReceiveByte();

                    switch (command)
                    {
                        case 0:
                            this.HandleSetPixelFormat();
                            break;

                        case 2:
                            this.HandleSetEncodings();
                            break;

                        case 3:
                            this.HandleFramebufferUpdateRequest();
                            break;

                        case 4:
                            this.HandleKeyEvent();
                            break;

                        case 5:
                            this.HandlePointerEvent();
                            break;

                        case 6:
                            this.HandleReceiveClipboardData();
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

            this._requester.Stop();

            this._c.Stream = null;
            if (this.IsConnected)
            {
                this.IsConnected = false;
                this.OnClosed();
            }
            else
            {
                this.OnConnectionFailed();
            }
        }

        private void NegotiateVersion(out AuthenticationMethod[] methods)
        {
            this._c.SendVersion(new Version(3, 8));

            this._clientVersion = this._c.ReceiveVersion();
            if (this._clientVersion == new Version(3, 8))
            {
                methods = new[]
                {
                    this._options.AuthenticationMethod == AuthenticationMethod.Password
                        ? AuthenticationMethod.Password : AuthenticationMethod.None
                };
            }
            else
            {
                methods = new AuthenticationMethod[0];
            }
        }

        private void NegotiateSecurity(AuthenticationMethod[] methods)
        {
            this._c.SendByte((byte)methods.Length);
            VncStream.Require(
                methods.Length > 0,
                                  "Client is not allowed in.",
                                  VncFailureReason.NoSupportedAuthenticationMethods);
            foreach (var method in methods)
            {
                this._c.SendByte((byte)method);
            }

            var selectedMethod = (AuthenticationMethod)this._c.ReceiveByte();
            VncStream.Require(
                methods.Contains(selectedMethod),
                              "Invalid authentication method.",
                              VncFailureReason.UnrecognizedProtocolElement);

            bool success = true;
            if (selectedMethod == AuthenticationMethod.Password)
            {
                var challenge = VncPasswordChallenge.GenerateChallenge();
                using (new Utility.AutoClear(challenge))
                {
                    this._c.Send(challenge);

                    var response = this._c.Receive(16);
                    using (new Utility.AutoClear(response))
                    {
                        var e = new PasswordProvidedEventArgs(challenge, response);
                        this.OnPasswordProvided(e);
                        success = e.IsAuthenticated;
                    }
                }
            }

            this._c.SendUInt32BE(success ? 0 : (uint)1);
            VncStream.Require(
                success,
                              "Failed to authenticate.",
                              VncFailureReason.AuthenticationFailed);
        }

        private void NegotiateDesktop()
        {
            byte shareDesktopSetting = this._c.ReceiveByte();
            bool shareDesktop = shareDesktopSetting != 0;

            var e = new CreatingDesktopEventArgs(shareDesktop);
            this.OnCreatingDesktop(e);

            var fbSource = this._fbSource;
            this.Framebuffer = fbSource != null ? fbSource.Capture() : null;
            VncStream.Require(
                this.Framebuffer != null,
                              "No framebuffer. Make sure you've called SetFramebufferSource. It can be set to a VncFramebuffer.",
                              VncFailureReason.SanityCheckFailed);
            this._clientPixelFormat = this.Framebuffer.PixelFormat;
            this._clientWidth = this.Framebuffer.Width;
            this._clientHeight = this.Framebuffer.Height;
            this._fbuAutoCache = null;

            this._c.SendUInt16BE((ushort)this.Framebuffer.Width);
            this._c.SendUInt16BE((ushort)this.Framebuffer.Height);
            var pixelFormat = new byte[VncPixelFormat.Size];
            this.Framebuffer.PixelFormat.Encode(pixelFormat, 0);
            this._c.Send(pixelFormat);
            this._c.SendString(this.Framebuffer.Name, true);
        }

        private void NegotiateEncodings()
        {
            this._clientEncoding = new VncEncoding[0]; // Default to no encodings.
        }

        private void HandleSetPixelFormat()
        {
            this._c.Receive(3);

            var pixelFormat = this._c.Receive(VncPixelFormat.Size);
            this._clientPixelFormat = VncPixelFormat.Decode(pixelFormat, 0);
        }

        private void HandleSetEncodings()
        {
            this._c.Receive(1);

            int encodingCount = this._c.ReceiveUInt16BE();
            VncStream.SanityCheck(encodingCount <= 0x1ff);
            var clientEncoding = new VncEncoding[encodingCount];
            for (int i = 0; i < clientEncoding.Length; i++)
            {
                uint encoding = this._c.ReceiveUInt32BE();
                clientEncoding[i] = (VncEncoding)encoding;
            }

            this._clientEncoding = clientEncoding;
        }

        private void HandleFramebufferUpdateRequest()
        {
            var incremental = this._c.ReceiveByte() != 0;
            var region = this._c.ReceiveRectangle();

            lock (this.FramebufferUpdateRequestLock)
            {
                this.FramebufferUpdateRequest = new FramebufferUpdateRequest(incremental, region);
                this.FramebufferChanged();
            }
        }

        private void HandleKeyEvent()
        {
            var pressed = this._c.ReceiveByte() != 0;
            this._c.Receive(2);
            var keysym = (int)this._c.ReceiveUInt32BE();

            this.OnKeyChanged(new KeyChangedEventArgs(keysym, pressed));
        }

        private void HandlePointerEvent()
        {
            int pressedButtons = this._c.ReceiveByte();
            int x = this._c.ReceiveUInt16BE();
            int y = this._c.ReceiveUInt16BE();

            this.OnPointerChanged(new PointerChangedEventArgs(x, y, pressedButtons));
        }

        private void HandleReceiveClipboardData()
        {
            this._c.Receive(3); // padding

            var clipboard = this._c.ReceiveString(0xffffff);

            this.OnRemoteClipboardChanged(new RemoteClipboardChangedEventArgs(clipboard));
        }

        /// <summary>
        /// Tells the client to play a bell sound.
        /// </summary>
        public void Bell()
        {
            lock (this._c.SyncRoot)
            {
                if (!this.IsConnected)
                {
                    return;
                }

                this._c.SendByte((byte)2);
            }
        }

        /// <summary>
        /// Notifies the client that the local clipboard has changed.
        /// If you are implementing clipboard integration, use this to set the remote clipboard.
        /// </summary>
        /// <param name="data">The contents of the local clipboard.</param>
        public void SendLocalClipboardChange(string data)
        {
            Throw.If.Null(data, "data");

            lock (this._c.SyncRoot)
            {
                if (!this.IsConnected)
                {
                    return;
                }

                this._c.SendByte((byte)3);
                this._c.Send(new byte[3]);
                this._c.SendString(data, true);
            }
        }

        /// <summary>
        /// Sets the framebuffer source.
        /// </summary>
        /// <param name="source">The framebuffer source, or <c>null</c> if you intend to handle the framebuffer manually.</param>
        public void SetFramebufferSource(IVncFramebufferSource source)
        {
            this._fbSource = source;
        }

        /// <summary>
        /// Notifies the framebuffer update thread to check for recent changes.
        /// </summary>
        public void FramebufferChanged()
        {
            this._requester.Signal();
        }

        private bool FramebufferSendChanges()
        {
            var e = new FramebufferUpdatingEventArgs();

            lock (this.FramebufferUpdateRequestLock)
            {
                if (this.FramebufferUpdateRequest != null)
                {
                    var fbSource = this._fbSource;
                    if (fbSource != null)
                    {
                        var newFramebuffer = fbSource.Capture();
                        if (newFramebuffer != null && newFramebuffer != this.Framebuffer)
                        {
                            this.Framebuffer = newFramebuffer;
                        }
                    }

                    this.OnFramebufferCapturing();
                    this.OnFramebufferUpdating(e);

                    if (!e.Handled)
                    {
                        if (this._fbuAutoCache == null || this._fbuAutoCache.Framebuffer != this.Framebuffer)
                        {
                            this._fbuAutoCache = new VncFramebufferCache(this.Framebuffer);
                        }

                        e.Handled = true;
                        e.SentChanges = this._fbuAutoCache.RespondToUpdateRequest(this);
                    }
                }
            }

            return e.SentChanges;
        }

        /// <summary>
        /// Begins a manual framebuffer update.
        ///
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        public void FramebufferManualBeginUpdate()
        {
            this._fbuRectangles.Clear();
        }

        private void AddRegion(VncRectangle region, VncEncoding encoding, byte[] contents)
        {
            this._fbuRectangles.Add(new Rectangle() { Region = region, Encoding = encoding, Contents = contents });

            // Avoid the overflow of updated rectangle count.
            // NOTE: EndUpdate may implicitly add one for desktop resizing.
            if (this._fbuRectangles.Count >= ushort.MaxValue - 1)
            {
                this.FramebufferManualEndUpdate();
                this.FramebufferManualBeginUpdate();
            }
        }

        /// <summary>
        /// Queues an update corresponding to one region of the framebuffer being copide to another.
        ///
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        public void FramebufferManualCopyRegion(VncRectangle target, int sourceX, int sourceY)
        {
            if (!this._clientEncoding.Contains(VncEncoding.CopyRect))
            {
                var source = new VncRectangle(sourceX, sourceY, target.Width, target.Height);
                var region = VncRectangle.Union(source, target);

                if (region.Area > source.Area + target.Area)
                {
                    this.FramebufferManualInvalidate(new[] { source, target });
                }
                else
                {
                    this.FramebufferManualInvalidate(region);
                }

                return;
            }

            var contents = new byte[4];
            VncUtility.EncodeUInt16BE(contents, 0, (ushort)sourceX);
            VncUtility.EncodeUInt16BE(contents, 2, (ushort)sourceY);
            this.AddRegion(target, VncEncoding.CopyRect, contents);
        }

        private void InitFramebufferEncoder()
        {
#if DEFLATESTREAM_FLUSH_WORKS
            _zlibMemoryStream = new MemoryStream();
            _zlibDeflater = null;
#endif
        }

        /// <summary>
        /// Queues an update for the entire framebuffer.
        ///
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        public void FramebufferManualInvalidateAll()
        {
            this.FramebufferManualInvalidate(new VncRectangle(0, 0, this.Framebuffer.Width, this.Framebuffer.Height));
        }

        /// <summary>
        /// Queues an update for the specified region.
        ///
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        /// <param name="region">The region to invalidate.</param>
        public void FramebufferManualInvalidate(VncRectangle region)
        {
            var fb = this.Framebuffer;
            var cpf = this._clientPixelFormat;
            region = VncRectangle.Intersect(region, new VncRectangle(0, 0, this._clientWidth, this._clientHeight));
            if (region.IsEmpty)
            {
                return;
            }

            int x = region.X, y = region.Y, w = region.Width, h = region.Height, bpp = cpf.BytesPerPixel;
            var contents = new byte[w * h * bpp];

            VncPixelFormat.Copy(fb.GetBuffer(), fb.Stride, fb.PixelFormat, region,
                                contents, w * bpp, cpf);

#if DEFLATESTREAM_FLUSH_WORKS
            if (_clientEncoding.Contains(VncEncoding.Zlib))
            {
                _zlibMemoryStream.Position = 0;
                _zlibMemoryStream.SetLength(0);
                _zlibMemoryStream.Write(new byte[4], 0, 4);

                if (_zlibDeflater == null)
                {
                    _zlibMemoryStream.Write(new[] { (byte)120, (byte)218 }, 0, 2);
                    _zlibDeflater = new DeflateStream(_zlibMemoryStream, CompressionMode.Compress, false);
                }

                _zlibDeflater.Write(contents, 0, contents.Length);
                _zlibDeflater.Flush();
                contents = _zlibMemoryStream.ToArray();

                VncUtility.EncodeUInt32BE(contents, 0, (uint)(contents.Length - 4));
                AddRegion(region, VncEncoding.Zlib, contents);
            }
            else
#endif
            {
                this.AddRegion(region, VncEncoding.Raw, contents);
            }
        }

        /// <summary>
        /// Queues an update for each of the specified regions.
        ///
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        /// <param name="regions">The regions to invalidate.</param>
        public void FramebufferManualInvalidate(VncRectangle[] regions)
        {
            Throw.If.Null(regions, "regions");
            foreach (var region in regions)
            {
                this.FramebufferManualInvalidate(region);
            }
        }

        /// <summary>
        /// Completes a manual framebuffer update.
        ///
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        public bool FramebufferManualEndUpdate()
        {
            var fb = this.Framebuffer;
            if (this._clientWidth != fb.Width || this._clientHeight != fb.Height)
            {
                if (this._clientEncoding.Contains(VncEncoding.PseudoDesktopSize))
                {
                    var region = new VncRectangle(0, 0, fb.Width, fb.Height);
                    this.AddRegion(region, VncEncoding.PseudoDesktopSize, new byte[0]);
                    this._clientWidth = this.Framebuffer.Width;
                    this._clientHeight = this.Framebuffer.Height;
                }
            }

            if (this._fbuRectangles.Count == 0)
            {
                return false;
            }

            this.FramebufferUpdateRequest = null;

            lock (this._c.SyncRoot)
            {
                this._c.Send(new byte[2] { 0, 0 });
                this._c.SendUInt16BE((ushort)this._fbuRectangles.Count);

                foreach (var rectangle in this._fbuRectangles)
                {
                    this._c.SendRectangle(rectangle.Region);
                    this._c.SendUInt32BE((uint)rectangle.Encoding);
                    this._c.Send(rectangle.Contents);
                }

                this._fbuRectangles.Clear();
                return true;
            }
        }

        protected virtual void OnPasswordProvided(PasswordProvidedEventArgs e)
        {
            this.RaisePasswordProvided(e);
        }

        protected void RaisePasswordProvided(PasswordProvidedEventArgs e)
        {
            var ev = this.PasswordProvided;
            if (ev != null)
            {
                ev(this, e);
            }
        }

        protected virtual void OnCreatingDesktop(CreatingDesktopEventArgs e)
        {
            this.RaiseCreatingDesktop(e);
        }

        protected void RaiseCreatingDesktop(CreatingDesktopEventArgs e)
        {
            var ev = this.CreatingDesktop;
            if (ev != null)
            {
                ev(this, e);
            }
        }

        protected virtual void OnConnected()
        {
            this.RaiseConnected();
        }

        protected void RaiseConnected()
        {
            var ev = this.Connected;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        protected virtual void OnConnectionFailed()
        {
            this.RaiseConnectionFailed();
        }

        protected void RaiseConnectionFailed()
        {
            var ev = this.ConnectionFailed;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        protected virtual void OnClosed()
        {
            this.RaiseClosed();
        }

        protected void RaiseClosed()
        {
            var ev = this.Closed;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        protected virtual void OnFramebufferCapturing()
        {
            this.RaiseFramebufferCapturing();
        }

        protected void RaiseFramebufferCapturing()
        {
            var ev = this.FramebufferCapturing;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

        protected virtual void OnFramebufferUpdating(FramebufferUpdatingEventArgs e)
        {
            this.RaiseFramebufferUpdating(e);
        }

        protected void RaiseFramebufferUpdating(FramebufferUpdatingEventArgs e)
        {
            var ev = this.FramebufferUpdating;
            if (ev != null)
            {
                ev(this, e);
            }
        }

        protected void OnKeyChanged(KeyChangedEventArgs e)
        {
            this.RaiseKeyChanged(e);
        }

        protected void RaiseKeyChanged(KeyChangedEventArgs e)
        {
            var ev = this.KeyChanged;
            if (ev != null)
            {
                ev(this, e);
            }
        }

        protected void OnPointerChanged(PointerChangedEventArgs e)
        {
            this.RaisePointerChanged(e);
        }

        protected void RaisePointerChanged(PointerChangedEventArgs e)
        {
            var ev = this.PointerChanged;
            if (ev != null)
            {
                ev(this, e);
            }
        }

        protected virtual void OnRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            this.RaiseRemoteClipboardChanged(e);
        }

        protected void RaiseRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            var ev = this.RemoteClipboardChanged;
            if (ev != null)
            {
                ev(this, e);
            }
        }

        /// <summary>
        /// The protocol version of the client.
        /// </summary>
        public Version ClientVersion
        {
            get { return this._clientVersion; }
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
        /// Information about the client's most recent framebuffer update request.
        /// This may be <c>null</c> if the client has no framebuffer request queued.
        /// </summary>
        public FramebufferUpdateRequest FramebufferUpdateRequest
        {
            get;
            private set;
        }

        /// <summary>
        /// Lock this before performing any framebuffer updates.
        /// </summary>
        public object FramebufferUpdateRequestLock
        {
            get { return this._fbuSync; }
        }

        /// <summary>
        /// <c>true</c> if the server is connected to a client.
        /// </summary>
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// The max rate to send framebuffer updates at, in frames per second.
        ///
        /// The default is 15.
        /// </summary>
        public double MaxUpdateRate
        {
            get
            {
                return this._maxUpdateRate;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "Max update rate must be positive.",
                                                          (Exception)null);
                }

                this._maxUpdateRate = value;
            }
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
