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

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// Represents the various fields of the <see cref="RfbScreenInfoPtr"/> struct.
    /// </summary>
    public enum RfbScreenInfoPtrField
    {
#pragma warning disable SA1602 // Enumeration items should be documented
        ScaledScreenNext,
        ScaledScreenRefCount,
        Width,
        PaddedWidthInBytes,
        Height,
        Depth,
        BitsPerPixel,
        SizeInBytes,
        BlackPixel,
        WhitePixel,
        ScreenData,
        ServerFormat,
        ColourMap,
        DesktopName,
        ThisHost,
        AutoPort,
        Port,
        ListenSock,
        MaxSock,
        MaxFd,
        AllFds,
        SocketState,
        InetdSock,
        InetdInitDone,
        UdpPort,
        UdpSock,
        UdpClient,
        UdpSockConnected,
        UdpRemoteAddr,
        MaxClientWait,
        HttpInitDone,
        HttpEnableProxyConnect,
        HttpPort,
        HttpDir,
        HttpListenSock,
        HttpSock,
        PasswordCheck,
        AuthPasswdData,
        AuthPasswdFirstViewOnly,
        MaxRectsPerUpdate,
        DeferUpdateTime,
        AlwaysShared,
        NeverShared,
        DontDisconnect,
        ClientHead,
        PointerClient,
        CursorX,
        CursorY,
        UnderCursorBufferLen,
        UnderCursorBuffer,
        DontConvertRichCursorToXCursor,
        Cursor,
        FrameBuffer,
        KbdAddEvent,
        KbdReleaseAllKeys,
        PtrAddEvent,
        SetXCutText,
        GetCursorPtr,
        SetTranslateFunction,
        SetSingleWindow,
        SetServerInput,
        GetFileTransferPermission,
        SetTextChat,
        NewClientHook,
        DisplayHook,
        GetKeyboardLedStateHook,
        CursorMutex,
        BackgroundLoop,
        IgnoreSIGPIPE,
        ProgressiveSliceHeight,
        ListenInterface,
        DeferPtrUpdateTime,
        HandleEventsEagerly,
        VersionString,
        ProtocolMajorVersion,
        ProtocolMinorVersion,
        PermitFileTransfer,
        DisplayFinishedHook,
        XvpHooka,
        Sslkeyfile,
        Sslcertfile,
        Ipv6port,
        Listen6Interface,
        Listen6Sock,
        Http6Port,
        HttpListen6Sock,
        SetDesktopSizeHook,
        NumberOfExtDesktopScreensHook,
        GetExtDesktopScreenHook,
        FdQuota,
#pragma warning restore SA1602 // Enumeration items should be documented
    }
}
