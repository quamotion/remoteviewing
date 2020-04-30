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
    /// Contains logic to determine the offset of individual fields of the <see cref="RfbClientRecPtr"/> class.
    /// </summary>
    public partial class RfbClientRecPtr
    {
        private static readonly RfbType[] FieldTypes = new RfbType[]
        {
            RfbType.Pointer, // screen
            RfbType.Pointer, // scaledScreen
            RfbType.Bool, // PalmVNC
            RfbType.Pointer, // clientData
            RfbType.Pointer, // clientGoneHook
            RfbType.Socket, // sock
            RfbType.Pointer, // host
            RfbType.Int, // protocolMajorVersion
            RfbType.Int, // protocolMinorVersion
            RfbType.PthreadT, // client_thread
            RfbType.Int, // state
            RfbType.Bool, // reverseConnection
            RfbType.Bool, // onHold
            RfbType.Bool, // readyForSetColourMapEntries
            RfbType.Bool, // useCopyRect
            RfbType.Int, // preferredEncoding
            RfbType.Int, // correMaxWidth
            RfbType.Int, // correMaxHeight
            RfbType.Bool, // viewOnly
            RfbType.Byte_CHALLENGESIZE, // authChallenge
            RfbType.Pointer, // copyRegion,
            RfbType.Int, // copyDX,
            RfbType.Int, // copyDY
            RfbType.Pointer, // modifiedRegion
            RfbType.Pointer, // requestedRegion
            RfbType.Timeval, // startDeferring
            RfbType.Timeval, // startPtrDeferring
            RfbType.Int, // lastPtrX
            RfbType.Int, // lastPtrY
            RfbType.Int, // lastPtrButtons
            RfbType.Pointer, // translateFn
            RfbType.Pointer, // translateLookupTable
            RfbType.PixelFormat, // rfbPixelFormat
            RfbType.Byte_30000, // updateBuf
            RfbType.Int, // ublen
            RfbType.Pointer, // statEncList
            RfbType.Pointer, // statMsgList
            RfbType.Int, // rawBytesEquivalent
            RfbType.Int, // bytesSent
            RfbType.Z_stream_s, // compStream
            RfbType.Bool, // compStreamInited
            RfbType.Int, // zlibCompressLevel
            RfbType.Int, // tightQualityLevel
            RfbType.ZsStruct_4, // z_stream
            RfbType.Bool_4, // zsActive
            RfbType.Int_4, // zsLevel
            RfbType.Int, // tightCompressLevel
            RfbType.Bool, // compStreamInitedLZO
            RfbType.Pointer, // lzoWrkMem
            RfbType.RfbFileTransferData, // fileTransfer
            RfbType.Int, // lastKeyboardLedState
            RfbType.Bool, // enableSupportedMessages
            RfbType.Bool, // enableSupportedEncodings
            RfbType.Bool, // enableServerIdentity
            RfbType.Bool, // enableKeyboardLedState
            RfbType.Bool, // enableLastRectEncoding
            RfbType.Bool, // enableCursorShapeUpdates
            RfbType.Bool, // enableCursorPosUpdates
            RfbType.Bool, // useRichCursorEncoding
            RfbType.Bool, // cursorWasChanged
            RfbType.Bool, // cursorWasMoved
            RfbType.Int, // cursorX
            RfbType.Int, // cursorY
            RfbType.Bool, // useNewFBSize
            RfbType.Bool, // newFBSizePending
            RfbType.Pointer, // prev
            RfbType.Pointer, // next
            RfbType.Int, // refCount
            RfbType.Mutex, // refCountMute
            RfbType.Pthread_cond_t, // deleteCond
            RfbType.Mutex, // outputMutex
            RfbType.Mutex, // updateMutex
            RfbType.Pthread_cond_t, // updateCond
            RfbType.Pointer, // zrleData
            RfbType.Int, // zywrleLevel
            RfbType.Int_ZRLE, // zywrleBuf
            RfbType.Int, // progressiveSliceY
            RfbType.Pointer, // extensions
            RfbType.Pointer, // zrleBeforeBuf
            RfbType.Pointer, // paletteHelper
            RfbType.Mutex, // sendMutex
            RfbType.Pointer, // beforeEncBuf
            RfbType.Int, // beforeEncBufSize
            RfbType.Pointer, // afterEncBuf
            RfbType.Int, // afterEncBufSize
            RfbType.Int, // afterEncBufLen
            RfbType.Int, // tightEncoding
            RfbType.Int, // turboSubsampLevel
            RfbType.Int, // turboQualityLevel
            RfbType.Pointer, // sslctx
            RfbType.Pointer, // wsctx
            RfbType.Pointer, // wspath
            RfbType.Int_2, // pipe_notify_client_thread
            RfbType.Pointer, // clientFramebufferUpdateRequestHook
            RfbType.Bool, // useExtDesktopSize
            RfbType.Int, // requestedDesktopSizeChange
            RfbType.Int, // lastDesktopSizeChangeError
        };

        private static readonly int[] FieldOffsets = GetFieldOffsets(NativeLayout.GetOSPlatform(), Environment.Is64BitProcess);

        public static int[] GetFieldOffsets(OSPlatform platform, bool is64Bit)
        {
            var fieldTypes = new RfbType[FieldTypes.Length];
            Array.Copy(FieldTypes, fieldTypes, fieldTypes.Length);

            if (platform == OSPlatform.Windows)
            {
                bool haveLibZ = false;
                bool haveLibPng = false;
                bool haveLibJpeg = false;

                if (!haveLibZ)
                {
                    fieldTypes[(int)RfbClientRecPtrField.CompStream] = RfbType.Skip;
                    fieldTypes[(int)RfbClientRecPtrField.CompStreamInited] = RfbType.Skip;
                    fieldTypes[(int)RfbClientRecPtrField.ZlibCompressLevel] = RfbType.Skip;
                }

                if (!haveLibZ && !haveLibPng)
                {
                    fieldTypes[(int)RfbClientRecPtrField.TightQualityLevel] = RfbType.Skip;

                    if (!haveLibJpeg)
                    {
                        fieldTypes[(int)RfbClientRecPtrField.ZsStruct] = RfbType.Skip;
                        fieldTypes[(int)RfbClientRecPtrField.ZsActive] = RfbType.Skip;
                        fieldTypes[(int)RfbClientRecPtrField.ZsLevel] = RfbType.Skip;
                        fieldTypes[(int)RfbClientRecPtrField.TightCompressLevel] = RfbType.Skip;
                    }
                }

                if (haveLibZ)
                {
                    fieldTypes[(int)RfbClientRecPtrField.ZrleData] = RfbType.Skip;
                    fieldTypes[(int)RfbClientRecPtrField.ZywrleLevel] = RfbType.Skip;
                    fieldTypes[(int)RfbClientRecPtrField.ZywrleBuf] = RfbType.Skip;
                }

                if (!haveLibZ && !haveLibPng)
                {
                    fieldTypes[(int)RfbClientRecPtrField.TightEncoding] = RfbType.Skip;

                    if (!haveLibJpeg)
                    {
                        fieldTypes[(int)RfbClientRecPtrField.TurboSubsampLevel] = RfbType.Skip;
                        fieldTypes[(int)RfbClientRecPtrField.TurboQualityLevel] = RfbType.Skip;
                    }
                }
            }

            return NativeLayout.GetFieldOffsets(fieldTypes, platform, is64Bit);
        }
    }
}
