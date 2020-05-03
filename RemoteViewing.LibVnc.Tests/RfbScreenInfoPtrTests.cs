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

using RemoteViewing.LibVnc.Interop;
using System;
using System.Runtime.InteropServices;
using Xunit;

namespace RemoteViewing.LibVnc.Tests
{
    /// <summary>
    /// Tests the <see cref="RfbScreenInfoPtr"/> class.
    /// </summary>
    public unsafe class RfbScreenInfoPtrTests
    {
        // You can use the following regex to transform output of get_offset.c to xUnit assert statements:
        //   offsetof\(rfbScreenInfo, ([a-zA-Z_]*)\) : (\d*)
        //   Assert.Equal($2, offsets[(int)RfbScreenInfoPtrField.$1]);

        /// <summary>
        /// Tests the basic layout of the <see cref="RfbScreenInfoPtr"/> class by accessing most properties.
        /// </summary>
        [Fact]
        public void LayoutTest()
        {
            using (RfbScreenInfoPtr server = NativeMethods.rfbGetScreen(400, 300, 8, 3, 4))
            {
                Assert.Equal(0, server.ScaledScreenRefCount);
                Assert.Equal(400, server.Width);
                Assert.Equal(4 * 400, server.PaddedWidthInBytes);
                Assert.Equal(300, server.Height);
                Assert.Equal(32, server.Depth);
                Assert.Equal(32, server.BitsPerPixel);

                Assert.Equal(0, server.SizeInBytes);
                Assert.Equal(0x000000u, server.BlackPixel);
                Assert.Equal(0x000000u, server.WhitePixel);

                Assert.Equal(IntPtr.Zero, server.ScreenData);

                var pixelFormat = server.ServerFormat;
                Assert.Equal(32, pixelFormat.BitsPerPixel);
                Assert.Equal(32, pixelFormat.Depth);
                Assert.Equal(0, pixelFormat.BigEndian);
                Assert.Equal(1, pixelFormat.TrueColour);
                Assert.Equal(255, pixelFormat.RedMax);
                Assert.Equal(255, pixelFormat.GreenMax);
                Assert.Equal(255, pixelFormat.BlueMax);
                Assert.Equal(0, pixelFormat.RedShift);
                Assert.Equal(8, pixelFormat.GreenShift);
                Assert.Equal(16, pixelFormat.BlueShift);

                Assert.Equal("LibVNCServer", server.DesktopName);
                Assert.Equal(Environment.MachineName, server.ThisHost);
                Assert.False(server.AutoPort);
                Assert.Equal(5900, server.Port);

                Assert.Equal(RfbSocketState.RFB_SOCKET_INIT, server.SocketState);

                Assert.Equal(3, server.ProtocolMajorVersion);
                Assert.Equal(8, server.ProtocolMinorVersion);

                Assert.Equal(IntPtr.Zero, server.DisplayHook);
                Assert.Equal(IntPtr.Zero, server.Framebuffer);
                Assert.Equal(IntPtr.Zero, server.GetFileTransferPermission);
                Assert.Equal(IntPtr.Zero, server.GetKeyboardLedStateHook);
                Assert.NotEqual(IntPtr.Zero, server.KbdAddEvent); // rfbDefaultKbdAddEvent
                Assert.NotEqual(IntPtr.Zero, server.KbdReleaseAllKeys); // rfbDoNothingWithClient
                Assert.NotEqual(IntPtr.Zero, server.NewClientHook); // rfbDefaultNewClientHook
                Assert.NotEqual(IntPtr.Zero, server.PtrAddEvent); // rfbDefaultPtrAddEvent
                Assert.Equal(IntPtr.Zero, server.ScreenData);
                Assert.Equal(IntPtr.Zero, server.SetServerInput);
                Assert.Equal(IntPtr.Zero, server.SetTextChat);

                Assert.False(server.IsClosed);
                Assert.False(server.IsInvalid);
            }
        }

        /// <summary>
        /// Tests the <see cref="RfbScreenInfoPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 64-bit Windows.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_Win64_Pthread()
        {
            var offsets = RfbScreenInfoPtr.GetFieldOffsets(
                OSPlatform.Windows,
                is64Bit: true,
                new NativeCapabilities()
                {
                    HaveLibPthread = true,
                });

            Assert.Equal(0, offsets[(int)RfbScreenInfoPtrField.ScaledScreenNext]);
            Assert.Equal(8, offsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount]);
            Assert.Equal(12, offsets[(int)RfbScreenInfoPtrField.Width]);
            Assert.Equal(16, offsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes]);
            Assert.Equal(20, offsets[(int)RfbScreenInfoPtrField.Height]);
            Assert.Equal(24, offsets[(int)RfbScreenInfoPtrField.Depth]);
            Assert.Equal(28, offsets[(int)RfbScreenInfoPtrField.BitsPerPixel]);
            Assert.Equal(32, offsets[(int)RfbScreenInfoPtrField.SizeInBytes]);
            Assert.Equal(36, offsets[(int)RfbScreenInfoPtrField.BlackPixel]);
            Assert.Equal(40, offsets[(int)RfbScreenInfoPtrField.WhitePixel]);
            Assert.Equal(48, offsets[(int)RfbScreenInfoPtrField.ScreenData]);
            Assert.Equal(56, offsets[(int)RfbScreenInfoPtrField.ServerFormat]);
            Assert.Equal(72, offsets[(int)RfbScreenInfoPtrField.ColourMap]);
            Assert.Equal(88, offsets[(int)RfbScreenInfoPtrField.DesktopName]);
            Assert.Equal(96, offsets[(int)RfbScreenInfoPtrField.ThisHost]);
            Assert.Equal(351, offsets[(int)RfbScreenInfoPtrField.AutoPort]);
            Assert.Equal(352, offsets[(int)RfbScreenInfoPtrField.Port]);
            Assert.Equal(360, offsets[(int)RfbScreenInfoPtrField.ListenSock]);
            Assert.Equal(368, offsets[(int)RfbScreenInfoPtrField.MaxSock]);
            Assert.Equal(372, offsets[(int)RfbScreenInfoPtrField.MaxFd]);
            Assert.Equal(376, offsets[(int)RfbScreenInfoPtrField.AllFds]);
            Assert.Equal(896, offsets[(int)RfbScreenInfoPtrField.SocketState]);
            Assert.Equal(904, offsets[(int)RfbScreenInfoPtrField.InetdSock]);
            Assert.Equal(912, offsets[(int)RfbScreenInfoPtrField.InetdInitDone]);
            Assert.Equal(916, offsets[(int)RfbScreenInfoPtrField.UdpPort]);
            Assert.Equal(920, offsets[(int)RfbScreenInfoPtrField.UdpSock]);
            Assert.Equal(928, offsets[(int)RfbScreenInfoPtrField.UdpClient]);
            Assert.Equal(936, offsets[(int)RfbScreenInfoPtrField.UdpSockConnected]);
            Assert.Equal(940, offsets[(int)RfbScreenInfoPtrField.UdpRemoteAddr]);
            Assert.Equal(956, offsets[(int)RfbScreenInfoPtrField.MaxClientWait]);
            Assert.Equal(960, offsets[(int)RfbScreenInfoPtrField.HttpInitDone]);
            Assert.Equal(961, offsets[(int)RfbScreenInfoPtrField.HttpEnableProxyConnect]);
            Assert.Equal(964, offsets[(int)RfbScreenInfoPtrField.HttpPort]);
            Assert.Equal(968, offsets[(int)RfbScreenInfoPtrField.HttpDir]);
            Assert.Equal(976, offsets[(int)RfbScreenInfoPtrField.HttpListenSock]);
            Assert.Equal(984, offsets[(int)RfbScreenInfoPtrField.HttpSock]);
            Assert.Equal(992, offsets[(int)RfbScreenInfoPtrField.PasswordCheck]);
            Assert.Equal(1000, offsets[(int)RfbScreenInfoPtrField.AuthPasswdData]);
            Assert.Equal(1008, offsets[(int)RfbScreenInfoPtrField.AuthPasswdFirstViewOnly]);
            Assert.Equal(1016, offsets[(int)RfbScreenInfoPtrField.DeferUpdateTime]);
            Assert.Equal(1020, offsets[(int)RfbScreenInfoPtrField.AlwaysShared]);
            Assert.Equal(1021, offsets[(int)RfbScreenInfoPtrField.NeverShared]);
            Assert.Equal(1022, offsets[(int)RfbScreenInfoPtrField.DontDisconnect]);
            Assert.Equal(1024, offsets[(int)RfbScreenInfoPtrField.ClientHead]);
            Assert.Equal(1032, offsets[(int)RfbScreenInfoPtrField.PointerClient]);
            Assert.Equal(1040, offsets[(int)RfbScreenInfoPtrField.CursorX]);
            Assert.Equal(1044, offsets[(int)RfbScreenInfoPtrField.CursorY]);
            Assert.Equal(1048, offsets[(int)RfbScreenInfoPtrField.UnderCursorBufferLen]);
            Assert.Equal(1056, offsets[(int)RfbScreenInfoPtrField.UnderCursorBuffer]);
            Assert.Equal(1064, offsets[(int)RfbScreenInfoPtrField.DontConvertRichCursorToXCursor]);
            Assert.Equal(1072, offsets[(int)RfbScreenInfoPtrField.Cursor]);
            Assert.Equal(1080, offsets[(int)RfbScreenInfoPtrField.FrameBuffer]);
            Assert.Equal(1088, offsets[(int)RfbScreenInfoPtrField.KbdAddEvent]);
            Assert.Equal(1096, offsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys]);
            Assert.Equal(1104, offsets[(int)RfbScreenInfoPtrField.PtrAddEvent]);
            Assert.Equal(1112, offsets[(int)RfbScreenInfoPtrField.SetXCutText]);
            Assert.Equal(1120, offsets[(int)RfbScreenInfoPtrField.GetCursorPtr]);
            Assert.Equal(1128, offsets[(int)RfbScreenInfoPtrField.SetTranslateFunction]);
            Assert.Equal(1136, offsets[(int)RfbScreenInfoPtrField.SetSingleWindow]);
            Assert.Equal(1144, offsets[(int)RfbScreenInfoPtrField.SetServerInput]);
            Assert.Equal(1152, offsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission]);
            Assert.Equal(1160, offsets[(int)RfbScreenInfoPtrField.SetTextChat]);
            Assert.Equal(1168, offsets[(int)RfbScreenInfoPtrField.NewClientHook]);
            Assert.Equal(1176, offsets[(int)RfbScreenInfoPtrField.DisplayHook]);
            Assert.Equal(1184, offsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook]);
            Assert.Equal(1192, offsets[(int)RfbScreenInfoPtrField.CursorMutex]);
            Assert.Equal(1200, offsets[(int)RfbScreenInfoPtrField.BackgroundLoop]);
            Assert.Equal(1201, offsets[(int)RfbScreenInfoPtrField.IgnoreSIGPIPE]);
            Assert.Equal(1204, offsets[(int)RfbScreenInfoPtrField.ProgressiveSliceHeight]);
            Assert.Equal(1208, offsets[(int)RfbScreenInfoPtrField.ListenInterface]);
            Assert.Equal(1212, offsets[(int)RfbScreenInfoPtrField.DeferPtrUpdateTime]);
            Assert.Equal(1216, offsets[(int)RfbScreenInfoPtrField.HandleEventsEagerly]);
            Assert.Equal(1224, offsets[(int)RfbScreenInfoPtrField.VersionString]);
            Assert.Equal(1232, offsets[(int)RfbScreenInfoPtrField.ProtocolMajorVersion]);
            Assert.Equal(1236, offsets[(int)RfbScreenInfoPtrField.ProtocolMinorVersion]);
            Assert.Equal(1240, offsets[(int)RfbScreenInfoPtrField.PermitFileTransfer]);
            Assert.Equal(1248, offsets[(int)RfbScreenInfoPtrField.DisplayFinishedHook]);
            Assert.Equal(1256, offsets[(int)RfbScreenInfoPtrField.XvpHooka]);
            Assert.Equal(1264, offsets[(int)RfbScreenInfoPtrField.Sslkeyfile]);
            Assert.Equal(1272, offsets[(int)RfbScreenInfoPtrField.Sslcertfile]);
            Assert.Equal(1280, offsets[(int)RfbScreenInfoPtrField.Ipv6port]);
            Assert.Equal(1288, offsets[(int)RfbScreenInfoPtrField.Listen6Interface]);
            Assert.Equal(1296, offsets[(int)RfbScreenInfoPtrField.Listen6Sock]);
            Assert.Equal(1304, offsets[(int)RfbScreenInfoPtrField.Http6Port]);
            Assert.Equal(1312, offsets[(int)RfbScreenInfoPtrField.HttpListen6Sock]);
            Assert.Equal(1320, offsets[(int)RfbScreenInfoPtrField.SetDesktopSizeHook]);
            Assert.Equal(1328, offsets[(int)RfbScreenInfoPtrField.NumberOfExtDesktopScreensHook]);
            Assert.Equal(1336, offsets[(int)RfbScreenInfoPtrField.GetExtDesktopScreenHook]);
            Assert.Equal(1344, offsets[(int)RfbScreenInfoPtrField.FdQuota]);
        }

        [Fact]
        public void GetFieldOffsetstest_Win64()
        {
            var offsets = RfbScreenInfoPtr.GetFieldOffsets(
                OSPlatform.Windows,
                is64Bit: true,
                new NativeCapabilities()
                {
                    HaveLibPthread = false,
                    HaveWin32Threads = true,
                });

            Assert.Equal(0, offsets[(int)RfbScreenInfoPtrField.ScaledScreenNext]);
            Assert.Equal(8, offsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount]);
            Assert.Equal(12, offsets[(int)RfbScreenInfoPtrField.Width]);
            Assert.Equal(16, offsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes]);
            Assert.Equal(20, offsets[(int)RfbScreenInfoPtrField.Height]);
            Assert.Equal(24, offsets[(int)RfbScreenInfoPtrField.Depth]);
            Assert.Equal(28, offsets[(int)RfbScreenInfoPtrField.BitsPerPixel]);
            Assert.Equal(32, offsets[(int)RfbScreenInfoPtrField.SizeInBytes]);
            Assert.Equal(36, offsets[(int)RfbScreenInfoPtrField.BlackPixel]);
            Assert.Equal(40, offsets[(int)RfbScreenInfoPtrField.WhitePixel]);
            Assert.Equal(48, offsets[(int)RfbScreenInfoPtrField.ScreenData]);
            Assert.Equal(56, offsets[(int)RfbScreenInfoPtrField.ServerFormat]);
            Assert.Equal(72, offsets[(int)RfbScreenInfoPtrField.ColourMap]);
            Assert.Equal(88, offsets[(int)RfbScreenInfoPtrField.DesktopName]);
            Assert.Equal(96, offsets[(int)RfbScreenInfoPtrField.ThisHost]);
            Assert.Equal(351, offsets[(int)RfbScreenInfoPtrField.AutoPort]);
            Assert.Equal(352, offsets[(int)RfbScreenInfoPtrField.Port]);
            Assert.Equal(360, offsets[(int)RfbScreenInfoPtrField.ListenSock]);
            Assert.Equal(368, offsets[(int)RfbScreenInfoPtrField.MaxSock]);
            Assert.Equal(372, offsets[(int)RfbScreenInfoPtrField.MaxFd]);
            Assert.Equal(376, offsets[(int)RfbScreenInfoPtrField.AllFds]);
            Assert.Equal(896, offsets[(int)RfbScreenInfoPtrField.SocketState]);
            Assert.Equal(904, offsets[(int)RfbScreenInfoPtrField.InetdSock]);
            Assert.Equal(912, offsets[(int)RfbScreenInfoPtrField.InetdInitDone]);
            Assert.Equal(916, offsets[(int)RfbScreenInfoPtrField.UdpPort]);
            Assert.Equal(920, offsets[(int)RfbScreenInfoPtrField.UdpSock]);
            Assert.Equal(928, offsets[(int)RfbScreenInfoPtrField.UdpClient]);
            Assert.Equal(936, offsets[(int)RfbScreenInfoPtrField.UdpSockConnected]);
            Assert.Equal(940, offsets[(int)RfbScreenInfoPtrField.UdpRemoteAddr]);
            Assert.Equal(956, offsets[(int)RfbScreenInfoPtrField.MaxClientWait]);
            Assert.Equal(960, offsets[(int)RfbScreenInfoPtrField.HttpInitDone]);
            Assert.Equal(961, offsets[(int)RfbScreenInfoPtrField.HttpEnableProxyConnect]);
            Assert.Equal(964, offsets[(int)RfbScreenInfoPtrField.HttpPort]);
            Assert.Equal(968, offsets[(int)RfbScreenInfoPtrField.HttpDir]);
            Assert.Equal(976, offsets[(int)RfbScreenInfoPtrField.HttpListenSock]);
            Assert.Equal(984, offsets[(int)RfbScreenInfoPtrField.HttpSock]);
            Assert.Equal(992, offsets[(int)RfbScreenInfoPtrField.PasswordCheck]);
            Assert.Equal(1000, offsets[(int)RfbScreenInfoPtrField.AuthPasswdData]);
            Assert.Equal(1008, offsets[(int)RfbScreenInfoPtrField.AuthPasswdFirstViewOnly]);
            Assert.Equal(1016, offsets[(int)RfbScreenInfoPtrField.DeferUpdateTime]);
            Assert.Equal(1020, offsets[(int)RfbScreenInfoPtrField.AlwaysShared]);
            Assert.Equal(1021, offsets[(int)RfbScreenInfoPtrField.NeverShared]);
            Assert.Equal(1022, offsets[(int)RfbScreenInfoPtrField.DontDisconnect]);
            Assert.Equal(1024, offsets[(int)RfbScreenInfoPtrField.ClientHead]);
            Assert.Equal(1032, offsets[(int)RfbScreenInfoPtrField.PointerClient]);
            Assert.Equal(1040, offsets[(int)RfbScreenInfoPtrField.CursorX]);
            Assert.Equal(1044, offsets[(int)RfbScreenInfoPtrField.CursorY]);
            Assert.Equal(1048, offsets[(int)RfbScreenInfoPtrField.UnderCursorBufferLen]);
            Assert.Equal(1056, offsets[(int)RfbScreenInfoPtrField.UnderCursorBuffer]);
            Assert.Equal(1064, offsets[(int)RfbScreenInfoPtrField.DontConvertRichCursorToXCursor]);
            Assert.Equal(1072, offsets[(int)RfbScreenInfoPtrField.Cursor]);
            Assert.Equal(1080, offsets[(int)RfbScreenInfoPtrField.FrameBuffer]);
            Assert.Equal(1088, offsets[(int)RfbScreenInfoPtrField.KbdAddEvent]);
            Assert.Equal(1096, offsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys]);
            Assert.Equal(1104, offsets[(int)RfbScreenInfoPtrField.PtrAddEvent]);
            Assert.Equal(1112, offsets[(int)RfbScreenInfoPtrField.SetXCutText]);
            Assert.Equal(1120, offsets[(int)RfbScreenInfoPtrField.GetCursorPtr]);
            Assert.Equal(1128, offsets[(int)RfbScreenInfoPtrField.SetTranslateFunction]);
            Assert.Equal(1136, offsets[(int)RfbScreenInfoPtrField.SetSingleWindow]);
            Assert.Equal(1144, offsets[(int)RfbScreenInfoPtrField.SetServerInput]);
            Assert.Equal(1152, offsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission]);
            Assert.Equal(1160, offsets[(int)RfbScreenInfoPtrField.SetTextChat]);
            Assert.Equal(1168, offsets[(int)RfbScreenInfoPtrField.NewClientHook]);
            Assert.Equal(1176, offsets[(int)RfbScreenInfoPtrField.DisplayHook]);
            Assert.Equal(1184, offsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook]);
            Assert.Equal(1192, offsets[(int)RfbScreenInfoPtrField.CursorMutex]);
            Assert.Equal(1232, offsets[(int)RfbScreenInfoPtrField.BackgroundLoop]);
            Assert.Equal(1233, offsets[(int)RfbScreenInfoPtrField.IgnoreSIGPIPE]);
            Assert.Equal(1236, offsets[(int)RfbScreenInfoPtrField.ProgressiveSliceHeight]);
            Assert.Equal(1240, offsets[(int)RfbScreenInfoPtrField.ListenInterface]);
            Assert.Equal(1244, offsets[(int)RfbScreenInfoPtrField.DeferPtrUpdateTime]);
            Assert.Equal(1248, offsets[(int)RfbScreenInfoPtrField.HandleEventsEagerly]);
            Assert.Equal(1256, offsets[(int)RfbScreenInfoPtrField.VersionString]);
            Assert.Equal(1264, offsets[(int)RfbScreenInfoPtrField.ProtocolMajorVersion]);
            Assert.Equal(1268, offsets[(int)RfbScreenInfoPtrField.ProtocolMinorVersion]);
            Assert.Equal(1272, offsets[(int)RfbScreenInfoPtrField.PermitFileTransfer]);
            Assert.Equal(1280, offsets[(int)RfbScreenInfoPtrField.DisplayFinishedHook]);
            Assert.Equal(1288, offsets[(int)RfbScreenInfoPtrField.XvpHooka]);
            Assert.Equal(1296, offsets[(int)RfbScreenInfoPtrField.Sslkeyfile]);
            Assert.Equal(1304, offsets[(int)RfbScreenInfoPtrField.Sslcertfile]);
            Assert.Equal(1312, offsets[(int)RfbScreenInfoPtrField.Ipv6port]);
            Assert.Equal(1320, offsets[(int)RfbScreenInfoPtrField.Listen6Interface]);
            Assert.Equal(1328, offsets[(int)RfbScreenInfoPtrField.Listen6Sock]);
            Assert.Equal(1336, offsets[(int)RfbScreenInfoPtrField.Http6Port]);
            Assert.Equal(1344, offsets[(int)RfbScreenInfoPtrField.HttpListen6Sock]);
            Assert.Equal(1352, offsets[(int)RfbScreenInfoPtrField.SetDesktopSizeHook]);
            Assert.Equal(1360, offsets[(int)RfbScreenInfoPtrField.NumberOfExtDesktopScreensHook]);
            Assert.Equal(1368, offsets[(int)RfbScreenInfoPtrField.GetExtDesktopScreenHook]);
            Assert.Equal(1376, offsets[(int)RfbScreenInfoPtrField.FdQuota]);
        }

        /// <summary>
        /// Tests the <see cref="RfbScreenInfoPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 32-bit Windows.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_Win32()
        {
            var offsets = RfbScreenInfoPtr.GetFieldOffsets(
                OSPlatform.Windows,
                is64Bit: false,
                new NativeCapabilities()
                {
                    HaveLibJpeg = true,
                    HaveLibPng = true,
                    HaveLibZ = true,
                    HaveLibPthread = false,
                    HaveWin32Threads = true,
                });

            Assert.Equal(0, offsets[(int)RfbScreenInfoPtrField.ScaledScreenNext]);
            Assert.Equal(4, offsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount]);
            Assert.Equal(8, offsets[(int)RfbScreenInfoPtrField.Width]);
            Assert.Equal(12, offsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes]);
            Assert.Equal(16, offsets[(int)RfbScreenInfoPtrField.Height]);
            Assert.Equal(20, offsets[(int)RfbScreenInfoPtrField.Depth]);
            Assert.Equal(24, offsets[(int)RfbScreenInfoPtrField.BitsPerPixel]);
            Assert.Equal(28, offsets[(int)RfbScreenInfoPtrField.SizeInBytes]);
            Assert.Equal(32, offsets[(int)RfbScreenInfoPtrField.BlackPixel]);
            Assert.Equal(36, offsets[(int)RfbScreenInfoPtrField.WhitePixel]);
            Assert.Equal(40, offsets[(int)RfbScreenInfoPtrField.ScreenData]);
            Assert.Equal(44, offsets[(int)RfbScreenInfoPtrField.ServerFormat]);
            Assert.Equal(60, offsets[(int)RfbScreenInfoPtrField.ColourMap]);
            Assert.Equal(72, offsets[(int)RfbScreenInfoPtrField.DesktopName]);
            Assert.Equal(76, offsets[(int)RfbScreenInfoPtrField.ThisHost]);
            Assert.Equal(331, offsets[(int)RfbScreenInfoPtrField.AutoPort]);
            Assert.Equal(332, offsets[(int)RfbScreenInfoPtrField.Port]);
            Assert.Equal(336, offsets[(int)RfbScreenInfoPtrField.ListenSock]);
            Assert.Equal(340, offsets[(int)RfbScreenInfoPtrField.MaxSock]);
            Assert.Equal(344, offsets[(int)RfbScreenInfoPtrField.MaxFd]);
            Assert.Equal(348, offsets[(int)RfbScreenInfoPtrField.AllFds]);
            Assert.Equal(608, offsets[(int)RfbScreenInfoPtrField.SocketState]);
            Assert.Equal(612, offsets[(int)RfbScreenInfoPtrField.InetdSock]);
            Assert.Equal(616, offsets[(int)RfbScreenInfoPtrField.InetdInitDone]);
            Assert.Equal(620, offsets[(int)RfbScreenInfoPtrField.UdpPort]);
            Assert.Equal(624, offsets[(int)RfbScreenInfoPtrField.UdpSock]);
            Assert.Equal(628, offsets[(int)RfbScreenInfoPtrField.UdpClient]);
            Assert.Equal(632, offsets[(int)RfbScreenInfoPtrField.UdpSockConnected]);
            Assert.Equal(636, offsets[(int)RfbScreenInfoPtrField.UdpRemoteAddr]);
            Assert.Equal(652, offsets[(int)RfbScreenInfoPtrField.MaxClientWait]);
            Assert.Equal(656, offsets[(int)RfbScreenInfoPtrField.HttpInitDone]);
            Assert.Equal(657, offsets[(int)RfbScreenInfoPtrField.HttpEnableProxyConnect]);
            Assert.Equal(660, offsets[(int)RfbScreenInfoPtrField.HttpPort]);
            Assert.Equal(664, offsets[(int)RfbScreenInfoPtrField.HttpDir]);
            Assert.Equal(668, offsets[(int)RfbScreenInfoPtrField.HttpListenSock]);
            Assert.Equal(672, offsets[(int)RfbScreenInfoPtrField.HttpSock]);
            Assert.Equal(676, offsets[(int)RfbScreenInfoPtrField.PasswordCheck]);
            Assert.Equal(680, offsets[(int)RfbScreenInfoPtrField.AuthPasswdData]);
            Assert.Equal(684, offsets[(int)RfbScreenInfoPtrField.AuthPasswdFirstViewOnly]);
            Assert.Equal(692, offsets[(int)RfbScreenInfoPtrField.DeferUpdateTime]);
            Assert.Equal(696, offsets[(int)RfbScreenInfoPtrField.AlwaysShared]);
            Assert.Equal(697, offsets[(int)RfbScreenInfoPtrField.NeverShared]);
            Assert.Equal(698, offsets[(int)RfbScreenInfoPtrField.DontDisconnect]);
            Assert.Equal(700, offsets[(int)RfbScreenInfoPtrField.ClientHead]);
            Assert.Equal(704, offsets[(int)RfbScreenInfoPtrField.PointerClient]);
            Assert.Equal(708, offsets[(int)RfbScreenInfoPtrField.CursorX]);
            Assert.Equal(712, offsets[(int)RfbScreenInfoPtrField.CursorY]);
            Assert.Equal(716, offsets[(int)RfbScreenInfoPtrField.UnderCursorBufferLen]);
            Assert.Equal(720, offsets[(int)RfbScreenInfoPtrField.UnderCursorBuffer]);
            Assert.Equal(724, offsets[(int)RfbScreenInfoPtrField.DontConvertRichCursorToXCursor]);
            Assert.Equal(728, offsets[(int)RfbScreenInfoPtrField.Cursor]);
            Assert.Equal(732, offsets[(int)RfbScreenInfoPtrField.FrameBuffer]);
            Assert.Equal(736, offsets[(int)RfbScreenInfoPtrField.KbdAddEvent]);
            Assert.Equal(740, offsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys]);
            Assert.Equal(744, offsets[(int)RfbScreenInfoPtrField.PtrAddEvent]);
            Assert.Equal(748, offsets[(int)RfbScreenInfoPtrField.SetXCutText]);
            Assert.Equal(752, offsets[(int)RfbScreenInfoPtrField.GetCursorPtr]);
            Assert.Equal(756, offsets[(int)RfbScreenInfoPtrField.SetTranslateFunction]);
            Assert.Equal(760, offsets[(int)RfbScreenInfoPtrField.SetSingleWindow]);
            Assert.Equal(764, offsets[(int)RfbScreenInfoPtrField.SetServerInput]);
            Assert.Equal(768, offsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission]);
            Assert.Equal(772, offsets[(int)RfbScreenInfoPtrField.SetTextChat]);
            Assert.Equal(776, offsets[(int)RfbScreenInfoPtrField.NewClientHook]);
            Assert.Equal(780, offsets[(int)RfbScreenInfoPtrField.DisplayHook]);
            Assert.Equal(784, offsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook]);
            Assert.Equal(788, offsets[(int)RfbScreenInfoPtrField.CursorMutex]);
            Assert.Equal(812, offsets[(int)RfbScreenInfoPtrField.BackgroundLoop]);
            Assert.Equal(813, offsets[(int)RfbScreenInfoPtrField.IgnoreSIGPIPE]);
            Assert.Equal(816, offsets[(int)RfbScreenInfoPtrField.ProgressiveSliceHeight]);
            Assert.Equal(820, offsets[(int)RfbScreenInfoPtrField.ListenInterface]);
            Assert.Equal(824, offsets[(int)RfbScreenInfoPtrField.DeferPtrUpdateTime]);
            Assert.Equal(828, offsets[(int)RfbScreenInfoPtrField.HandleEventsEagerly]);
            Assert.Equal(832, offsets[(int)RfbScreenInfoPtrField.VersionString]);
            Assert.Equal(836, offsets[(int)RfbScreenInfoPtrField.ProtocolMajorVersion]);
            Assert.Equal(840, offsets[(int)RfbScreenInfoPtrField.ProtocolMinorVersion]);
            Assert.Equal(844, offsets[(int)RfbScreenInfoPtrField.PermitFileTransfer]);
            Assert.Equal(848, offsets[(int)RfbScreenInfoPtrField.DisplayFinishedHook]);
            Assert.Equal(852, offsets[(int)RfbScreenInfoPtrField.XvpHooka]);
            Assert.Equal(856, offsets[(int)RfbScreenInfoPtrField.Sslkeyfile]);
            Assert.Equal(860, offsets[(int)RfbScreenInfoPtrField.Sslcertfile]);
            Assert.Equal(864, offsets[(int)RfbScreenInfoPtrField.Ipv6port]);
            Assert.Equal(868, offsets[(int)RfbScreenInfoPtrField.Listen6Interface]);
            Assert.Equal(872, offsets[(int)RfbScreenInfoPtrField.Listen6Sock]);
            Assert.Equal(876, offsets[(int)RfbScreenInfoPtrField.Http6Port]);
            Assert.Equal(880, offsets[(int)RfbScreenInfoPtrField.HttpListen6Sock]);
            Assert.Equal(884, offsets[(int)RfbScreenInfoPtrField.SetDesktopSizeHook]);
            Assert.Equal(888, offsets[(int)RfbScreenInfoPtrField.NumberOfExtDesktopScreensHook]);
            Assert.Equal(892, offsets[(int)RfbScreenInfoPtrField.GetExtDesktopScreenHook]);
            Assert.Equal(896, offsets[(int)RfbScreenInfoPtrField.FdQuota]);
        }

        /// <summary>
        /// Tests the <see cref="RfbScreenInfoPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 32-bit Windows.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_Win32_Pthread()
        {
            var offsets = RfbScreenInfoPtr.GetFieldOffsets(
                OSPlatform.Windows,
                is64Bit: false,
                new NativeCapabilities()
                {
                    HaveLibJpeg = false,
                    HaveLibPng = false,
                    HaveLibZ = false,
                    HaveLibPthread = true,
                    HaveWin32Threads = false,
                });

            Assert.Equal(0, offsets[(int)RfbScreenInfoPtrField.ScaledScreenNext]);
            Assert.Equal(4, offsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount]);
            Assert.Equal(8, offsets[(int)RfbScreenInfoPtrField.Width]);
            Assert.Equal(12, offsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes]);
            Assert.Equal(16, offsets[(int)RfbScreenInfoPtrField.Height]);
            Assert.Equal(20, offsets[(int)RfbScreenInfoPtrField.Depth]);
            Assert.Equal(24, offsets[(int)RfbScreenInfoPtrField.BitsPerPixel]);
            Assert.Equal(28, offsets[(int)RfbScreenInfoPtrField.SizeInBytes]);
            Assert.Equal(32, offsets[(int)RfbScreenInfoPtrField.BlackPixel]);
            Assert.Equal(36, offsets[(int)RfbScreenInfoPtrField.WhitePixel]);
            Assert.Equal(40, offsets[(int)RfbScreenInfoPtrField.ScreenData]);
            Assert.Equal(44, offsets[(int)RfbScreenInfoPtrField.ServerFormat]);
            Assert.Equal(60, offsets[(int)RfbScreenInfoPtrField.ColourMap]);
            Assert.Equal(72, offsets[(int)RfbScreenInfoPtrField.DesktopName]);
            Assert.Equal(76, offsets[(int)RfbScreenInfoPtrField.ThisHost]);
            Assert.Equal(331, offsets[(int)RfbScreenInfoPtrField.AutoPort]);
            Assert.Equal(332, offsets[(int)RfbScreenInfoPtrField.Port]);
            Assert.Equal(336, offsets[(int)RfbScreenInfoPtrField.ListenSock]);
            Assert.Equal(340, offsets[(int)RfbScreenInfoPtrField.MaxSock]);
            Assert.Equal(344, offsets[(int)RfbScreenInfoPtrField.MaxFd]);
            Assert.Equal(348, offsets[(int)RfbScreenInfoPtrField.AllFds]);
            Assert.Equal(608, offsets[(int)RfbScreenInfoPtrField.SocketState]);
            Assert.Equal(612, offsets[(int)RfbScreenInfoPtrField.InetdSock]);
            Assert.Equal(616, offsets[(int)RfbScreenInfoPtrField.InetdInitDone]);
            Assert.Equal(620, offsets[(int)RfbScreenInfoPtrField.UdpPort]);
            Assert.Equal(624, offsets[(int)RfbScreenInfoPtrField.UdpSock]);
            Assert.Equal(628, offsets[(int)RfbScreenInfoPtrField.UdpClient]);
            Assert.Equal(632, offsets[(int)RfbScreenInfoPtrField.UdpSockConnected]);
            Assert.Equal(636, offsets[(int)RfbScreenInfoPtrField.UdpRemoteAddr]);
            Assert.Equal(652, offsets[(int)RfbScreenInfoPtrField.MaxClientWait]);
            Assert.Equal(656, offsets[(int)RfbScreenInfoPtrField.HttpInitDone]);
            Assert.Equal(657, offsets[(int)RfbScreenInfoPtrField.HttpEnableProxyConnect]);
            Assert.Equal(660, offsets[(int)RfbScreenInfoPtrField.HttpPort]);
            Assert.Equal(664, offsets[(int)RfbScreenInfoPtrField.HttpDir]);
            Assert.Equal(668, offsets[(int)RfbScreenInfoPtrField.HttpListenSock]);
            Assert.Equal(672, offsets[(int)RfbScreenInfoPtrField.HttpSock]);
            Assert.Equal(676, offsets[(int)RfbScreenInfoPtrField.PasswordCheck]);
            Assert.Equal(680, offsets[(int)RfbScreenInfoPtrField.AuthPasswdData]);
            Assert.Equal(684, offsets[(int)RfbScreenInfoPtrField.AuthPasswdFirstViewOnly]);
            Assert.Equal(692, offsets[(int)RfbScreenInfoPtrField.DeferUpdateTime]);
            Assert.Equal(696, offsets[(int)RfbScreenInfoPtrField.AlwaysShared]);
            Assert.Equal(697, offsets[(int)RfbScreenInfoPtrField.NeverShared]);
            Assert.Equal(698, offsets[(int)RfbScreenInfoPtrField.DontDisconnect]);
            Assert.Equal(700, offsets[(int)RfbScreenInfoPtrField.ClientHead]);
            Assert.Equal(704, offsets[(int)RfbScreenInfoPtrField.PointerClient]);
            Assert.Equal(708, offsets[(int)RfbScreenInfoPtrField.CursorX]);
            Assert.Equal(712, offsets[(int)RfbScreenInfoPtrField.CursorY]);
            Assert.Equal(716, offsets[(int)RfbScreenInfoPtrField.UnderCursorBufferLen]);
            Assert.Equal(720, offsets[(int)RfbScreenInfoPtrField.UnderCursorBuffer]);
            Assert.Equal(724, offsets[(int)RfbScreenInfoPtrField.DontConvertRichCursorToXCursor]);
            Assert.Equal(728, offsets[(int)RfbScreenInfoPtrField.Cursor]);
            Assert.Equal(732, offsets[(int)RfbScreenInfoPtrField.FrameBuffer]);
            Assert.Equal(736, offsets[(int)RfbScreenInfoPtrField.KbdAddEvent]);
            Assert.Equal(740, offsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys]);
            Assert.Equal(744, offsets[(int)RfbScreenInfoPtrField.PtrAddEvent]);
            Assert.Equal(748, offsets[(int)RfbScreenInfoPtrField.SetXCutText]);
            Assert.Equal(752, offsets[(int)RfbScreenInfoPtrField.GetCursorPtr]);
            Assert.Equal(756, offsets[(int)RfbScreenInfoPtrField.SetTranslateFunction]);
            Assert.Equal(760, offsets[(int)RfbScreenInfoPtrField.SetSingleWindow]);
            Assert.Equal(764, offsets[(int)RfbScreenInfoPtrField.SetServerInput]);
            Assert.Equal(768, offsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission]);
            Assert.Equal(772, offsets[(int)RfbScreenInfoPtrField.SetTextChat]);
            Assert.Equal(776, offsets[(int)RfbScreenInfoPtrField.NewClientHook]);
            Assert.Equal(780, offsets[(int)RfbScreenInfoPtrField.DisplayHook]);
            Assert.Equal(784, offsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook]);
            Assert.Equal(788, offsets[(int)RfbScreenInfoPtrField.CursorMutex]);
            Assert.Equal(792, offsets[(int)RfbScreenInfoPtrField.BackgroundLoop]);
            Assert.Equal(793, offsets[(int)RfbScreenInfoPtrField.IgnoreSIGPIPE]);
            Assert.Equal(796, offsets[(int)RfbScreenInfoPtrField.ProgressiveSliceHeight]);
            Assert.Equal(800, offsets[(int)RfbScreenInfoPtrField.ListenInterface]);
            Assert.Equal(804, offsets[(int)RfbScreenInfoPtrField.DeferPtrUpdateTime]);
            Assert.Equal(808, offsets[(int)RfbScreenInfoPtrField.HandleEventsEagerly]);
            Assert.Equal(812, offsets[(int)RfbScreenInfoPtrField.VersionString]);
            Assert.Equal(816, offsets[(int)RfbScreenInfoPtrField.ProtocolMajorVersion]);
            Assert.Equal(820, offsets[(int)RfbScreenInfoPtrField.ProtocolMinorVersion]);
            Assert.Equal(824, offsets[(int)RfbScreenInfoPtrField.PermitFileTransfer]);
            Assert.Equal(828, offsets[(int)RfbScreenInfoPtrField.DisplayFinishedHook]);
            Assert.Equal(832, offsets[(int)RfbScreenInfoPtrField.XvpHooka]);
            Assert.Equal(836, offsets[(int)RfbScreenInfoPtrField.Sslkeyfile]);
            Assert.Equal(840, offsets[(int)RfbScreenInfoPtrField.Sslcertfile]);
            Assert.Equal(844, offsets[(int)RfbScreenInfoPtrField.Ipv6port]);
            Assert.Equal(848, offsets[(int)RfbScreenInfoPtrField.Listen6Interface]);
            Assert.Equal(852, offsets[(int)RfbScreenInfoPtrField.Listen6Sock]);
            Assert.Equal(856, offsets[(int)RfbScreenInfoPtrField.Http6Port]);
            Assert.Equal(860, offsets[(int)RfbScreenInfoPtrField.HttpListen6Sock]);
            Assert.Equal(864, offsets[(int)RfbScreenInfoPtrField.SetDesktopSizeHook]);
            Assert.Equal(868, offsets[(int)RfbScreenInfoPtrField.NumberOfExtDesktopScreensHook]);
            Assert.Equal(872, offsets[(int)RfbScreenInfoPtrField.GetExtDesktopScreenHook]);
            Assert.Equal(876, offsets[(int)RfbScreenInfoPtrField.FdQuota]);
        }

        /// <summary>
        /// Tests the <see cref="RfbScreenInfoPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 64-bit Linux.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_Linux64()
        {
            var offsets = RfbScreenInfoPtr.GetFieldOffsets(OSPlatform.Linux, is64Bit: true);

            Assert.Equal(0, offsets[(int)RfbScreenInfoPtrField.ScaledScreenNext]);
            Assert.Equal(8, offsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount]);
            Assert.Equal(12, offsets[(int)RfbScreenInfoPtrField.Width]);
            Assert.Equal(16, offsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes]);
            Assert.Equal(20, offsets[(int)RfbScreenInfoPtrField.Height]);
            Assert.Equal(24, offsets[(int)RfbScreenInfoPtrField.Depth]);
            Assert.Equal(28, offsets[(int)RfbScreenInfoPtrField.BitsPerPixel]);
            Assert.Equal(32, offsets[(int)RfbScreenInfoPtrField.SizeInBytes]);
            Assert.Equal(36, offsets[(int)RfbScreenInfoPtrField.BlackPixel]);
            Assert.Equal(40, offsets[(int)RfbScreenInfoPtrField.WhitePixel]);
            Assert.Equal(48, offsets[(int)RfbScreenInfoPtrField.ScreenData]);
            Assert.Equal(56, offsets[(int)RfbScreenInfoPtrField.ServerFormat]);
            Assert.Equal(72, offsets[(int)RfbScreenInfoPtrField.ColourMap]);
            Assert.Equal(88, offsets[(int)RfbScreenInfoPtrField.DesktopName]);
            Assert.Equal(96, offsets[(int)RfbScreenInfoPtrField.ThisHost]);
            Assert.Equal(351, offsets[(int)RfbScreenInfoPtrField.AutoPort]);
            Assert.Equal(352, offsets[(int)RfbScreenInfoPtrField.Port]);
            Assert.Equal(356, offsets[(int)RfbScreenInfoPtrField.ListenSock]);
            Assert.Equal(360, offsets[(int)RfbScreenInfoPtrField.MaxSock]);
            Assert.Equal(364, offsets[(int)RfbScreenInfoPtrField.MaxFd]);
            Assert.Equal(368, offsets[(int)RfbScreenInfoPtrField.AllFds]);
            Assert.Equal(496, offsets[(int)RfbScreenInfoPtrField.SocketState]);
            Assert.Equal(500, offsets[(int)RfbScreenInfoPtrField.InetdSock]);
            Assert.Equal(504, offsets[(int)RfbScreenInfoPtrField.InetdInitDone]);
            Assert.Equal(508, offsets[(int)RfbScreenInfoPtrField.UdpPort]);
            Assert.Equal(512, offsets[(int)RfbScreenInfoPtrField.UdpSock]);
            Assert.Equal(520, offsets[(int)RfbScreenInfoPtrField.UdpClient]);
            Assert.Equal(528, offsets[(int)RfbScreenInfoPtrField.UdpSockConnected]);
            Assert.Equal(532, offsets[(int)RfbScreenInfoPtrField.UdpRemoteAddr]);
            Assert.Equal(548, offsets[(int)RfbScreenInfoPtrField.MaxClientWait]);
            Assert.Equal(552, offsets[(int)RfbScreenInfoPtrField.HttpInitDone]);
            Assert.Equal(553, offsets[(int)RfbScreenInfoPtrField.HttpEnableProxyConnect]);
            Assert.Equal(556, offsets[(int)RfbScreenInfoPtrField.HttpPort]);
            Assert.Equal(560, offsets[(int)RfbScreenInfoPtrField.HttpDir]);
            Assert.Equal(568, offsets[(int)RfbScreenInfoPtrField.HttpListenSock]);
            Assert.Equal(572, offsets[(int)RfbScreenInfoPtrField.HttpSock]);
            Assert.Equal(576, offsets[(int)RfbScreenInfoPtrField.PasswordCheck]);
            Assert.Equal(584, offsets[(int)RfbScreenInfoPtrField.AuthPasswdData]);
            Assert.Equal(592, offsets[(int)RfbScreenInfoPtrField.AuthPasswdFirstViewOnly]);
            Assert.Equal(600, offsets[(int)RfbScreenInfoPtrField.DeferUpdateTime]);
            Assert.Equal(604, offsets[(int)RfbScreenInfoPtrField.AlwaysShared]);
            Assert.Equal(605, offsets[(int)RfbScreenInfoPtrField.NeverShared]);
            Assert.Equal(606, offsets[(int)RfbScreenInfoPtrField.DontDisconnect]);
            Assert.Equal(608, offsets[(int)RfbScreenInfoPtrField.ClientHead]);
            Assert.Equal(616, offsets[(int)RfbScreenInfoPtrField.PointerClient]);
            Assert.Equal(624, offsets[(int)RfbScreenInfoPtrField.CursorX]);
            Assert.Equal(628, offsets[(int)RfbScreenInfoPtrField.CursorY]);
            Assert.Equal(632, offsets[(int)RfbScreenInfoPtrField.UnderCursorBufferLen]);
            Assert.Equal(640, offsets[(int)RfbScreenInfoPtrField.UnderCursorBuffer]);
            Assert.Equal(648, offsets[(int)RfbScreenInfoPtrField.DontConvertRichCursorToXCursor]);
            Assert.Equal(656, offsets[(int)RfbScreenInfoPtrField.Cursor]);
            Assert.Equal(664, offsets[(int)RfbScreenInfoPtrField.FrameBuffer]);
            Assert.Equal(672, offsets[(int)RfbScreenInfoPtrField.KbdAddEvent]);
            Assert.Equal(680, offsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys]);
            Assert.Equal(688, offsets[(int)RfbScreenInfoPtrField.PtrAddEvent]);
            Assert.Equal(696, offsets[(int)RfbScreenInfoPtrField.SetXCutText]);
            Assert.Equal(704, offsets[(int)RfbScreenInfoPtrField.GetCursorPtr]);
            Assert.Equal(712, offsets[(int)RfbScreenInfoPtrField.SetTranslateFunction]);
            Assert.Equal(720, offsets[(int)RfbScreenInfoPtrField.SetSingleWindow]);
            Assert.Equal(728, offsets[(int)RfbScreenInfoPtrField.SetServerInput]);
            Assert.Equal(736, offsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission]);
            Assert.Equal(744, offsets[(int)RfbScreenInfoPtrField.SetTextChat]);
            Assert.Equal(752, offsets[(int)RfbScreenInfoPtrField.NewClientHook]);
            Assert.Equal(760, offsets[(int)RfbScreenInfoPtrField.DisplayHook]);
            Assert.Equal(768, offsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook]);
            Assert.Equal(776, offsets[(int)RfbScreenInfoPtrField.CursorMutex]);
            Assert.Equal(816, offsets[(int)RfbScreenInfoPtrField.BackgroundLoop]);
            Assert.Equal(817, offsets[(int)RfbScreenInfoPtrField.IgnoreSIGPIPE]);
            Assert.Equal(820, offsets[(int)RfbScreenInfoPtrField.ProgressiveSliceHeight]);
            Assert.Equal(824, offsets[(int)RfbScreenInfoPtrField.ListenInterface]);
            Assert.Equal(828, offsets[(int)RfbScreenInfoPtrField.DeferPtrUpdateTime]);
            Assert.Equal(832, offsets[(int)RfbScreenInfoPtrField.HandleEventsEagerly]);
            Assert.Equal(840, offsets[(int)RfbScreenInfoPtrField.VersionString]);
            Assert.Equal(848, offsets[(int)RfbScreenInfoPtrField.ProtocolMajorVersion]);
            Assert.Equal(852, offsets[(int)RfbScreenInfoPtrField.ProtocolMinorVersion]);
            Assert.Equal(856, offsets[(int)RfbScreenInfoPtrField.PermitFileTransfer]);
            Assert.Equal(864, offsets[(int)RfbScreenInfoPtrField.DisplayFinishedHook]);
            Assert.Equal(872, offsets[(int)RfbScreenInfoPtrField.XvpHooka]);
            Assert.Equal(880, offsets[(int)RfbScreenInfoPtrField.Sslkeyfile]);
            Assert.Equal(888, offsets[(int)RfbScreenInfoPtrField.Sslcertfile]);
            Assert.Equal(896, offsets[(int)RfbScreenInfoPtrField.Ipv6port]);
            Assert.Equal(904, offsets[(int)RfbScreenInfoPtrField.Listen6Interface]);
            Assert.Equal(912, offsets[(int)RfbScreenInfoPtrField.Listen6Sock]);
            Assert.Equal(916, offsets[(int)RfbScreenInfoPtrField.Http6Port]);
            Assert.Equal(920, offsets[(int)RfbScreenInfoPtrField.HttpListen6Sock]);
        }

        /// <summary>
        /// Tests the <see cref="RfbScreenInfoPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 64-bit OSX.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_OSX64()
        {
            var offsets = RfbScreenInfoPtr.GetFieldOffsets(OSPlatform.OSX, is64Bit: true);

            Assert.Equal(0, offsets[(int)RfbScreenInfoPtrField.ScaledScreenNext]);
            Assert.Equal(8, offsets[(int)RfbScreenInfoPtrField.ScaledScreenRefCount]);
            Assert.Equal(12, offsets[(int)RfbScreenInfoPtrField.Width]);
            Assert.Equal(16, offsets[(int)RfbScreenInfoPtrField.PaddedWidthInBytes]);
            Assert.Equal(20, offsets[(int)RfbScreenInfoPtrField.Height]);
            Assert.Equal(24, offsets[(int)RfbScreenInfoPtrField.Depth]);
            Assert.Equal(28, offsets[(int)RfbScreenInfoPtrField.BitsPerPixel]);
            Assert.Equal(32, offsets[(int)RfbScreenInfoPtrField.SizeInBytes]);
            Assert.Equal(36, offsets[(int)RfbScreenInfoPtrField.BlackPixel]);
            Assert.Equal(40, offsets[(int)RfbScreenInfoPtrField.WhitePixel]);
            Assert.Equal(48, offsets[(int)RfbScreenInfoPtrField.ScreenData]);
            Assert.Equal(56, offsets[(int)RfbScreenInfoPtrField.ServerFormat]);
            Assert.Equal(72, offsets[(int)RfbScreenInfoPtrField.ColourMap]);
            Assert.Equal(88, offsets[(int)RfbScreenInfoPtrField.DesktopName]);
            Assert.Equal(96, offsets[(int)RfbScreenInfoPtrField.ThisHost]);
            Assert.Equal(351, offsets[(int)RfbScreenInfoPtrField.AutoPort]);
            Assert.Equal(352, offsets[(int)RfbScreenInfoPtrField.Port]);
            Assert.Equal(356, offsets[(int)RfbScreenInfoPtrField.ListenSock]);
            Assert.Equal(360, offsets[(int)RfbScreenInfoPtrField.MaxSock]);
            Assert.Equal(364, offsets[(int)RfbScreenInfoPtrField.MaxFd]);
            Assert.Equal(368, offsets[(int)RfbScreenInfoPtrField.AllFds]);
            Assert.Equal(496, offsets[(int)RfbScreenInfoPtrField.SocketState]);
            Assert.Equal(500, offsets[(int)RfbScreenInfoPtrField.InetdSock]);
            Assert.Equal(504, offsets[(int)RfbScreenInfoPtrField.InetdInitDone]);
            Assert.Equal(508, offsets[(int)RfbScreenInfoPtrField.UdpPort]);
            Assert.Equal(512, offsets[(int)RfbScreenInfoPtrField.UdpSock]);
            Assert.Equal(520, offsets[(int)RfbScreenInfoPtrField.UdpClient]);
            Assert.Equal(528, offsets[(int)RfbScreenInfoPtrField.UdpSockConnected]);
            Assert.Equal(532, offsets[(int)RfbScreenInfoPtrField.UdpRemoteAddr]);
            Assert.Equal(548, offsets[(int)RfbScreenInfoPtrField.MaxClientWait]);
            Assert.Equal(552, offsets[(int)RfbScreenInfoPtrField.HttpInitDone]);
            Assert.Equal(553, offsets[(int)RfbScreenInfoPtrField.HttpEnableProxyConnect]);
            Assert.Equal(556, offsets[(int)RfbScreenInfoPtrField.HttpPort]);
            Assert.Equal(560, offsets[(int)RfbScreenInfoPtrField.HttpDir]);
            Assert.Equal(568, offsets[(int)RfbScreenInfoPtrField.HttpListenSock]);
            Assert.Equal(572, offsets[(int)RfbScreenInfoPtrField.HttpSock]);
            Assert.Equal(576, offsets[(int)RfbScreenInfoPtrField.PasswordCheck]);
            Assert.Equal(584, offsets[(int)RfbScreenInfoPtrField.AuthPasswdData]);
            Assert.Equal(592, offsets[(int)RfbScreenInfoPtrField.AuthPasswdFirstViewOnly]);
            Assert.Equal(600, offsets[(int)RfbScreenInfoPtrField.DeferUpdateTime]);
            Assert.Equal(604, offsets[(int)RfbScreenInfoPtrField.AlwaysShared]);
            Assert.Equal(605, offsets[(int)RfbScreenInfoPtrField.NeverShared]);
            Assert.Equal(606, offsets[(int)RfbScreenInfoPtrField.DontDisconnect]);
            Assert.Equal(608, offsets[(int)RfbScreenInfoPtrField.ClientHead]);
            Assert.Equal(616, offsets[(int)RfbScreenInfoPtrField.PointerClient]);
            Assert.Equal(624, offsets[(int)RfbScreenInfoPtrField.CursorX]);
            Assert.Equal(628, offsets[(int)RfbScreenInfoPtrField.CursorY]);
            Assert.Equal(632, offsets[(int)RfbScreenInfoPtrField.UnderCursorBufferLen]);
            Assert.Equal(640, offsets[(int)RfbScreenInfoPtrField.UnderCursorBuffer]);
            Assert.Equal(648, offsets[(int)RfbScreenInfoPtrField.DontConvertRichCursorToXCursor]);
            Assert.Equal(656, offsets[(int)RfbScreenInfoPtrField.Cursor]);
            Assert.Equal(664, offsets[(int)RfbScreenInfoPtrField.FrameBuffer]);
            Assert.Equal(672, offsets[(int)RfbScreenInfoPtrField.KbdAddEvent]);
            Assert.Equal(680, offsets[(int)RfbScreenInfoPtrField.KbdReleaseAllKeys]);
            Assert.Equal(688, offsets[(int)RfbScreenInfoPtrField.PtrAddEvent]);
            Assert.Equal(696, offsets[(int)RfbScreenInfoPtrField.SetXCutText]);
            Assert.Equal(704, offsets[(int)RfbScreenInfoPtrField.GetCursorPtr]);
            Assert.Equal(712, offsets[(int)RfbScreenInfoPtrField.SetTranslateFunction]);
            Assert.Equal(720, offsets[(int)RfbScreenInfoPtrField.SetSingleWindow]);
            Assert.Equal(728, offsets[(int)RfbScreenInfoPtrField.SetServerInput]);
            Assert.Equal(736, offsets[(int)RfbScreenInfoPtrField.GetFileTransferPermission]);
            Assert.Equal(744, offsets[(int)RfbScreenInfoPtrField.SetTextChat]);
            Assert.Equal(752, offsets[(int)RfbScreenInfoPtrField.NewClientHook]);
            Assert.Equal(760, offsets[(int)RfbScreenInfoPtrField.DisplayHook]);
            Assert.Equal(768, offsets[(int)RfbScreenInfoPtrField.GetKeyboardLedStateHook]);
            Assert.Equal(776, offsets[(int)RfbScreenInfoPtrField.CursorMutex]);
            Assert.Equal(840, offsets[(int)RfbScreenInfoPtrField.BackgroundLoop]);
            Assert.Equal(841, offsets[(int)RfbScreenInfoPtrField.IgnoreSIGPIPE]);
            Assert.Equal(844, offsets[(int)RfbScreenInfoPtrField.ProgressiveSliceHeight]);
            Assert.Equal(848, offsets[(int)RfbScreenInfoPtrField.ListenInterface]);
            Assert.Equal(852, offsets[(int)RfbScreenInfoPtrField.DeferPtrUpdateTime]);
            Assert.Equal(856, offsets[(int)RfbScreenInfoPtrField.HandleEventsEagerly]);
            Assert.Equal(864, offsets[(int)RfbScreenInfoPtrField.VersionString]);
            Assert.Equal(872, offsets[(int)RfbScreenInfoPtrField.ProtocolMajorVersion]);
            Assert.Equal(876, offsets[(int)RfbScreenInfoPtrField.ProtocolMinorVersion]);
            Assert.Equal(880, offsets[(int)RfbScreenInfoPtrField.PermitFileTransfer]);
            Assert.Equal(888, offsets[(int)RfbScreenInfoPtrField.DisplayFinishedHook]);
            Assert.Equal(896, offsets[(int)RfbScreenInfoPtrField.XvpHooka]);
            Assert.Equal(904, offsets[(int)RfbScreenInfoPtrField.Sslkeyfile]);
            Assert.Equal(912, offsets[(int)RfbScreenInfoPtrField.Sslcertfile]);
            Assert.Equal(920, offsets[(int)RfbScreenInfoPtrField.Ipv6port]);
            Assert.Equal(928, offsets[(int)RfbScreenInfoPtrField.Listen6Interface]);
            Assert.Equal(936, offsets[(int)RfbScreenInfoPtrField.Listen6Sock]);
            Assert.Equal(940, offsets[(int)RfbScreenInfoPtrField.Http6Port]);
            Assert.Equal(944, offsets[(int)RfbScreenInfoPtrField.HttpListen6Sock]);
            Assert.Equal(952, offsets[(int)RfbScreenInfoPtrField.SetDesktopSizeHook]);
            Assert.Equal(960, offsets[(int)RfbScreenInfoPtrField.NumberOfExtDesktopScreensHook]);
            Assert.Equal(968, offsets[(int)RfbScreenInfoPtrField.GetExtDesktopScreenHook]);
            Assert.Equal(976, offsets[(int)RfbScreenInfoPtrField.FdQuota]);
        }
    }
}