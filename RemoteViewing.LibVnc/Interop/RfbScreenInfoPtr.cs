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

        /// <summary>
        /// Gets or sets a value indicating the number of clients for the current scaled screen.
        /// </summary>
        public int ScaledScreenRefCount
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount], value); }
        }

        /// <summary>
        /// Gets or sets the width of the screen, in pixels.
        /// </summary>
        public int Width
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.Width]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.Width], value); }
        }

        /// <summary>
        /// Gets or sets the aligned width of the screen, in bytes.
        /// </summary>
        public int PaddedWidthInBytes
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes], value); }
        }

        /// <summary>
        /// Gets or sets the height of the screen, in bytes.
        /// </summary>
        public int Height
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.Height]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.Height], value); }
        }

        /// <summary>
        /// Gets or sets the color depth.
        /// </summary>
        public int Depth
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.Depth]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.Depth], value); }
        }

        /// <summary>
        /// Gets or sets the number of bits per pixel.
        /// </summary>
        public int BitsPerPixel
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.BitsPerPixel]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.BitsPerPixel], value); }
        }

        /// <summary>
        /// Gets or sets the number of bytes per pixel.
        /// </summary>
        public int SizeInBytes
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.SizeInBytes]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.SizeInBytes], value); }
        }

        /// <summary>
        /// Gets or sets the value for a black pixel.
        /// </summary>
        public uint BlackPixel
        {
            get { return (uint)Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.BlackPixel]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.BlackPixel], (int)value); }
        }

        /// <summary>
        /// Gets or sets the value for a white pixel.
        /// </summary>
        public uint WhitePixel
        {
            get { return (uint)Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.WhitePixel]); }
            set { Marshal.WriteInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.WhitePixel], (int)value); }
        }

        /// <summary>
        /// Gets or sets a reference to a screen-specific data structure.
        /// </summary>
        public IntPtr ScreenData
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.ScreenData]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.ScreenData], value); }
        }

        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        public RfbPixelFormat ServerFormat
        {
            get { return Marshal.PtrToStructure<RfbPixelFormat>(this.handle + FieldOffsets[(int)RfbScreenInfoPtrField.ServerFormat]); }
        }

        /// <summary>
        /// Gets the name of the desktop.
        /// </summary>
        public string DesktopName
        {
            get
            {
                var desktopName = (sbyte**)((sbyte*)this.handle.ToPointer() + FieldOffsets[(int)RfbScreenInfoPtrField.DesktopName]);
                return new string(*desktopName);
            }
        }

        /// <summary>
        /// Gets the name of the current host.
        /// </summary>
        public string ThisHost
        {
            get
            {
                sbyte* thisName = (sbyte*)this.handle.ToPointer() + FieldOffsets[(int)RfbScreenInfoPtrField.ThisHost];
                return new string(thisName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use an auto-selected port.
        /// </summary>
        public bool AutoPort
        {
            get { return this.GetBool(RfbScreenInfoPtrField.AutoPort); }
            set { this.SetBool(RfbScreenInfoPtrField.AutoPort, value); }
        }

        /// <summary>
        /// Gets the TCP port used.
        /// </summary>
        public int Port
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.Port]); }
        }

        /// <summary>
        /// Gets the state of the TCP socket.
        /// </summary>
        public RfbSocketState SocketState
        {
            get { return (RfbSocketState)Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.SocketState]); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbPasswordCheckProcPtr"/> delegate which is invoked
        /// when the client provides a password.
        /// </summary>
        public IntPtr PasswordCheck
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.PasswordCheck]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.PasswordCheck], value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to always treat new clients as shared.
        /// </summary>
        public bool AlwaysShared
        {
            get { return this.GetBool(RfbScreenInfoPtrField.AlwaysShared); }
            set { this.SetBool(RfbScreenInfoPtrField.AlwaysShared, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to never treat new clients as shared.
        /// </summary>
        public bool NeverShared
        {
            get { return this.GetBool(RfbScreenInfoPtrField.NeverShared); }
            set { this.SetBool(RfbScreenInfoPtrField.NeverShared, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to not disconnect existing clients when a new non-shared
        /// connection comes in (and refuse the new connection instead).
        /// </summary>
        public bool DontDisconnect
        {
            get { return this.GetBool(RfbScreenInfoPtrField.DontDisconnect); }
            set { this.SetBool(RfbScreenInfoPtrField.DontDisconnect, value); }
        }

        /// <summary>
        /// Gets or sets a pointer to the framebuffer.
        /// </summary>
        public IntPtr Framebuffer
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.FrameBuffer]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.FrameBuffer], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.RfbKbdAddEventProcPtr"/> delegate which is invoked
        /// when a keyboard event occurs.
        /// </summary>
        public IntPtr KbdAddEvent
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.KbdAddEvent]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.KbdAddEvent], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.release"/>.
        /// </summary>
        public IntPtr KbdReleaseAllKeys
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.RfbPtrAddEventProcPtr"/> delegate which is invoked
        /// when a mouse event occurs.
        /// </summary>
        public IntPtr PtrAddEvent
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.PtrAddEvent]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.PtrAddEvent], value); }
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
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.SetServerInput]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.SetServerInput], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbFileTransferPermitted"/> delegate which is invoked to determine
        /// whether file transfers are permitted.
        /// </summary>
        public IntPtr GetFileTransferPermission
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbSetTextChat"/> delegate which is invoked to handle
        /// textchat messages.
        /// </summary>
        public IntPtr SetTextChat
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.SetTextChat]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.SetTextChat], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.RfbNewClientHookPtr"/> delegate which is invoked when
        /// a new client connects.
        /// </summary>
        public IntPtr NewClientHook
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.NewClientHook]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.NewClientHook], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbDisplayHookPtr"/> delegate which is invoked
        /// just before a frame buffer update.
        /// </summary>
        public IntPtr DisplayHook
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.DisplayHook]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.DisplayHook], value); }
        }

        /// <summary>
        /// Gets or sets a pointer to a <see cref="NativeMethods.rfbGetKeyboardLedStateHookPtr"/> delegate which is invoked
        /// when the server wants to determine the state of the caps/num/scroll leds of the server.
        /// </summary>
        public IntPtr GetKeyboardLedStateHook
        {
            get { return Marshal.ReadIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook]); }
            set { Marshal.WriteIntPtr(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook], value); }
        }

        /// <summary>
        /// Gets the major version number of the RFB protocol in use.
        /// </summary>
        public int ProtocolMajorVersion
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.ProtocolMajorVersion]); }
        }

        /// <summary>
        /// Gets the minor version number of the RFB protocol in use.
        /// </summary>
        public int ProtocolMinorVersion
        {
            get { return Marshal.ReadInt32(this.handle, FieldOffsets[(int)RfbScreenInfoPtrField.ProtocolMinorVersion]); }
        }

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            NativeMethods.rfbScreenCleanup(this.handle);
            return true;
        }

        private bool GetBool(RfbScreenInfoPtrField field)
        {
            return Marshal.ReadByte(this.handle, FieldOffsets[(int)field]) == 1;
        }

        private void SetBool(RfbScreenInfoPtrField field, bool value)
        {
            Marshal.WriteByte(this.handle, FieldOffsets[(int)field], value ? (byte)1 : (byte)0);
        }
    }
}
