#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2020 Quamotion bvba
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

using Microsoft.Extensions.Logging;
using RemoteViewing.LibVnc.Interop;
using RemoteViewing.Vnc;
using RemoteViewing.Vnc.Server;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace RemoteViewing.LibVnc
{
    public unsafe class LibVncServer : IVncServer
    {
        private readonly ILogger logger;
        private readonly IVncPasswordChallenge passwordChallenge;

        private readonly IVncFramebufferSource fbSource;
        private readonly IVncRemoteController controller;
        private readonly IVncRemoteKeyboard keyboard;

        private readonly NativeMethods.RfbNewClientHookPtr newClientHook;
        private readonly NativeMethods.ClientGoneHookPtr clientGoneHook;
        private readonly NativeMethods.ClientFramebufferUpdateRequestHookPtr clientFramebufferUpdateRequestHook;
        private readonly NativeMethods.RfbKbdAddEventProcPtr rfbKbdAddEventHook;
        private readonly NativeMethods.RfbPtrAddEventProcPtr rfbPtrAddEventProc;
        private readonly NativeMethods.rfbPasswordCheckProcPtr rfbPasswordCheckProc;

        private readonly IntPtr newClientHookPtr;
        private readonly IntPtr clientGoneHookPtr;
        private readonly IntPtr clientFramebufferUpdateRequestHookPtr;
        private readonly IntPtr rfbKbdAddEventHookPtr;
        private readonly IntPtr rfbPtrAddEventProcPtr;
        private readonly IntPtr rfbPasswordCheckProcPtr;

        private readonly MemoryPool<byte> memoryPool = MemoryPool<byte>.Shared;
        private IMemoryOwner<byte> currentFramebuffer = null;
        private MemoryHandle currentFramebufferHandle = default;

        private Thread mainLoop;
        private bool isRunning = false;

        private RfbScreenInfoPtr server;

        public LibVncServer(IVncFramebufferSource framebufferSource, IVncRemoteKeyboard keyboard, IVncRemoteController controller, ILogger logger)
        {
            this.fbSource = framebufferSource ?? throw new ArgumentNullException(nameof(framebufferSource));
            this.keyboard = keyboard;
            this.controller = controller;

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.passwordChallenge = new VncPasswordChallenge();

            this.newClientHook = new NativeMethods.RfbNewClientHookPtr(this.RfbNewClientHook);
            this.clientGoneHook = new NativeMethods.ClientGoneHookPtr(this.ClientGoneHook);
            this.clientFramebufferUpdateRequestHook = new NativeMethods.ClientFramebufferUpdateRequestHookPtr(this.ClientFramebufferUpdateRequestHook);
            this.rfbKbdAddEventHook = new NativeMethods.RfbKbdAddEventProcPtr(this.RfbKbdAddEventHook);
            this.rfbPtrAddEventProc = new NativeMethods.RfbPtrAddEventProcPtr(this.RfbPtrAddEventHook);
            this.rfbPasswordCheckProc = new NativeMethods.rfbPasswordCheckProcPtr(this.RfbPasswordCheckHook);

            this.newClientHookPtr = Marshal.GetFunctionPointerForDelegate(this.newClientHook);
            this.clientGoneHookPtr = Marshal.GetFunctionPointerForDelegate(this.clientGoneHook);
            this.clientFramebufferUpdateRequestHookPtr = Marshal.GetFunctionPointerForDelegate(this.clientFramebufferUpdateRequestHook);
            this.rfbKbdAddEventHookPtr = Marshal.GetFunctionPointerForDelegate(this.rfbKbdAddEventHook);
            this.rfbPtrAddEventProcPtr = Marshal.GetFunctionPointerForDelegate(this.rfbPtrAddEventProc);
            this.rfbPasswordCheckProcPtr = Marshal.GetFunctionPointerForDelegate(this.rfbPasswordCheckProc);

#if !NETSTANDARD2_0 && !NET45
            NativeLogging.Logger = logger;
#endif
        }

        /// <inheritdoc/>
        public event EventHandler Connected;

        /// <inheritdoc/>
        public event EventHandler Closed;

        /// <inheritdoc/>
        public event EventHandler<PasswordProvidedEventArgs> PasswordProvided;

        /// <inheritdoc/>
        public IReadOnlyList<IVncServerSession> Sessions => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Start(IPEndPoint endPoint)
        {
            if (endPoint == null || endPoint.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentOutOfRangeException(nameof(endPoint));
            }

            var fb = this.fbSource.Capture();

            this.server = NativeMethods.rfbGetScreen(fb.Width, fb.Height, fb.PixelFormat.BlueBits, 3, fb.PixelFormat.BytesPerPixel);

            var serverFormat = this.server.ServerFormat;
            serverFormat.RedShift = 16;
            serverFormat.GreenShift = 8;
            serverFormat.BlueShift = 0;

            this.server.ServerFormat = serverFormat;

            this.server.ListenInterface = MemoryMarshal.Read<int>(endPoint.Address.GetAddressBytes());
            this.server.AutoPort = false;
            this.server.Port = endPoint.Port;

            this.currentFramebuffer = this.memoryPool.Rent(fb.Width * fb.Height * 4);
            this.currentFramebufferHandle = this.currentFramebuffer.Memory.Pin();

            fb.GetBuffer().CopyTo(this.currentFramebuffer.Memory);

            this.server.Framebuffer = new IntPtr(this.currentFramebufferHandle.Pointer);
            this.server.KbdAddEvent = this.rfbKbdAddEventHookPtr;
            this.server.PtrAddEvent = this.rfbPtrAddEventProcPtr;
            this.server.NewClientHook = this.newClientHookPtr;
            this.server.AuthPasswdData = new IntPtr(-1);
            this.server.PasswordCheck = this.rfbPasswordCheckProcPtr;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                NativeMethods.rfbInitServerWithoutPthreadsButWithZRLE(this.server);
            }
            else
            {
                NativeMethods.rfbInitServerWithPthreadsAndZRLE(this.server);
            }

            this.mainLoop = new Thread(new ThreadStart(this.MainLoop));
            this.mainLoop.Start();
        }

        /// <inheritdoc/>
        public void Stop() => this.Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.mainLoop != null)
            {
                this.isRunning = false;
                this.mainLoop.Join();
                this.mainLoop = null;
            }

            if (this.server != null)
            {
                this.server.Dispose();
                this.server = null;
            }
        }

        protected void MainLoop()
        {
            long usec = +1;
            if (usec < 0)
            {
                usec = this.server.DeferUpdateTime * 1000;
            }

            this.isRunning = true;

            while (NativeMethods.rfbIsActive(this.server) != 0 && this.isRunning)
            {
                Stopwatch s = new Stopwatch();
                s.Reset();
                s.Start();
                this.UpdateFramebuffer(this.server);
                s.Stop();
                this.logger.LogDebug($"Updated framebuffer. Took {s.ElapsedMilliseconds} ms.");

                s.Reset();
                s.Start();
                NativeMethods.rfbProcessEvents(this.server, usec);
                s.Stop();
                this.logger.LogDebug($"Processed server events. Took {s.ElapsedMilliseconds} ms.");
            }

            this.currentFramebufferHandle.Dispose();
            this.currentFramebuffer.Dispose();
        }

        private void UpdateFramebuffer(RfbScreenInfoPtr server)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            var fb = this.fbSource.Capture();

            if (fb.Width != server.Width || fb.Height != server.Height)
            {
                // TODO: This should only be necessary if the current framebuffer is not large enough.
                var oldFramebufferHandle = this.currentFramebufferHandle;
                var oldFramebuffer = this.currentFramebuffer;

                this.currentFramebuffer = this.memoryPool.Rent(fb.Width * fb.Height * 4);
                this.currentFramebufferHandle = this.currentFramebuffer.Memory.Pin();

                if (fb.PixelFormat != VncPixelFormat.RGB32)
                {
                    this.logger.LogWarning($"The pixel format {fb.PixelFormat} is not supported");
                }

                fb.GetBuffer().CopyTo(this.currentFramebuffer.Memory);

                NativeMethods.rfbNewFramebuffer(server, this.currentFramebufferHandle.Pointer, fb.Width, fb.Height, fb.PixelFormat.BlueBits, 3, fb.PixelFormat.BytesPerPixel);

                oldFramebufferHandle.Dispose();
                oldFramebuffer.Dispose();
            }
            else
            {
                fb.GetBuffer().CopyTo(this.currentFramebuffer.Memory);
                NativeMethods.rfbMarkRectAsModified(server, 0, 0, fb.Width, fb.Height);
            }
        }

        protected virtual RfbNewClientAction RfbNewClientHook(IntPtr cl)
        {
            RfbClientRecPtr client = new RfbClientRecPtr(cl, false);
            this.logger.LogInformation("A new client connected");

            var host = client.Host;
            var major = client.ProtocolMajorVersion;
            var minor = client.ProtocolMinorVersion;

            client.ClientGoneHook = this.clientGoneHookPtr;

            if (NativeMethods.IsVersion_0_9_13_OrNewer)
            {
                client.ClientFramebufferUpdateRequestHook = this.clientFramebufferUpdateRequestHookPtr;
            }

            this.Connected?.Invoke(this, EventArgs.Empty);

            return RfbNewClientAction.RFB_CLIENT_ACCEPT;
        }

        protected virtual void ClientFramebufferUpdateRequestHook(IntPtr cl, IntPtr furMsg)
        {
            RfbClientRecPtr client = new RfbClientRecPtr(cl, false);
            this.logger.LogInformation("Framebuffer update requested");
        }

        protected virtual void ClientGoneHook(IntPtr cl)
        {
            RfbClientRecPtr client = new RfbClientRecPtr(cl, false);
            this.logger.LogInformation("A client disconnected.");

            this.Closed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RfbKbdAddEventHook(byte down, KeySym keySym, IntPtr cl)
        {
            RfbClientRecPtr client = new RfbClientRecPtr(cl, false);
            this.logger.LogInformation($"Pressed button {keySym}");

            this.keyboard?.HandleKeyEvent(
                this,
                new KeyChangedEventArgs(
                    keysym: keySym,
                    pressed: down == 1));
        }

        protected void RfbPtrAddEventHook(int buttonMask, int x, int y, IntPtr cl)
        {
            RfbClientRecPtr client = new RfbClientRecPtr(cl, false);
            this.logger.LogInformation($"Mouse event at ({x},{y})");

            this.controller?.HandleTouchEvent(
                this,
                new PointerChangedEventArgs(x, y, buttonMask));
        }

        protected bool RfbPasswordCheckHook(IntPtr cl, sbyte* encryptedPassWord, int len)
        {
            RfbClientRecPtr client = new RfbClientRecPtr(cl, false);
            var response = new Span<byte>((void*)encryptedPassWord, len);
            var challenge = client.AuthChallenge;

            Span<byte> expectedResponse = null;

            var e = new PasswordProvidedEventArgs(
                this.passwordChallenge,
                challenge.ToArray(),
                response.ToArray());

            this.PasswordProvided?.Invoke(this, e);

            return e.IsAuthenticated;
        }
    }
}
