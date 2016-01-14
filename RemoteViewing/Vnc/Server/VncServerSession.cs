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

        struct Rectangle
        {
            public VncRectangle Region;
            public VncEncoding Encoding;
            public byte[] Contents;
        }
        VncStream _c = new VncStream();
        VncEncoding[] _clientEncoding = new VncEncoding[0];
        VncPixelFormat _clientPixelFormat;
        int _clientWidth, _clientHeight;
        Version _clientVersion = new Version();
        VncServerSessionOptions _options;
        VncFramebufferCache _fbuAutoCache;
        List<Rectangle> _fbuRectangles = new List<Rectangle>();
        object _fbuSync = new object();
        IVncFramebufferSource _fbSource;
        double _maxUpdateRate;
        Utility.PeriodicThread _requester;
        object _specialSync = new object();
        Thread _threadMain;
#if DEFLATESTREAM_FLUSH_WORKS
        MemoryStream _zlibMemoryStream;
        DeflateStream _zlibDeflater;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="VncServerSession"/> class.
        /// </summary>
        public VncServerSession()
        {
            MaxUpdateRate = 15;
        }

        /// <summary>
        /// Closes the connection with the remote client.
        /// </summary>
        public void Close()
        {
            var thread = _threadMain; _c.Close();
            if (thread != null) { thread.Join(); }
        }

        /// <summary>
        /// Starts a session with a VNC client.
        /// </summary>
        /// <param name="stream">The stream containing the connection.</param>
        /// <param name="options">Session options, if any.</param>
        public void Connect(Stream stream, VncServerSessionOptions options = null)
        {
            Throw.If.Null(stream, "stream");

            lock (_c.SyncRoot)
            {
                Close();

                _options = options ?? new VncServerSessionOptions();
                _c.Stream = stream;

                _threadMain = new Thread(ThreadMain);
                _threadMain.IsBackground = true;
                _threadMain.Start();
            }
        }

        void ThreadMain()
        {
            _requester = new Utility.PeriodicThread();

            try
            {
                InitFramebufferEncoder();

                AuthenticationMethod[] methods;
                NegotiateVersion(out methods);
                NegotiateSecurity(methods);
                NegotiateDesktop();
                NegotiateEncodings();

                _requester.Start(FramebufferSendChanges, () => MaxUpdateRate, false);

                IsConnected = true; OnConnected();

                while (true)
                {
                    var command = _c.ReceiveByte();

                    switch (command)
                    {
                        case 0:
                            HandleSetPixelFormat();
                            break;

                        case 2:
                            HandleSetEncodings();
                            break;

                        case 3:
                            HandleFramebufferUpdateRequest();
                            break;

                        case 4:
                            HandleKeyEvent();
                            break;

                        case 5:
                            HandlePointerEvent();
                            break;

                        case 6:
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

            _requester.Stop();

            _c.Stream = null;
            if (IsConnected)
            {
                IsConnected = false; OnClosed();
            }
            else
            {
                OnConnectionFailed();
            }
        }

        void NegotiateVersion(out AuthenticationMethod[] methods)
        {
            _c.SendVersion(new Version(3, 8));

            _clientVersion = _c.ReceiveVersion();
            if (_clientVersion == new Version(3, 8))
            {
                methods = new[]
                {
                    _options.AuthenticationMethod == AuthenticationMethod.Password
                        ? AuthenticationMethod.Password : AuthenticationMethod.None
                };
            }
            else
            {
                methods = new AuthenticationMethod[0];
            }
        }

        void NegotiateSecurity(AuthenticationMethod[] methods)
        {
            _c.SendByte((byte)methods.Length);
            VncStream.Require(methods.Length > 0,
                                  "Client is not allowed in.",
                                  VncFailureReason.NoSupportedAuthenticationMethods);
            foreach (var method in methods) { _c.SendByte((byte)method); }

            var selectedMethod = (AuthenticationMethod)_c.ReceiveByte();
            VncStream.Require(methods.Contains(selectedMethod),
                              "Invalid authentication method.",
                              VncFailureReason.UnrecognizedProtocolElement);

            bool success = true;
            if (selectedMethod == AuthenticationMethod.Password)
            {
                var challenge = VncPasswordChallenge.GenerateChallenge();
                using (new Utility.AutoClear(challenge))
                {
                    _c.Send(challenge);

                    var response = _c.Receive(16);
                    using (new Utility.AutoClear(response))
                    {
                        var e = new PasswordProvidedEventArgs(challenge, response);
                        OnPasswordProvided(e);
                        success = e.IsAuthenticated;
                    }
                }
            }

            _c.SendUInt32BE(success ? 0 : (uint)1);
            VncStream.Require(success,
                              "Failed to authenticate.",
                              VncFailureReason.AuthenticationFailed);
        }

        void NegotiateDesktop()
        {
            byte shareDesktopSetting = _c.ReceiveByte();
            bool shareDesktop = shareDesktopSetting != 0;

            var e = new CreatingDesktopEventArgs(shareDesktop);
            OnCreatingDesktop(e);

            var fbSource = _fbSource;
            Framebuffer = fbSource != null ? fbSource.Capture() : null;
            VncStream.Require(Framebuffer != null,
                              "No framebuffer. Make sure you've called SetFramebufferSource. It can be set to a VncFramebuffer.",
                              VncFailureReason.SanityCheckFailed);
            _clientPixelFormat = Framebuffer.PixelFormat;
            _clientWidth = Framebuffer.Width; _clientHeight = Framebuffer.Height;
            _fbuAutoCache = null;
            
            _c.SendUInt16BE((ushort)Framebuffer.Width);
            _c.SendUInt16BE((ushort)Framebuffer.Height);
            var pixelFormat = new byte[VncPixelFormat.Size];
            Framebuffer.PixelFormat.Encode(pixelFormat, 0);
            _c.Send(pixelFormat);
            _c.SendString(Framebuffer.Name, true);
        }

        void NegotiateEncodings()
        {
            _clientEncoding = new VncEncoding[0]; // Default to no encodings.
        }

        void HandleSetPixelFormat()
        {
            _c.Receive(3);

            var pixelFormat = _c.Receive(VncPixelFormat.Size);
            _clientPixelFormat = VncPixelFormat.Decode(pixelFormat, 0);
        }

        void HandleSetEncodings()
        {
            _c.Receive(1);

            int encodingCount = _c.ReceiveUInt16BE(); VncStream.SanityCheck(encodingCount <= 0x1ff);
            var clientEncoding = new VncEncoding[encodingCount];
            for (int i = 0; i < clientEncoding.Length; i++)
            {
                uint encoding = _c.ReceiveUInt32BE();
                clientEncoding[i] = (VncEncoding)encoding;
            }
            _clientEncoding = clientEncoding;
        }

        void HandleFramebufferUpdateRequest()
        {
            var incremental = _c.ReceiveByte() != 0;
            var region = _c.ReceiveRectangle();

            lock (FramebufferUpdateRequestLock)
            {
                FramebufferUpdateRequest = new FramebufferUpdateRequest(incremental, region);
                FramebufferChanged();
            }
        }

        void HandleKeyEvent()
        {
            var pressed = _c.ReceiveByte() != 0; _c.Receive(2);
            var keysym = (int)_c.ReceiveUInt32BE();

            OnKeyChanged(new KeyChangedEventArgs(keysym, pressed));
        }

        void HandlePointerEvent()
        {
            int pressedButtons = _c.ReceiveByte();
            int x = _c.ReceiveUInt16BE();
            int y = _c.ReceiveUInt16BE();

            OnPointerChanged(new PointerChangedEventArgs(x, y, pressedButtons));
        }

        void HandleReceiveClipboardData()
        {
            _c.Receive(3); // padding

            var clipboard = _c.ReceiveString(0xffffff);

            OnRemoteClipboardChanged(new RemoteClipboardChangedEventArgs(clipboard));
        }

        /// <summary>
        /// Tells the client to play a bell sound.
        /// </summary>
        public void Bell()
        {
            lock (_c.SyncRoot)
            {
                if (!IsConnected) { return; }
                _c.SendByte((byte)2);
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

            lock (_c.SyncRoot)
            {
                if (!IsConnected) { return; }
                _c.SendByte((byte)3);
                _c.Send(new byte[3]);
                _c.SendString(data, true);
            }
        }

        /// <summary>
        /// Sets the framebuffer source.
        /// </summary>
        /// <param name="source">The framebuffer source, or <c>null</c> if you intend to handle the framebuffer manually.</param>
        public void SetFramebufferSource(IVncFramebufferSource source)
        {
            _fbSource = source;
        }

        /// <summary>
        /// Notifies the framebuffer update thread to check for recent changes.
        /// </summary>
        public void FramebufferChanged()
        {
            _requester.Signal();
        }

        bool FramebufferSendChanges()
        {
            var e = new FramebufferUpdatingEventArgs();

            lock (FramebufferUpdateRequestLock)
            {
                if (FramebufferUpdateRequest != null)
                {
                    var fbSource = _fbSource;
                    if (fbSource != null)
                    {
                        var newFramebuffer = fbSource.Capture();
                        if (newFramebuffer != null && newFramebuffer != Framebuffer)
                        {
                            Framebuffer = newFramebuffer;
                        }
                    }

                    OnFramebufferCapturing();
                    OnFramebufferUpdating(e);

                    if (!e.Handled)
                    {
                        if (_fbuAutoCache == null || _fbuAutoCache.Framebuffer != Framebuffer)
                        {
                            _fbuAutoCache = new VncFramebufferCache(Framebuffer);
                        }

                        e.Handled = true;
                        e.SentChanges = _fbuAutoCache.RespondToUpdateRequest(this);
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
            _fbuRectangles.Clear();
        }

        void AddRegion(VncRectangle region, VncEncoding encoding, byte[] contents)
        {
            _fbuRectangles.Add(new Rectangle() { Region = region, Encoding = encoding, Contents = contents });

            // Avoid the overflow of updated rectangle count.
            // NOTE: EndUpdate may implicitly add one for desktop resizing.
            if (_fbuRectangles.Count >= ushort.MaxValue - 1)
            {
                FramebufferManualEndUpdate(); FramebufferManualBeginUpdate();
            }
        }

        /// <summary>
        /// Queues an update corresponding to one region of the framebuffer being copide to another.
        /// 
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        public void FramebufferManualCopyRegion(VncRectangle target, int sourceX, int sourceY)
        {
            if (!_clientEncoding.Contains(VncEncoding.CopyRect))
            {
                var source = new VncRectangle(sourceX, sourceY, target.Width, target.Height);
                var region = VncRectangle.Union(source, target);

                if (region.Area > source.Area + target.Area) { FramebufferManualInvalidate(new[] { source, target }); }
                else                                         { FramebufferManualInvalidate(region); }
                return;
            }

            var contents = new byte[4];
            VncUtility.EncodeUInt16BE(contents, 0, (ushort)sourceX);
            VncUtility.EncodeUInt16BE(contents, 2, (ushort)sourceY);
            AddRegion(target, VncEncoding.CopyRect, contents);
        }

        void InitFramebufferEncoder()
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
            FramebufferManualInvalidate(new VncRectangle(0, 0, Framebuffer.Width, Framebuffer.Height));
        }

        /// <summary>
        /// Queues an update for the specified region.
        /// 
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        /// <param name="region">The region to invalidate.</param>
        public void FramebufferManualInvalidate(VncRectangle region)
        {
            var fb = Framebuffer; var cpf = _clientPixelFormat;
            region = VncRectangle.Intersect(region, new VncRectangle(0, 0, _clientWidth, _clientHeight));
            if (region.IsEmpty) { return; }

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
                AddRegion(region, VncEncoding.Raw, contents);
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
            foreach (var region in regions) { FramebufferManualInvalidate(region); }
        }

        /// <summary>
        /// Completes a manual framebuffer update.
        /// 
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </summary>
        public bool FramebufferManualEndUpdate()
        {
            var fb = Framebuffer;
            if (_clientWidth != fb.Width || _clientHeight != fb.Height)
            {
                if (_clientEncoding.Contains(VncEncoding.PseudoDesktopSize))
                {
                    var region = new VncRectangle(0, 0, fb.Width, fb.Height);
                    AddRegion(region, VncEncoding.PseudoDesktopSize, new byte[0]);
                    _clientWidth = Framebuffer.Width; _clientHeight = Framebuffer.Height;
                }
            }

            if (_fbuRectangles.Count == 0) { return false; }
            FramebufferUpdateRequest = null;

            lock (_c.SyncRoot)
            {
                _c.Send(new byte[2] { 0, 0 });
                _c.SendUInt16BE((ushort)_fbuRectangles.Count);

                foreach (var rectangle in _fbuRectangles)
                {
                    _c.SendRectangle(rectangle.Region);
                    _c.SendUInt32BE((uint)rectangle.Encoding);
                    _c.Send(rectangle.Contents);
                }

                _fbuRectangles.Clear(); return true;
            }
        }

        protected virtual void OnPasswordProvided(PasswordProvidedEventArgs e)
        {
            RaisePasswordProvided(e);
        }

        protected void RaisePasswordProvided(PasswordProvidedEventArgs e)
        {
            var ev = PasswordProvided;
            if (ev != null) { ev(this, e); }
        }

        protected virtual void OnCreatingDesktop(CreatingDesktopEventArgs e)
        {
            RaiseCreatingDesktop(e);
        }

        protected void RaiseCreatingDesktop(CreatingDesktopEventArgs e)
        {
            var ev = CreatingDesktop;
            if (ev != null) { ev(this, e); }
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

        protected virtual void OnFramebufferCapturing()
        {
            RaiseFramebufferCapturing();
        }

        protected void RaiseFramebufferCapturing()
        {
            var ev = FramebufferCapturing;
            if (ev != null) { ev(this, EventArgs.Empty); }
        }

        protected virtual void OnFramebufferUpdating(FramebufferUpdatingEventArgs e)
        {
            RaiseFramebufferUpdating(e);
        }

        protected void RaiseFramebufferUpdating(FramebufferUpdatingEventArgs e)
        {
            var ev = FramebufferUpdating;
            if (ev != null) { ev(this, e); }
        }

        protected void OnKeyChanged(KeyChangedEventArgs e)
        {
            RaiseKeyChanged(e);
        }

        protected void RaiseKeyChanged(KeyChangedEventArgs e)
        {
            var ev = KeyChanged;
            if (ev != null) { ev(this, e); }
        }

        protected void OnPointerChanged(PointerChangedEventArgs e)
        {
            RaisePointerChanged(e);
        }

        protected void RaisePointerChanged(PointerChangedEventArgs e)
        {
            var ev = PointerChanged;
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
        /// The protocol version of the client.
        /// </summary>
        public Version ClientVersion
        {
            get { return _clientVersion; }
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
            get { return _fbuSync; }
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
        /// Store anything you want here.
        /// </summary>
        public object UserData
        {
            get;
            set;
        }
    }
}
