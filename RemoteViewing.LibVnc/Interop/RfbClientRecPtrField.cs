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
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS SERVICES,
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

namespace RemoteViewing.LibVnc.Interop
{
    /// <summary>
    /// Represents the various fields of the <see cref="RfbClientRecPtr"/> struct.
    /// </summary>
    public enum RfbClientRecPtrField
    {
#pragma warning disable SA1602 // Enumeration items should be documented
        Screen,
        ScaledScreen,
        PalmVNC,
        ClientData,
        ClientGoneHook,
        Sock,
        Host,
        ProtocolMajorVersion,
        ProtocolMinorVersion,
        Client_thread,
        State,
        ReverseConnection,
        OnHold,
        ReadyForSetColourMapEntries,
        UseCopyRect,
        PreferredEncoding,
        CorreMaxWidth,
        CorreMaxHeight,
        ViewOnly,
        AuthChallenge,
        CopyRegion,
        CopyDX,
        CopyDY,
        ModifiedRegion,
        RequestedRegion,
        StartDeferring,
        StartPtrDeferring,
        LastPtrX,
        LastPtrY,
        LastPtrButtons,
        TranslateFn,
        TranslateLookupTable,
        Format,
        UpdateBuf,
        Ublen,
        StatEncList,
        StatMsgList,
        RawBytesEquivalent,
        BytesSent,
        CompStream,
        CompStreamInited,
        ZlibCompressLevel,
        TightQualityLevel,
        ZsStruct,
        ZsActive,
        ZsLevel,
        TightCompressLevel,
        CompStreamInitedLZO,
        LzoWrkMem,
        FileTransfer,
        LastKeyboardLedState,
        EnableSupportedMessages,
        EnableSupportedEncodings,
        EnableServerIdentity,
        EnableKeyboardLedState,
        EnableLastRectEncoding,
        EnableCursorShapeUpdates,
        EnableCursorPosUpdates,
        UseRichCursorEncoding,
        CursorWasChanged,
        CursorWasMoved,
        CursorX,
        CursorY,
        UseNewFBSize,
        NewFBSizePending,
        Prev,
        Next,
        RefCount,
        RefCountMutex,
        DeleteCond,
        OutputMutex,
        UpdateMutex,
        UpdateCond,
        ZrleData,
        ZywrleLevel,
        ZywrleBuf,
        ProgressiveSliceY,
        Extensions,
        ZrleBeforeBuf,
        PaletteHelper,
        SendMutex,
        BeforeEncBuf,
        BeforeEncBufSize,
        AfterEncBuf,
        AfterEncBufSize,
        AfterEncBufLen,
        TightEncoding,
        TurboSubsampLevel,
        TurboQualityLevel,
        Sslctx,
        Wsctx,
        Wspath,
        Pipe_notify_client_thread,
        ClientFramebufferUpdateRequestHook,
        UseExtDesktopSize,
        RequestedDesktopSizeChange,
        LastDesktopSizeChangeError,
#pragma warning restore SA1602 // Enumeration items should be documented
    }
}
