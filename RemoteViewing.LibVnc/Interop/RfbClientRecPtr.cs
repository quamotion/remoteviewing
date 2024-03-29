﻿#region License
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

using Microsoft.Win32.SafeHandles;
using RemoteViewing.Vnc;
using System;
using System.Runtime.InteropServices;

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// Represents a client to a server session.
    /// </summary>
    /// <seealso href="https://github.com/LibVNC/libvncserver/blob/master/rfb/rfb.h#L479:L748"/>
    public partial class RfbClientRecPtr : SafeHandleMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RfbClientRecPtr"/> class.
        /// </summary>
        public RfbClientRecPtr()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RfbClientRecPtr"/> class,
        /// specifying whether the handle is to be reliably released.
        /// </summary>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to reliably release the handle during the finalization phase; <see langword="false"/> to prevent
        /// reliable release (not recommended).
        /// </param>
        public RfbClientRecPtr(bool ownsHandle)
            : base(ownsHandle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RfbClientRecPtr"/> class from a <see cref="IntPtr"/>,
        /// specifying whether the handle is to be reliably released.
        /// </summary>
        /// <param name="handle">
        /// The underlying handle.
        /// </param>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to reliably release the handle during the finalization phase; <see langword="false"/> to prevent
        /// reliable release (not recommended).
        /// </param>
        public RfbClientRecPtr(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets a back pointer to the screen.
        /// </summary>
        public RfbScreenInfoPtr Screen
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a back pointer to a scaled version of the screen.
        /// </summary>
        public RfbScreenInfoPtr ScaledScreen
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the client requested a screen change in ultra style or palm style.
        /// </summary>
        public bool PalmVNC
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to application-specific client data.
        /// </summary>
        public IntPtr ClientData
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.ClientGoneHookPtr"/> delegate which is invoked
        /// when the client disconnects.
        /// </summary>
        public IntPtr ClientGoneHook
        {
            get { return NativeMethods.rfbClient_get_clientGoneHook(this.handle); }
            set { NativeMethods.rfbClient_set_clientGoneHook(this.handle, value); }
        }

        /// <summary>
        /// Gets the name or IP address of the remote host.
        /// </summary>
        public unsafe string Host
        {
            get { return new string((sbyte*)NativeMethods.rfbClient_get_host(this.handle)); }
        }

        /// <summary>
        /// Gets the major version number of the RFB protocol in use by the client.
        /// </summary>
        public int ProtocolMajorVersion
        {
            get { return NativeMethods.rfbClient_get_protocolMajorVersion(this.handle); }
        }

        /// <summary>
        /// Gets the minor version number of the RFB protocol in use by the client.
        /// </summary>
        public int ProtocolMinorVersion
        {
            get { return NativeMethods.rfbClient_get_protocolMinorVersion(this.handle); }
        }

        /// <summary>
        /// Gets the state of the client.
        /// </summary>
        public RfbClientRecState State
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the client is reverse connection.
        /// </summary>
        public bool ReverseConnection
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the client is on hold.
        /// </summary>
        public bool OnHold
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the client is ready to set colour map entries.
        /// </summary>
        public bool ReadyForSetColourMapEntries
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the client is using Copy Rect encoding.
        /// </summary>
        public bool UseCopyRect
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the preferred client encoding.
        /// </summary>
        public VncEncoding PreferredEncoding
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the maximum width of a CoRRE-encoded rectangle.
        /// </summary>
        public int CorreMaxWidth
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the maximum height of a CoRRE-encoded rectangle.
        /// </summary>
        public int CorreMaxHeight
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the client is in view-only mode.
        /// </summary>
        public bool ViewOnly
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the authentication challenge sent to the client.
        /// </summary>
        /// <remarks>
        /// This value is only used during VNC authentication.
        /// </remarks>
        public unsafe Span<byte> AuthChallenge
        {
            get
            {
                var authChallenge = new byte[16];

                fixed (byte* authChallengePtr = authChallenge)
                {
                    NativeMethods.rfbClient_get_authChallenge(this.handle, authChallengePtr);
                }

                return authChallenge;
            }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.ClientFramebufferUpdateRequestHookPtr"/> delegate which is invoked
        /// when the client requests a framebuffer update.
        /// </summary>
        public IntPtr ClientFramebufferUpdateRequestHook
        {
            get
            {
                if (!NativeMethods.IsVersion_0_9_13_OrNewer)
                {
                    throw new InvalidOperationException("ClientFramebufferUpdateRequestHook is available on libvncserver 0.9.13 or newer only.");
                }

                return NativeMethods.rfbClient_get_clientFramebufferUpdateRequestHook(this.handle);
            }

            set
            {
                if (!NativeMethods.IsVersion_0_9_13_OrNewer)
                {
                    throw new InvalidOperationException("ClientFramebufferUpdateRequestHook is available on libvncserver 0.9.13 or newer only.");
                }

                NativeMethods.rfbClient_set_clientFramebufferUpdateRequestHook(this.handle, value);
            }
        }

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            NativeMethods.rfbScreenCleanup(this.handle);
            return true;
        }
    }
}
