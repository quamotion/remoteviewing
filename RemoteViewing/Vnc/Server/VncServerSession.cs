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

using RemoteViewing.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// Serves a VNC client with framebuffer information and receives keyboard and mouse interactions.
    /// </summary>
    public class VncServerSession : IVncServerSession
    {
        private ILog logger;
        private IVncPasswordChallenge passwordChallenge;
        private VncStream c = new VncStream();
        private VncEncoding[] clientEncoding = new VncEncoding[0];
        private VncPixelFormat clientPixelFormat;
        private int clientWidth;
        private int clientHeight;
        private Version clientVersion;
        private VncServerSessionOptions options;
        private IVncFramebufferCache fbuAutoCache;
        private List<Rectangle> fbuRectangles = new List<Rectangle>();
        private object fbuSync = new object();
        private IVncFramebufferSource fbSource;
        private double maxUpdateRate;
        private Utility.PeriodicThread requester;
        private object specialSync = new object();
        private Thread threadMain;
        private bool securityNegotiated = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncServerSession"/> class.
        /// </summary>
        public VncServerSession()
            : this(new VncPasswordChallenge(), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VncServerSession"/> class.
        /// </summary>
        /// <param name="passwordChallenge">
        /// The <see cref="IVncPasswordChallenge"/> to use to generate password challenges.
        /// </param>
        /// <param name="logger">
        /// The logger to use when logging diagnostic messages.
        /// </param>
        public VncServerSession(IVncPasswordChallenge passwordChallenge, ILog logger)
        {
            if (passwordChallenge == null)
            {
                throw new ArgumentNullException(nameof(passwordChallenge));
            }

            this.passwordChallenge = passwordChallenge;
            this.logger = logger;
            this.MaxUpdateRate = 15;
        }

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
        /// If you do not set <see cref="FramebufferUpdatingEventArgs.SentChanges"/>,
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

        /// <summary>
        /// Gets the protocol version of the client.
        /// </summary>
        public Version ClientVersion
        {
            get { return this.clientVersion; }
        }

        /// <summary>
        /// Gets the framebuffer for the VNC session.
        /// </summary>
        public VncFramebuffer Framebuffer
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public FramebufferUpdateRequest FramebufferUpdateRequest
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IVncPasswordChallenge"/> to use when authenticating clients.
        /// </summary>
        public IVncPasswordChallenge PasswordChallenge
        {
            get
            {
                return this.passwordChallenge;
            }

            set
            {
                if (this.securityNegotiated)
                {
                    throw new InvalidOperationException("You cannot change the password challenge once the security has been negotiated");
                }

                this.passwordChallenge = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ILog"/> logger to use when logging.
        /// </summary>
        public ILog Logger
        {
            get { return this.logger; }
            set { this.logger = value; }
        }

        /// <summary>
        /// Gets a lock which should be used before performing any framebuffer updates.
        /// </summary>
        public object FramebufferUpdateRequestLock
        {
            get { return this.fbuSync; }
        }

        /// <summary>
        /// Gets a value indicating whether the server is connected to a client.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server is connected to a client.
        /// </value>
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the max rate to send framebuffer updates at, in frames per second.
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
        /// Gets or sets a function which initializes a new <see cref="IVncFramebufferCache"/> for use by
        /// this <see cref="VncServerSession"/>.
        /// </summary>
        public Func<VncFramebuffer, ILog, IVncFramebufferCache> CreateFramebufferCache
        { get; set; } = (framebuffer, log) => new VncFramebufferCache(framebuffer, log);

        /// <summary>
        /// Gets a list of encoders which this <see cref="VncServerSession"/> can use.
        /// </summary>
        public Collection<VncEncoder> Encoders
        { get; } =
            new Collection<VncEncoder>()
            {
                new TightEncoder(),
                new ZlibEncoder(),
            };

        /// <summary>
        /// Gets or sets the encoder which is currently in use.
        /// </summary>
        public VncEncoder Encoder { get; protected set; } = new RawEncoder();

        /// <summary>
        /// Closes the connection with the remote client.
        /// </summary>
        public void Close()
        {
            var thread = this.threadMain;
            this.c.Close();
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
            this.Connect(stream, options, true);
        }

        /// <summary>
        /// Starts a session with a VNC client.
        /// </summary>
        /// <param name="stream">The stream containing the connection.</param>
        /// <param name="options">Session options, if any.</param>
        /// <param name="startThread">A value indicating whether to start the </param>
        /// <param name="forceConnected">A value indicated whether to immediately set <see cref="IsConnected"/>. Intended for unit tests only.</param>
        public void Connect(Stream stream, VncServerSessionOptions options = null, bool startThread = true, bool forceConnected = false)
        {
            Throw.If.Null(stream, "stream");

            lock (this.c.SyncRoot)
            {
                this.Close();

                this.options = options ?? new VncServerSessionOptions();
                this.c.Stream = stream;

                if (startThread)
                {
                    this.threadMain = new Thread(this.ThreadMain);
                    this.threadMain.IsBackground = true;
                    this.threadMain.Start();
                }

                if (forceConnected)
                {
                    this.IsConnected = true;
                }
            }
        }

        /// <summary>
        /// Tells the client to play a bell sound.
        /// </summary>
        public void Bell()
        {
            lock (this.c.SyncRoot)
            {
                if (!this.IsConnected)
                {
                    return;
                }

                this.c.SendByte((byte)2);
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

            lock (this.c.SyncRoot)
            {
                if (!this.IsConnected)
                {
                    return;
                }

                this.c.SendByte((byte)3);
                this.c.Send(new byte[3]);
                this.c.SendString(data, true);
            }
        }

        /// <summary>
        /// Sets the framebuffer source.
        /// </summary>
        /// <param name="source">The framebuffer source, or <see langword="null"/> if you intend to handle the framebuffer manually.</param>
        public void SetFramebufferSource(IVncFramebufferSource source)
        {
            this.fbSource = source;
        }

        /// <summary>
        /// Notifies the framebuffer update thread to check for recent changes.
        /// </summary>
        public void FramebufferChanged()
        {
            this.requester.Signal();
        }

        /// <inheritdoc/>
        public void FramebufferManualBeginUpdate()
        {
            this.fbuRectangles.Clear();
        }

        /// <summary>
        /// Queues an update corresponding to one region of the framebuffer being copied to another.
        /// </summary>
        /// <param name="target">
        /// The updated <see cref="VncRectangle"/>.
        /// </param>
        /// <param name="sourceX">
        /// The X coordinate of the source.
        /// </param>
        /// <param name="sourceY">
        /// The Y coordinate of the source.
        /// </param>
        /// <remarks>
        /// Do not call this method without holding <see cref="VncServerSession.FramebufferUpdateRequestLock"/>.
        /// </remarks>
        public void FramebufferManualCopyRegion(VncRectangle target, int sourceX, int sourceY)
        {
            if (!this.clientEncoding.Contains(VncEncoding.CopyRect))
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

        /// <inheritdoc/>
        public void FramebufferManualInvalidateAll()
        {
            this.FramebufferManualInvalidate(new VncRectangle(0, 0, this.Framebuffer.Width, this.Framebuffer.Height));
        }

        /// <inheritdoc/>
        public void FramebufferManualInvalidate(VncRectangle region)
        {
            var fb = this.Framebuffer;
            var cpf = this.clientPixelFormat;
            region = VncRectangle.Intersect(region, new VncRectangle(0, 0, this.clientWidth, this.clientHeight));
            if (region.IsEmpty)
            {
                return;
            }

            int x = region.X, y = region.Y, w = region.Width, h = region.Height, bpp = cpf.BytesPerPixel;
            var contents = new byte[w * h * bpp];

            VncPixelFormat.Copy(
                fb.GetBuffer(),
                fb.Width,
                fb.Stride,
                fb.PixelFormat,
                region,
                contents,
                w,
                w * bpp,
                cpf);

            this.AddRegion(region, VncEncoding.Raw, contents);
        }

        /// <inheritdoc/>
        public void FramebufferManualInvalidate(VncRectangle[] regions)
        {
            Throw.If.Null(regions, "regions");
            foreach (var region in regions)
            {
                this.FramebufferManualInvalidate(region);
            }
        }

        /// <inheritdoc/>s
        public bool FramebufferManualEndUpdate()
        {
            var fb = this.Framebuffer;
            if (this.clientWidth != fb.Width || this.clientHeight != fb.Height)
            {
                if (this.clientEncoding.Contains(VncEncoding.PseudoDesktopSize))
                {
                    var region = new VncRectangle(0, 0, fb.Width, fb.Height);
                    this.AddRegion(region, VncEncoding.PseudoDesktopSize, new byte[0]);
                    this.clientWidth = this.Framebuffer.Width;
                    this.clientHeight = this.Framebuffer.Height;
                }
            }

            if (this.fbuRectangles.Count == 0)
            {
                return false;
            }

            this.FramebufferUpdateRequest = null;

            lock (this.c.SyncRoot)
            {
                this.c.Send(new byte[2] { 0, 0 });
                this.c.SendUInt16BE((ushort)this.fbuRectangles.Count);

                foreach (var rectangle in this.fbuRectangles)
                {
                    Debug.Assert(rectangle.Encoding == VncEncoding.Raw, "Rectangles should be in raw format");

                    this.c.SendRectangle(rectangle.Region);
                    this.c.SendUInt32BE((uint)this.Encoder.Encoding);

                    this.Encoder.Send(this.c.Stream, this.clientPixelFormat, rectangle.Region, rectangle.Contents);
                }

                this.fbuRectangles.Clear();
                return true;
            }
        }

        internal bool FramebufferSendChanges()
        {
            var e = new FramebufferUpdatingEventArgs();

            lock (this.FramebufferUpdateRequestLock)
            {
                if (this.FramebufferUpdateRequest != null)
                {
                    var fbSource = this.fbSource;
                    if (fbSource != null)
                    {
                        try
                        {
                            var newFramebuffer = fbSource.Capture();
                            if (newFramebuffer != null && newFramebuffer != this.Framebuffer)
                            {
                                this.Framebuffer = newFramebuffer;
                            }
                        }
                        catch (Exception exc)
                        {
                            this.logger?.Log(LogLevel.Error, () => $"Capturing the framebuffer source failed: {exc}.");
                        }
                    }

                    this.OnFramebufferCapturing();
                    this.OnFramebufferUpdating(e);

                    if (!e.Handled)
                    {
                        if (this.fbuAutoCache == null || this.fbuAutoCache.Framebuffer != this.Framebuffer)
                        {
                            this.fbuAutoCache = this.CreateFramebufferCache(this.Framebuffer, this.logger);
                        }

                        e.Handled = true;
                        e.SentChanges = this.fbuAutoCache.RespondToUpdateRequest(this);
                    }
                }
            }

            return e.SentChanges;
        }

        /// <summary>
        /// Negotiates the version of the RFB protocol used by server and client.
        /// </summary>
        /// <param name="methods">
        /// The authentication methods supported by the server.
        /// </param>
        /// <returns>
        /// This method always returns <see langowrd="true"/>, though <paramref name="methods"/>
        /// will be an empty array if the client and server failed to negotiate version 3.8
        /// of the RFB protocol.
        /// </returns>
        internal bool NegotiateVersion(out AuthenticationMethod[] methods)
        {
            this.logger?.Log(LogLevel.Info, () => "Negotiating the version.");

            this.c.SendVersion(new Version(3, 8));

            this.clientVersion = this.c.ReceiveVersion();
            if (this.clientVersion == new Version(3, 8))
            {
                methods = new[]
                {
                    this.options.AuthenticationMethod == AuthenticationMethod.Password
                        ? AuthenticationMethod.Password : AuthenticationMethod.None
                };
            }
            else
            {
                // Clients using any version of the RFB protocol other than 3.8 are not supported.
                // We'll let them know by sending an empty list of authenticaton methods, which will cause
                // NegotiateSecurity to fail.
                methods = new AuthenticationMethod[0];
            }

            var supportedMethods = $"Supported autentication method are {string.Join(" ", methods)}";

            this.logger?.Log(LogLevel.Info, () => $"The client version is {this.clientVersion}");
            this.logger?.Log(LogLevel.Info, () => supportedMethods);

            return true;
        }

        /// <summary>
        /// Negotiates the security mechanism used to authenticate the client, and authenticates the client.
        /// </summary>
        /// <param name="methods">
        /// The authentication methods supported by this server.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the client authenticated successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <seealso href="https://github.com/rfbproto/rfbproto/blob/master/rfbproto.rst#712security"/>
        internal bool NegotiateSecurity(AuthenticationMethod[] methods)
        {
            this.logger?.Log(LogLevel.Info, () => "Negotiating security");

            this.c.SendByte((byte)methods.Length);

            if (methods.Length == 0)
            {
                this.logger?.Log(LogLevel.Warn, () => "The server and client could not agree on any authentication method.");
                this.c.SendString("The server and client could not agree on any authentication method.", includeLength: true);
                return false;
            }

            foreach (var method in methods)
            {
                this.c.SendByte((byte)method);
            }

            var selectedMethod = (AuthenticationMethod)this.c.ReceiveByte();
            if (!methods.Contains(selectedMethod))
            {
                this.c.SendUInt32BE(1);
                this.logger?.Log(LogLevel.Info, () => "Invalid authentication method.");
                this.c.SendString("Invalid authentication method.", includeLength: true);
                return false;
            }

            bool success = true;
            if (selectedMethod == AuthenticationMethod.Password)
            {
                var challenge = this.passwordChallenge.GenerateChallenge();
                using (new Utility.AutoClear(challenge))
                {
                    this.c.Send(challenge);

                    var response = this.c.Receive(16);
                    using (new Utility.AutoClear(response))
                    {
                        var e = new PasswordProvidedEventArgs(this.passwordChallenge, challenge, response);
                        this.OnPasswordProvided(e);
                        success = e.IsAuthenticated;
                    }
                }
            }

            this.c.SendUInt32BE(success ? 0 : (uint)1);

            if (!success)
            {
                this.logger?.Log(LogLevel.Info, () => "The user failed to authenticate.");
                this.c.SendString("Failed to authenticate", includeLength: true);
                return false;
            }

            this.logger?.Log(LogLevel.Info, () => "The user authenticated successfully.");
            this.securityNegotiated = true;

            return true;
        }

        internal void HandleSetEncodings()
        {
            this.c.Receive(1);

            int encodingCount = this.c.ReceiveUInt16BE();
            VncStream.SanityCheck(encodingCount <= 0x1ff);
            var clientEncoding = new VncEncoding[encodingCount];

            this.logger?.Log(LogLevel.Info, () => "The client supports {0} encodings:", null, encodingCount);

            for (int i = 0; i < clientEncoding.Length; i++)
            {
                var encoding = (VncEncoding)this.c.ReceiveUInt32BE();
                clientEncoding[i] = encoding;

                this.logger?.Log(LogLevel.Info, () => "- {0}", null, encoding);
            }

            this.clientEncoding = clientEncoding;
            this.InitFramebufferEncoder();
        }

        /// <summary>
        /// Raises the <see cref="PasswordProvided"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnPasswordProvided(PasswordProvidedEventArgs e)
        {
            this.PasswordProvided?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="CreatingDesktop"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnCreatingDesktop(CreatingDesktopEventArgs e)
        {
            this.CreatingDesktop?.Invoke(this, e);
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
        /// Raises the <see cref="Closed"/> event.
        /// </summary>
        protected virtual void OnClosed()
        {
            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="FramebufferCapturing"/> event.
        /// </summary>
        protected virtual void OnFramebufferCapturing()
        {
            this.FramebufferCapturing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="FramebufferUpdating"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnFramebufferUpdating(FramebufferUpdatingEventArgs e)
        {
            this.FramebufferUpdating?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="KeyChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected void OnKeyChanged(KeyChangedEventArgs e)
        {
            this.KeyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="PointerChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected void OnPointerChanged(PointerChangedEventArgs e)
        {
            this.PointerChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="RemoteClipboardChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnRemoteClipboardChanged(RemoteClipboardChangedEventArgs e)
        {
            this.RemoteClipboardChanged?.Invoke(this, e);
        }

        private void ThreadMain()
        {
            this.requester = new Utility.PeriodicThread();

            try
            {
                this.InitFramebufferEncoder();

                AuthenticationMethod[] methods;

                if (this.NegotiateVersion(out methods)
                    && this.NegotiateSecurity(methods))
                {
                    this.NegotiateDesktop();

                    this.requester.Start(() => this.FramebufferSendChanges(), () => this.MaxUpdateRate, false);

                    this.IsConnected = true;
                    this.logger?.Log(LogLevel.Info, () => "The client has connected successfully");

                    this.OnConnected();

                    var previousCommand = VncMessageType.Unknown;
                    var commandCount = 0;

                    while (true)
                    {
                        var command = (VncMessageType)this.c.ReceiveByte();

                        // If the same command is sent repeatedly (e.g. FramebufferUpdateRequest), suppress repeated requests
                        if (previousCommand != command || commandCount >= 25)
                        {
                            if (commandCount > 0)
                            {
                                this.logger?.Log(LogLevel.Info, () => $"Suppressed {commandCount} notifications of the {command} command at the Info level.");
                            }

                            this.logger?.Log(LogLevel.Info, () => $"Received the {command} command.");
                            commandCount = 0;
                        }
                        else
                        {
                            this.logger?.Log(LogLevel.Debug, () => $"Received the {command} command");
                            commandCount++;
                        }

                        previousCommand = command;

                        switch (command)
                        {
                            case VncMessageType.SetPixelFormat:
                                this.HandleSetPixelFormat();
                                break;

                            case VncMessageType.SetEncodings:
                                this.HandleSetEncodings();
                                break;

                            case VncMessageType.FrameBufferUpdateRequest:
                                this.HandleFramebufferUpdateRequest();
                                break;

                            case VncMessageType.KeyEvent:
                                this.HandleKeyEvent();
                                break;

                            case VncMessageType.PointerEvent:
                                this.HandlePointerEvent();
                                break;

                            case VncMessageType.ClientCutText:
                                this.HandleReceiveClipboardData();
                                break;

                            default:
                                VncStream.Require(
                                    false,
                                    "Unsupported command.",
                                    VncFailureReason.UnrecognizedProtocolElement);

                                break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.logger?.Log(LogLevel.Error, () => $"VNC server session stopped due to: {exception.Message}");
            }

            this.requester.Stop();

            this.c.Stream = null;
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

        private void NegotiateDesktop()
        {
            this.logger?.Log(LogLevel.Info, () => "Negotiating desktop settings");

            byte shareDesktopSetting = this.c.ReceiveByte();
            bool shareDesktop = shareDesktopSetting != 0;

            var e = new CreatingDesktopEventArgs(shareDesktop);
            this.OnCreatingDesktop(e);

            var fbSource = this.fbSource;
            this.Framebuffer = fbSource != null ? fbSource.Capture() : null;
            VncStream.Require(
                this.Framebuffer != null,
                              "No framebuffer. Make sure you've called SetFramebufferSource. It can be set to a VncFramebuffer.",
                              VncFailureReason.SanityCheckFailed);
            this.clientPixelFormat = this.Framebuffer.PixelFormat;
            this.clientWidth = this.Framebuffer.Width;
            this.clientHeight = this.Framebuffer.Height;
            this.fbuAutoCache = null;

            this.c.SendUInt16BE((ushort)this.Framebuffer.Width);
            this.c.SendUInt16BE((ushort)this.Framebuffer.Height);
            var pixelFormat = new byte[VncPixelFormat.Size];
            this.Framebuffer.PixelFormat.Encode(pixelFormat, 0);
            this.c.Send(pixelFormat);
            this.c.SendString(this.Framebuffer.Name, true);

            this.logger?.Log(LogLevel.Info, () => $"The desktop {this.Framebuffer.Name} has initialized with pixel format {this.clientPixelFormat}; the screen size is {this.clientWidth}x{this.clientHeight}");
        }

        private void HandleSetPixelFormat()
        {
            this.c.Receive(3);

            var pixelFormat = this.c.Receive(VncPixelFormat.Size);
            this.clientPixelFormat = VncPixelFormat.Decode(pixelFormat, 0);

            this.InitFramebufferEncoder();
        }

        private void HandleFramebufferUpdateRequest()
        {
            var incremental = this.c.ReceiveByte() != 0;
            var region = this.c.ReceiveRectangle();

            lock (this.FramebufferUpdateRequestLock)
            {
                this.logger?.Log(LogLevel.Debug, () => $"Received a FramebufferUpdateRequest command for {region}");

                region = VncRectangle.Intersect(region, new VncRectangle(0, 0, this.Framebuffer.Width, this.Framebuffer.Height));

                if (region.IsEmpty)
                {
                    return;
                }

                this.FramebufferUpdateRequest = new FramebufferUpdateRequest(incremental, region);
                this.FramebufferChanged();
            }
        }

        private void HandleKeyEvent()
        {
            var pressed = this.c.ReceiveByte() != 0;
            this.c.Receive(2);
            var keysym = (KeySym)this.c.ReceiveUInt32BE();

            this.OnKeyChanged(new KeyChangedEventArgs(keysym, pressed));
        }

        private void HandlePointerEvent()
        {
            int pressedButtons = this.c.ReceiveByte();
            int x = this.c.ReceiveUInt16BE();
            int y = this.c.ReceiveUInt16BE();

            this.OnPointerChanged(new PointerChangedEventArgs(x, y, pressedButtons));
        }

        private void HandleReceiveClipboardData()
        {
            this.c.Receive(3); // padding

            var clipboard = this.c.ReceiveString(0xffffff);

            this.OnRemoteClipboardChanged(new RemoteClipboardChangedEventArgs(clipboard));
        }

        private void AddRegion(VncRectangle region, VncEncoding encoding, byte[] contents)
        {
            this.fbuRectangles.Add(new Rectangle() { Region = region, Encoding = encoding, Contents = contents });

            // Avoid the overflow of updated rectangle count.
            // NOTE: EndUpdate may implicitly add one for desktop resizing.
            if (this.fbuRectangles.Count >= ushort.MaxValue - 1)
            {
                this.FramebufferManualEndUpdate();
                this.FramebufferManualBeginUpdate();
            }
        }

        private void InitFramebufferEncoder()
        {
            this.logger?.Log(LogLevel.Info, () => "Initializing the frame buffer encoder");

            // For RealVNC compatibility: make sure we only start using encodings _after_
            // the connection has been initialized correctly.
            if (this.IsConnected)
            {
                // Iterate over all encodings supported by the client. Select the first encoding
                // which is supported by this session.
                foreach (var encoding in this.clientEncoding)
                {
                    var encoder = this.Encoders.SingleOrDefault(c => c.Encoding == encoding);

                    if (encoder != null)
                    {
                        this.Encoder = encoder;
                        break;
                    }
                }
            }
            else
            {
                this.logger?.Log(LogLevel.Info, () => "Not selecting any encoder because the connection setup phase has not completed.");
            }

            this.logger?.Log(LogLevel.Info, () => $"Initialized the frame buffer encoder. Using {this.Encoder.GetType().Name} ({this.Encoder.Encoding})");
        }

        private struct Rectangle
        {
            public VncRectangle Region;
            public VncEncoding Encoding;
            public byte[] Contents;
        }
    }
}
