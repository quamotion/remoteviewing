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

using System;
using System.Runtime.InteropServices;

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// Contains logic to determine the offset of individual fields of the <see cref="RfbScreenInfoPtr"/> class.
    /// </summary>
    public partial class RfbScreenInfoPtr
    {
        private static readonly RfbType[] FieldTypes = new RfbType[]
        {
            RfbType.Pointer, // scaledScreenNext
            RfbType.Int, // scaledScreenRefCount
            RfbType.Int, // width
            RfbType.Int, // paddedWidthInBytes
            RfbType.Int, // height
            RfbType.Int, // depth
            RfbType.Int, // bitsPerPixel
            RfbType.Int, // sizeInBytes
            RfbType.Pixel, // blackPixel
            RfbType.Pixel, // whitePixel,
            RfbType.Pointer, // screenData,
            RfbType.PixelFormat, // serverFormat,
            RfbType.ColourMap, // colourMap,
            RfbType.Pointer, // desktopName,
            RfbType.Char_255, // thisHost,
            RfbType.Bool, // autoPort,
            RfbType.Int, // port,
            RfbType.Socket, // listenSock,
            RfbType.Int, // maxSock,
            RfbType.Int, // maxFd,
            RfbType.FdSet, // allFds,
            RfbType.SocketState, // socketState,
            RfbType.Socket, // inetdSock,
            RfbType.Bool, // inetdInitDone,
            RfbType.Int, // udpPort,
            RfbType.Socket, // udpSock,
            RfbType.Pointer, // udpClient,
            RfbType.Bool, // udpSockConnected,
            RfbType.SockAddrIn, // udpRemoteAddr,
            RfbType.Int, // maxClientWait,
            RfbType.Bool, // httpInitDone,
            RfbType.Bool, // httpEnableProxyConnect,
            RfbType.Int, // httpPort,
            RfbType.Pointer, // httpDir,
            RfbType.Socket, // httpListenSock,
            RfbType.Socket, // httpSock,
            RfbType.Pointer, // passwordCheck,
            RfbType.Pointer, // authPasswdData,
            RfbType.Int, // authPasswdFirstViewOnly,
            RfbType.Int, // maxRectsPerUpdate,
            RfbType.Int, // deferUpdateTime,

            // RfbType.Skip, // screen
            RfbType.Bool, // alwaysShared,
            RfbType.Bool, // neverShared,
            RfbType.Bool, // dontDisconnect,
            RfbType.Pointer, // clientHead,
            RfbType.Pointer, // pointerClient,
            RfbType.Int, // cursorX,
            RfbType.Int, // cursorY,
            RfbType.Int, // underCursorBufferLen,
            RfbType.Pointer, // underCursorBuffer,
            RfbType.Bool, // dontConvertRichCursorToXCursor,
            RfbType.Pointer, // cursor,
            RfbType.Pointer, // frameBuffer,
            RfbType.Pointer, // kbdAddEvent,
            RfbType.Pointer, // kbdReleaseAllKeys,
            RfbType.Pointer, // ptrAddEvent,
            RfbType.Pointer, // setXCutText,
            RfbType.Pointer, // getCursorPtr,
            RfbType.Pointer, // setTranslateFunction,
            RfbType.Pointer, // setSingleWindow,
            RfbType.Pointer, // setServerInput,
            RfbType.Pointer, // getFileTransferPermission,
            RfbType.Pointer, // setTextChat,
            RfbType.Pointer, // newClientHook,
            RfbType.Pointer, // displayHook,
            RfbType.Pointer, // getKeyboardLedStateHook,
            RfbType.PthreadMutex, // cursorMutex,
            RfbType.Bool, // backgroundLoop,
            RfbType.Bool, // ignoreSIGPIPE,
            RfbType.Int, // progressiveSliceHeight,
            RfbType.InAddrT, // listenInterface,
            RfbType.Int, // deferPtrUpdateTime,
            RfbType.Bool, // handleEventsEagerly,
            RfbType.Pointer, // versionString,
            RfbType.Int, // protocolMajorVersion,
            RfbType.Int, // protocolMinorVersion,
            RfbType.Bool, // permitFileTransfer,
            RfbType.Pointer, // displayFinishedHook,
            RfbType.Pointer, // xvpHooka,
            RfbType.Pointer, // sslkeyfile,
            RfbType.Pointer, // sslcertfile,
            RfbType.Int, // ipv6port,
            RfbType.Pointer, // listen6Interface,
            RfbType.Socket, // listen6Sock,
            RfbType.Int, // http6Port,
            RfbType.Socket, // httpListen6Sock,
            RfbType.Pointer, // setDesktopSizeHook,
            RfbType.Pointer, // numberOfExtDesktopScreensHook,
            RfbType.Pointer, // getExtDesktopScreenHook,
            RfbType.Float, // fdQuota,
        };

        private static readonly int[] FieldOffsets = GetFieldOffsets(NativeLayout.GetOSPlatform(), Environment.Is64BitProcess);

        public static int[] GetFieldOffsets(OSPlatform platform, bool is64Bit)
        {
            NativeCapabilities nativeCapabilities;

            if (platform == OSPlatform.Windows)
            {
                nativeCapabilities = new NativeCapabilities()
                {
                    HaveLibJpeg = true,
                    HaveLibPng = true,
                    HaveLibZ = true,
                    HaveLibPthread = false,
                    HaveWin32Threads = true,
                };
            }
            else
            {
                nativeCapabilities = NativeCapabilities.Unix;
            }

            return GetFieldOffsets(platform, is64Bit, nativeCapabilities);
        }

        public static int[] GetFieldOffsets(OSPlatform platform, bool is64Bit, NativeCapabilities nativeCapabilities)
        {
            var fieldTypes = new RfbType[FieldTypes.Length];
            Array.Copy(FieldTypes, fieldTypes, fieldTypes.Length);

            if (nativeCapabilities.HaveWin32Threads)
            {
                fieldTypes[(int)RfbScreenInfoPtrField.CursorMutex] = RfbType.Win32Mutex;
            }

            return NativeLayout.GetFieldOffsets(fieldTypes, platform, is64Bit);
        }
    }
}
