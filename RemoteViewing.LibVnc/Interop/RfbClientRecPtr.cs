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

using Microsoft.Win32.SafeHandles;
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
        /// Gets or sets a pointer to a <see cref="NativeMethods.ClientGoneHookPtr"/> delegate which is invoked
        /// when the client disconnects.
        /// </summary>
        public IntPtr ClientGoneHook
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbClientRecPtrField.ClientGoneHook]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbClientRecPtrField.ClientGoneHook], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.ClientFramebufferUpdateRequestHookPtr"/> delegate which is invoked
        /// when the client requests a framebuffer update.
        /// </summary>
        public IntPtr ClientFramebufferUpdateRequestHook
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbClientRecPtrField.ClientFramebufferUpdateRequestHook]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbClientRecPtrField.ClientFramebufferUpdateRequestHook], value); }
        }

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            NativeMethods.rfbScreenCleanup(this.handle);
            return true;
        }
    }
}
