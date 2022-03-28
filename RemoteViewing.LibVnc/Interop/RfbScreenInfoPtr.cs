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
    /// Represents a VNC server.
    /// </summary>
    /// <seealso href="https://github.com/LibVNC/libvncserver/blob/master/rfb/rfb.h#L263"/>
    public unsafe partial class RfbScreenInfoPtr : SafeHandleMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RfbScreenInfoPtr"/> class.
        /// </summary>
        public RfbScreenInfoPtr()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RfbScreenInfoPtr"/> class,
        /// specifying whether the handle is to be reliably released.
        /// </summary>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to reliably release the handle during the finalization phase; <see langword="false"/> to prevent
        /// reliable release (not recommended).
        /// </param>
        public RfbScreenInfoPtr(bool ownsHandle)
            : base(ownsHandle)
        {
        }

        public RfbScreenInfoPtr(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Gets or sets a value indicating the number of clients for the current scaled screen.
        /// </summary>
        public int ScaledScreenRefCount
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the width of the screen, in pixels.
        /// </summary>
        public int Width
        {
            get { return NativeMethods.rfbScreenInfo_get_width(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_width(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets the aligned width of the screen, in bytes.
        /// </summary>
        public int PaddedWidthInBytes
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the height of the screen, in bytes.
        /// </summary>
        public int Height
        {
            get { return NativeMethods.rfbScreenInfo_get_height(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_height(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets the color depth.
        /// </summary>
        public int Depth
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the number of bits per pixel.
        /// </summary>
        public int BitsPerPixel
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the number of bytes per pixel.
        /// </summary>
        public int SizeInBytes
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the value for a black pixel.
        /// </summary>
        public uint BlackPixel
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the value for a white pixel.
        /// </summary>
        public uint WhitePixel
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a reference to a screen-specific data structure.
        /// </summary>
        public IntPtr ScreenData
        {
            get { return NativeMethods.rfbScreenInfo_get_screenData(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_screenData(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets the pixel format.
        /// </summary>
        public RfbPixelFormat ServerFormat
        {
            get { return NativeMethods.rfbScreenInfo_get_serverFormat(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_serverFormat(this.handle, value); }
        }

        /// <summary>
        /// Gets the name of the desktop.
        /// </summary>
        public string DesktopName
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the name of the current host.
        /// </summary>
        public string ThisHost
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use an auto-selected port.
        /// </summary>
        public bool AutoPort
        {
            get { return NativeMethods.rfbScreenInfo_get_autoPort(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_autoPort(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets the TCP port used.
        /// </summary>
        public int Port
        {
            get { return NativeMethods.rfbScreenInfo_get_port(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_port(this.handle, value); }
        }

        /// <summary>
        /// Gets the state of the TCP socket.
        /// </summary>
        public RfbSocketState SocketState
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbPasswordCheckProcPtr"/> delegate which is invoked
        /// when the client provides a password.
        /// </summary>
        public IntPtr PasswordCheck
        {
            get { return NativeMethods.rfbScreenInfo_get_passwordCheck(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_passwordCheck(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets a pointer to data which is passed to the password check. This is normally a list of passwords.
        /// The password check is not enforced if this value is <see cref="IntPtr.Zero"/>.
        /// </summary>
        public IntPtr AuthPasswdData
        {
            get { return NativeMethods.rfbScreenInfo_get_authPasswdData(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_authPasswdData(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first password in the <see cref="AuthPasswdData"/> list is
        /// a view-only password.
        /// </summary>
        public bool AuthPasswdFirstViewOnly
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the maximum number of rectangles to send in one update.
        /// </summary>
        public int MaxRectsPerUpdate
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the amount of time, in milliseconds, to wait before sending an update.
        /// </summary>
        public int DeferUpdateTime
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to always treat new clients as shared.
        /// </summary>
        public bool AlwaysShared
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to never treat new clients as shared.
        /// </summary>
        public bool NeverShared
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to not disconnect existing clients when a new non-shared
        /// connection comes in (and refuse the new connection instead).
        /// </summary>
        public bool DontDisconnect
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to the framebuffer.
        /// </summary>
        public IntPtr Framebuffer
        {
            get { return NativeMethods.rfbScreenInfo_get_frameBuffer(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_frameBuffer(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.RfbKbdAddEventProcPtr"/> delegate which is invoked
        /// when a keyboard event occurs.
        /// </summary>
        public IntPtr KbdAddEvent
        {
            get { return NativeMethods.rfbScreenInfo_get_kbdAddEvent(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_kbdAddEvent(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.release"/>.
        /// </summary>
        public IntPtr KbdReleaseAllKeys
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.RfbPtrAddEventProcPtr"/> delegate which is invoked
        /// when a mouse event occurs.
        /// </summary>
        public IntPtr PtrAddEvent
        {
            get { return NativeMethods.rfbScreenInfo_get_ptrAddEvent(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_ptrAddEvent(this.handle, value); }
        }

        // GetCursorPtr. This could be used to make an animated cursor.
        // SetTranslateFunction. If you insist on colour maps or something more obscure, you have to implement this. Default is a trueColour mapping.
        // SetSingleWindow. If x==1 and y==1 then set the whole display, else find the window underneath x and y and set the framebuffer to the dimensions of that window.

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbSetServerInputProcPtr"/> delegate which is invoked to determine
        /// whether the X11 server accepts input from the local user.
        /// </summary>
        public IntPtr SetServerInput
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbFileTransferPermitted"/> delegate which is invoked to determine
        /// whether file transfers are permitted.
        /// </summary>
        public IntPtr GetFileTransferPermission
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbSetTextChat"/> delegate which is invoked to handle
        /// textchat messages.
        /// </summary>
        public IntPtr SetTextChat
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.RfbNewClientHookPtr"/> delegate which is invoked when
        /// a new client connects.
        /// </summary>
        public IntPtr NewClientHook
        {
            get { return NativeMethods.rfbScreenInfo_get_newClientHook(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_newClientHook(this.handle, value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbDisplayHookPtr"/> delegate which is invoked
        /// just before a frame buffer update.
        /// </summary>
        public IntPtr DisplayHook
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbGetKeyboardLedStateHookPtr"/> delegate which is invoked
        /// when the server wants to determine the state of the caps/num/scroll leds of the server.
        /// </summary>
        public IntPtr GetKeyboardLedStateHook
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the IP address of the IPv4 interface on which to listen for new connections.
        /// </summary>
        public int ListenInterface
        {
            get { return NativeMethods.rfbScreenInfo_get_listenInterface(this.handle); }
            set { NativeMethods.rfbScreenInfo_set_listenInterface(this.handle, value); }
        }

        /// <summary>
        /// Gets the major version number of the RFB protocol in use.
        /// </summary>
        public int ProtocolMajorVersion
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the minor version number of the RFB protocol in use.
        /// </summary>
        public int ProtocolMinorVersion
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            NativeMethods.rfbScreenCleanup(this.handle);
            return true;
        }
    }
}
