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
using System.Runtime.InteropServices;
using Xunit;

namespace RemoteViewing.LibVnc.Tests
{
    /// <summary>
    /// Tests the <see cref="RfbClientRecPtr"/> class.
    /// </summary>
    public class RfbClientRecPtrTests
    {
        // You can use the following regex to transform output of get_offset.c to xUnit assert statements:
        //   offsetof\(rfbClientRec, ([a-zA-Z_]*)\) : (\d*)
        //   Assert.Equal($2, offsets[(int)RfbClientRecPtrField.$1]);

        /// <summary>
        /// Tests the <see cref="RfbClientRefPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 64-bit Windows.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_Win64()
        {
            var offsets = RfbClientRecPtr.GetFieldOffsets(OSPlatform.Windows, is64Bit: true);

            Assert.Equal(0, offsets[(int)RfbClientRecPtrField.Screen]);
            Assert.Equal(8, offsets[(int)RfbClientRecPtrField.ScaledScreen]);
            Assert.Equal(16, offsets[(int)RfbClientRecPtrField.PalmVNC]);
            Assert.Equal(24, offsets[(int)RfbClientRecPtrField.ClientData]);
            Assert.Equal(32, offsets[(int)RfbClientRecPtrField.ClientGoneHook]);
            Assert.Equal(40, offsets[(int)RfbClientRecPtrField.Sock]);
            Assert.Equal(48, offsets[(int)RfbClientRecPtrField.Host]);
            Assert.Equal(56, offsets[(int)RfbClientRecPtrField.ProtocolMajorVersion]);
            Assert.Equal(60, offsets[(int)RfbClientRecPtrField.ProtocolMinorVersion]);
            Assert.Equal(64, offsets[(int)RfbClientRecPtrField.Client_thread]);
            Assert.Equal(72, offsets[(int)RfbClientRecPtrField.State]);
            Assert.Equal(76, offsets[(int)RfbClientRecPtrField.ReverseConnection]);
            Assert.Equal(77, offsets[(int)RfbClientRecPtrField.OnHold]);
            Assert.Equal(78, offsets[(int)RfbClientRecPtrField.ReadyForSetColourMapEntries]);
            Assert.Equal(79, offsets[(int)RfbClientRecPtrField.UseCopyRect]);
            Assert.Equal(80, offsets[(int)RfbClientRecPtrField.PreferredEncoding]);
            Assert.Equal(84, offsets[(int)RfbClientRecPtrField.CorreMaxWidth]);
            Assert.Equal(88, offsets[(int)RfbClientRecPtrField.CorreMaxHeight]);
            Assert.Equal(92, offsets[(int)RfbClientRecPtrField.ViewOnly]);
            Assert.Equal(93, offsets[(int)RfbClientRecPtrField.AuthChallenge]);
            Assert.Equal(112, offsets[(int)RfbClientRecPtrField.CopyRegion]);
            Assert.Equal(120, offsets[(int)RfbClientRecPtrField.CopyDX]);
            Assert.Equal(124, offsets[(int)RfbClientRecPtrField.CopyDY]);
            Assert.Equal(128, offsets[(int)RfbClientRecPtrField.ModifiedRegion]);
            Assert.Equal(136, offsets[(int)RfbClientRecPtrField.RequestedRegion]);
            Assert.Equal(144, offsets[(int)RfbClientRecPtrField.StartDeferring]);
            Assert.Equal(152, offsets[(int)RfbClientRecPtrField.StartPtrDeferring]);
            Assert.Equal(160, offsets[(int)RfbClientRecPtrField.LastPtrX]);
            Assert.Equal(164, offsets[(int)RfbClientRecPtrField.LastPtrY]);
            Assert.Equal(168, offsets[(int)RfbClientRecPtrField.LastPtrButtons]);
            Assert.Equal(176, offsets[(int)RfbClientRecPtrField.TranslateFn]);
            Assert.Equal(184, offsets[(int)RfbClientRecPtrField.TranslateLookupTable]);
            Assert.Equal(192, offsets[(int)RfbClientRecPtrField.Format]);
            Assert.Equal(208, offsets[(int)RfbClientRecPtrField.UpdateBuf]);
            Assert.Equal(30208, offsets[(int)RfbClientRecPtrField.Ublen]);
            Assert.Equal(30216, offsets[(int)RfbClientRecPtrField.StatEncList]);
            Assert.Equal(30224, offsets[(int)RfbClientRecPtrField.StatMsgList]);
            Assert.Equal(30232, offsets[(int)RfbClientRecPtrField.RawBytesEquivalent]);
            Assert.Equal(30236, offsets[(int)RfbClientRecPtrField.BytesSent]);
            Assert.Equal(30240, offsets[(int)RfbClientRecPtrField.CompStreamInitedLZO]);
            Assert.Equal(30248, offsets[(int)RfbClientRecPtrField.LzoWrkMem]);
            Assert.Equal(30256, offsets[(int)RfbClientRecPtrField.FileTransfer]);
            Assert.Equal(30280, offsets[(int)RfbClientRecPtrField.LastKeyboardLedState]);
            Assert.Equal(30284, offsets[(int)RfbClientRecPtrField.EnableSupportedMessages]);
            Assert.Equal(30285, offsets[(int)RfbClientRecPtrField.EnableSupportedEncodings]);
            Assert.Equal(30286, offsets[(int)RfbClientRecPtrField.EnableServerIdentity]);
            Assert.Equal(30287, offsets[(int)RfbClientRecPtrField.EnableKeyboardLedState]);
            Assert.Equal(30288, offsets[(int)RfbClientRecPtrField.EnableLastRectEncoding]);
            Assert.Equal(30289, offsets[(int)RfbClientRecPtrField.EnableCursorShapeUpdates]);
            Assert.Equal(30290, offsets[(int)RfbClientRecPtrField.EnableCursorPosUpdates]);
            Assert.Equal(30291, offsets[(int)RfbClientRecPtrField.UseRichCursorEncoding]);
            Assert.Equal(30292, offsets[(int)RfbClientRecPtrField.CursorWasChanged]);
            Assert.Equal(30293, offsets[(int)RfbClientRecPtrField.CursorWasMoved]);
            Assert.Equal(30296, offsets[(int)RfbClientRecPtrField.CursorX]);
            Assert.Equal(30300, offsets[(int)RfbClientRecPtrField.CursorY]);
            Assert.Equal(30304, offsets[(int)RfbClientRecPtrField.UseNewFBSize]);
            Assert.Equal(30305, offsets[(int)RfbClientRecPtrField.NewFBSizePending]);
            Assert.Equal(30312, offsets[(int)RfbClientRecPtrField.Prev]);
            Assert.Equal(30320, offsets[(int)RfbClientRecPtrField.Next]);
            Assert.Equal(30328, offsets[(int)RfbClientRecPtrField.RefCount]);
            Assert.Equal(30336, offsets[(int)RfbClientRecPtrField.RefCountMutex]);
            Assert.Equal(30344, offsets[(int)RfbClientRecPtrField.DeleteCond]);
            Assert.Equal(30352, offsets[(int)RfbClientRecPtrField.OutputMutex]);
            Assert.Equal(30360, offsets[(int)RfbClientRecPtrField.UpdateMutex]);
            Assert.Equal(30368, offsets[(int)RfbClientRecPtrField.UpdateCond]);
            Assert.Equal(30376, offsets[(int)RfbClientRecPtrField.ProgressiveSliceY]);
            Assert.Equal(30384, offsets[(int)RfbClientRecPtrField.Extensions]);
            Assert.Equal(30392, offsets[(int)RfbClientRecPtrField.ZrleBeforeBuf]);
            Assert.Equal(30400, offsets[(int)RfbClientRecPtrField.PaletteHelper]);
            Assert.Equal(30408, offsets[(int)RfbClientRecPtrField.SendMutex]);
            Assert.Equal(30416, offsets[(int)RfbClientRecPtrField.BeforeEncBuf]);
            Assert.Equal(30424, offsets[(int)RfbClientRecPtrField.BeforeEncBufSize]);
            Assert.Equal(30432, offsets[(int)RfbClientRecPtrField.AfterEncBuf]);
            Assert.Equal(30440, offsets[(int)RfbClientRecPtrField.AfterEncBufSize]);
            Assert.Equal(30444, offsets[(int)RfbClientRecPtrField.AfterEncBufLen]);
            Assert.Equal(30448, offsets[(int)RfbClientRecPtrField.Sslctx]);
            Assert.Equal(30456, offsets[(int)RfbClientRecPtrField.Wsctx]);
            Assert.Equal(30464, offsets[(int)RfbClientRecPtrField.Wspath]);
            Assert.Equal(30472, offsets[(int)RfbClientRecPtrField.Pipe_notify_client_thread]);
            Assert.Equal(30480, offsets[(int)RfbClientRecPtrField.ClientFramebufferUpdateRequestHook]);
            Assert.Equal(30488, offsets[(int)RfbClientRecPtrField.UseExtDesktopSize]);
            Assert.Equal(30492, offsets[(int)RfbClientRecPtrField.RequestedDesktopSizeChange]);
            Assert.Equal(30496, offsets[(int)RfbClientRecPtrField.LastDesktopSizeChangeError]);
        }

        /// <summary>
        /// Tests the <see cref="RfbClientRefPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 32-bit Windows.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_Win32()
        {
            var offsets = RfbClientRecPtr.GetFieldOffsets(OSPlatform.Windows, is64Bit: false);

            Assert.Equal(0, offsets[(int)RfbClientRecPtrField.Screen]);
            Assert.Equal(4, offsets[(int)RfbClientRecPtrField.ScaledScreen]);
            Assert.Equal(8, offsets[(int)RfbClientRecPtrField.PalmVNC]);
            Assert.Equal(12, offsets[(int)RfbClientRecPtrField.ClientData]);
            Assert.Equal(16, offsets[(int)RfbClientRecPtrField.ClientGoneHook]);
            Assert.Equal(20, offsets[(int)RfbClientRecPtrField.Sock]);
            Assert.Equal(24, offsets[(int)RfbClientRecPtrField.Host]);
            Assert.Equal(28, offsets[(int)RfbClientRecPtrField.ProtocolMajorVersion]);
            Assert.Equal(32, offsets[(int)RfbClientRecPtrField.ProtocolMinorVersion]);
            Assert.Equal(36, offsets[(int)RfbClientRecPtrField.Client_thread]);
            Assert.Equal(40, offsets[(int)RfbClientRecPtrField.State]);
            Assert.Equal(44, offsets[(int)RfbClientRecPtrField.ReverseConnection]);
            Assert.Equal(45, offsets[(int)RfbClientRecPtrField.OnHold]);
            Assert.Equal(46, offsets[(int)RfbClientRecPtrField.ReadyForSetColourMapEntries]);
            Assert.Equal(47, offsets[(int)RfbClientRecPtrField.UseCopyRect]);
            Assert.Equal(48, offsets[(int)RfbClientRecPtrField.PreferredEncoding]);
            Assert.Equal(52, offsets[(int)RfbClientRecPtrField.CorreMaxWidth]);
            Assert.Equal(56, offsets[(int)RfbClientRecPtrField.CorreMaxHeight]);
            Assert.Equal(60, offsets[(int)RfbClientRecPtrField.ViewOnly]);
            Assert.Equal(61, offsets[(int)RfbClientRecPtrField.AuthChallenge]);
            Assert.Equal(80, offsets[(int)RfbClientRecPtrField.CopyRegion]);
            Assert.Equal(84, offsets[(int)RfbClientRecPtrField.CopyDX]);
            Assert.Equal(88, offsets[(int)RfbClientRecPtrField.CopyDY]);
            Assert.Equal(92, offsets[(int)RfbClientRecPtrField.ModifiedRegion]);
            Assert.Equal(96, offsets[(int)RfbClientRecPtrField.RequestedRegion]);
            Assert.Equal(100, offsets[(int)RfbClientRecPtrField.StartDeferring]);
            Assert.Equal(108, offsets[(int)RfbClientRecPtrField.StartPtrDeferring]);
            Assert.Equal(116, offsets[(int)RfbClientRecPtrField.LastPtrX]);
            Assert.Equal(120, offsets[(int)RfbClientRecPtrField.LastPtrY]);
            Assert.Equal(124, offsets[(int)RfbClientRecPtrField.LastPtrButtons]);
            Assert.Equal(128, offsets[(int)RfbClientRecPtrField.TranslateFn]);
            Assert.Equal(132, offsets[(int)RfbClientRecPtrField.TranslateLookupTable]);
            Assert.Equal(136, offsets[(int)RfbClientRecPtrField.Format]);
            Assert.Equal(152, offsets[(int)RfbClientRecPtrField.UpdateBuf]);
            Assert.Equal(30152, offsets[(int)RfbClientRecPtrField.Ublen]);
            Assert.Equal(30156, offsets[(int)RfbClientRecPtrField.StatEncList]);
            Assert.Equal(30160, offsets[(int)RfbClientRecPtrField.StatMsgList]);
            Assert.Equal(30164, offsets[(int)RfbClientRecPtrField.RawBytesEquivalent]);
            Assert.Equal(30168, offsets[(int)RfbClientRecPtrField.BytesSent]);
            Assert.Equal(30172, offsets[(int)RfbClientRecPtrField.CompStreamInitedLZO]);
            Assert.Equal(30176, offsets[(int)RfbClientRecPtrField.LzoWrkMem]);
            Assert.Equal(30180, offsets[(int)RfbClientRecPtrField.FileTransfer]);
            Assert.Equal(30204, offsets[(int)RfbClientRecPtrField.LastKeyboardLedState]);
            Assert.Equal(30208, offsets[(int)RfbClientRecPtrField.EnableSupportedMessages]);
            Assert.Equal(30209, offsets[(int)RfbClientRecPtrField.EnableSupportedEncodings]);
            Assert.Equal(30210, offsets[(int)RfbClientRecPtrField.EnableServerIdentity]);
            Assert.Equal(30211, offsets[(int)RfbClientRecPtrField.EnableKeyboardLedState]);
            Assert.Equal(30212, offsets[(int)RfbClientRecPtrField.EnableLastRectEncoding]);
            Assert.Equal(30213, offsets[(int)RfbClientRecPtrField.EnableCursorShapeUpdates]);
            Assert.Equal(30214, offsets[(int)RfbClientRecPtrField.EnableCursorPosUpdates]);
            Assert.Equal(30215, offsets[(int)RfbClientRecPtrField.UseRichCursorEncoding]);
            Assert.Equal(30216, offsets[(int)RfbClientRecPtrField.CursorWasChanged]);
            Assert.Equal(30217, offsets[(int)RfbClientRecPtrField.CursorWasMoved]);
            Assert.Equal(30220, offsets[(int)RfbClientRecPtrField.CursorX]);
            Assert.Equal(30224, offsets[(int)RfbClientRecPtrField.CursorY]);
            Assert.Equal(30228, offsets[(int)RfbClientRecPtrField.UseNewFBSize]);
            Assert.Equal(30229, offsets[(int)RfbClientRecPtrField.NewFBSizePending]);
            Assert.Equal(30232, offsets[(int)RfbClientRecPtrField.Prev]);
            Assert.Equal(30236, offsets[(int)RfbClientRecPtrField.Next]);
            Assert.Equal(30240, offsets[(int)RfbClientRecPtrField.RefCount]);
            Assert.Equal(30244, offsets[(int)RfbClientRecPtrField.RefCountMutex]);
            Assert.Equal(30248, offsets[(int)RfbClientRecPtrField.DeleteCond]);
            Assert.Equal(30252, offsets[(int)RfbClientRecPtrField.OutputMutex]);
            Assert.Equal(30256, offsets[(int)RfbClientRecPtrField.UpdateMutex]);
            Assert.Equal(30260, offsets[(int)RfbClientRecPtrField.UpdateCond]);
            Assert.Equal(30264, offsets[(int)RfbClientRecPtrField.ProgressiveSliceY]);
            Assert.Equal(30268, offsets[(int)RfbClientRecPtrField.Extensions]);
            Assert.Equal(30272, offsets[(int)RfbClientRecPtrField.ZrleBeforeBuf]);
            Assert.Equal(30276, offsets[(int)RfbClientRecPtrField.PaletteHelper]);
            Assert.Equal(30280, offsets[(int)RfbClientRecPtrField.SendMutex]);
            Assert.Equal(30284, offsets[(int)RfbClientRecPtrField.BeforeEncBuf]);
            Assert.Equal(30288, offsets[(int)RfbClientRecPtrField.BeforeEncBufSize]);
            Assert.Equal(30292, offsets[(int)RfbClientRecPtrField.AfterEncBuf]);
            Assert.Equal(30296, offsets[(int)RfbClientRecPtrField.AfterEncBufSize]);
            Assert.Equal(30300, offsets[(int)RfbClientRecPtrField.AfterEncBufLen]);
            Assert.Equal(30304, offsets[(int)RfbClientRecPtrField.Sslctx]);
            Assert.Equal(30308, offsets[(int)RfbClientRecPtrField.Wsctx]);
            Assert.Equal(30312, offsets[(int)RfbClientRecPtrField.Wspath]);
            Assert.Equal(30316, offsets[(int)RfbClientRecPtrField.Pipe_notify_client_thread]);
            Assert.Equal(30324, offsets[(int)RfbClientRecPtrField.ClientFramebufferUpdateRequestHook]);
            Assert.Equal(30328, offsets[(int)RfbClientRecPtrField.UseExtDesktopSize]);
            Assert.Equal(30332, offsets[(int)RfbClientRecPtrField.RequestedDesktopSizeChange]);
            Assert.Equal(30336, offsets[(int)RfbClientRecPtrField.LastDesktopSizeChangeError]);
        }

        /// <summary>
        /// Tests the <see cref="RfbClientRefPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 64-bit Linux.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_Linux64()
        {
            var offsets = RfbClientRecPtr.GetFieldOffsets(OSPlatform.Linux, is64Bit: true);

            Assert.Equal(0, offsets[(int)RfbClientRecPtrField.Screen]);
            Assert.Equal(8, offsets[(int)RfbClientRecPtrField.ScaledScreen]);
            Assert.Equal(16, offsets[(int)RfbClientRecPtrField.PalmVNC]);
            Assert.Equal(24, offsets[(int)RfbClientRecPtrField.ClientData]);
            Assert.Equal(32, offsets[(int)RfbClientRecPtrField.ClientGoneHook]);
            Assert.Equal(40, offsets[(int)RfbClientRecPtrField.Sock]);
            Assert.Equal(48, offsets[(int)RfbClientRecPtrField.Host]);
            Assert.Equal(56, offsets[(int)RfbClientRecPtrField.ProtocolMajorVersion]);
            Assert.Equal(60, offsets[(int)RfbClientRecPtrField.ProtocolMinorVersion]);
            Assert.Equal(64, offsets[(int)RfbClientRecPtrField.Client_thread]);
            Assert.Equal(72, offsets[(int)RfbClientRecPtrField.State]);
            Assert.Equal(76, offsets[(int)RfbClientRecPtrField.ReverseConnection]);
            Assert.Equal(77, offsets[(int)RfbClientRecPtrField.OnHold]);
            Assert.Equal(78, offsets[(int)RfbClientRecPtrField.ReadyForSetColourMapEntries]);
            Assert.Equal(79, offsets[(int)RfbClientRecPtrField.UseCopyRect]);
            Assert.Equal(80, offsets[(int)RfbClientRecPtrField.PreferredEncoding]);
            Assert.Equal(84, offsets[(int)RfbClientRecPtrField.CorreMaxWidth]);
            Assert.Equal(88, offsets[(int)RfbClientRecPtrField.CorreMaxHeight]);
            Assert.Equal(92, offsets[(int)RfbClientRecPtrField.ViewOnly]);
            Assert.Equal(93, offsets[(int)RfbClientRecPtrField.AuthChallenge]);
            Assert.Equal(112, offsets[(int)RfbClientRecPtrField.CopyRegion]);
            Assert.Equal(120, offsets[(int)RfbClientRecPtrField.CopyDX]);
            Assert.Equal(124, offsets[(int)RfbClientRecPtrField.CopyDY]);
            Assert.Equal(128, offsets[(int)RfbClientRecPtrField.ModifiedRegion]);
            Assert.Equal(136, offsets[(int)RfbClientRecPtrField.RequestedRegion]);
            Assert.Equal(144, offsets[(int)RfbClientRecPtrField.StartDeferring]);
            Assert.Equal(160, offsets[(int)RfbClientRecPtrField.StartPtrDeferring]);
            Assert.Equal(176, offsets[(int)RfbClientRecPtrField.LastPtrX]);
            Assert.Equal(180, offsets[(int)RfbClientRecPtrField.LastPtrY]);
            Assert.Equal(184, offsets[(int)RfbClientRecPtrField.LastPtrButtons]);
            Assert.Equal(192, offsets[(int)RfbClientRecPtrField.TranslateFn]);
            Assert.Equal(200, offsets[(int)RfbClientRecPtrField.TranslateLookupTable]);
            Assert.Equal(208, offsets[(int)RfbClientRecPtrField.Format]);
            Assert.Equal(224, offsets[(int)RfbClientRecPtrField.UpdateBuf]);
            Assert.Equal(30224, offsets[(int)RfbClientRecPtrField.Ublen]);
            Assert.Equal(30232, offsets[(int)RfbClientRecPtrField.StatEncList]);
            Assert.Equal(30240, offsets[(int)RfbClientRecPtrField.StatMsgList]);
            Assert.Equal(30248, offsets[(int)RfbClientRecPtrField.RawBytesEquivalent]);
            Assert.Equal(30252, offsets[(int)RfbClientRecPtrField.BytesSent]);
            Assert.Equal(30256, offsets[(int)RfbClientRecPtrField.CompStream]);
            Assert.Equal(30368, offsets[(int)RfbClientRecPtrField.CompStreamInited]);
            Assert.Equal(30372, offsets[(int)RfbClientRecPtrField.ZlibCompressLevel]);
            Assert.Equal(30376, offsets[(int)RfbClientRecPtrField.TightQualityLevel]);
            Assert.Equal(30384, offsets[(int)RfbClientRecPtrField.ZsStruct]);
            Assert.Equal(30832, offsets[(int)RfbClientRecPtrField.ZsActive]);
            Assert.Equal(30836, offsets[(int)RfbClientRecPtrField.ZsLevel]);
            Assert.Equal(30852, offsets[(int)RfbClientRecPtrField.TightCompressLevel]);
            Assert.Equal(30856, offsets[(int)RfbClientRecPtrField.CompStreamInitedLZO]);
            Assert.Equal(30864, offsets[(int)RfbClientRecPtrField.LzoWrkMem]);
            Assert.Equal(30872, offsets[(int)RfbClientRecPtrField.FileTransfer]);
            Assert.Equal(30896, offsets[(int)RfbClientRecPtrField.LastKeyboardLedState]);
            Assert.Equal(30900, offsets[(int)RfbClientRecPtrField.EnableSupportedMessages]);
            Assert.Equal(30901, offsets[(int)RfbClientRecPtrField.EnableSupportedEncodings]);
            Assert.Equal(30902, offsets[(int)RfbClientRecPtrField.EnableServerIdentity]);
            Assert.Equal(30903, offsets[(int)RfbClientRecPtrField.EnableKeyboardLedState]);
            Assert.Equal(30904, offsets[(int)RfbClientRecPtrField.EnableLastRectEncoding]);
            Assert.Equal(30905, offsets[(int)RfbClientRecPtrField.EnableCursorShapeUpdates]);
            Assert.Equal(30906, offsets[(int)RfbClientRecPtrField.EnableCursorPosUpdates]);
            Assert.Equal(30907, offsets[(int)RfbClientRecPtrField.UseRichCursorEncoding]);
            Assert.Equal(30908, offsets[(int)RfbClientRecPtrField.CursorWasChanged]);
            Assert.Equal(30909, offsets[(int)RfbClientRecPtrField.CursorWasMoved]);
            Assert.Equal(30912, offsets[(int)RfbClientRecPtrField.CursorX]);
            Assert.Equal(30916, offsets[(int)RfbClientRecPtrField.CursorY]);
            Assert.Equal(30920, offsets[(int)RfbClientRecPtrField.UseNewFBSize]);
            Assert.Equal(30921, offsets[(int)RfbClientRecPtrField.NewFBSizePending]);
            Assert.Equal(30928, offsets[(int)RfbClientRecPtrField.Prev]);
            Assert.Equal(30936, offsets[(int)RfbClientRecPtrField.Next]);
            Assert.Equal(30944, offsets[(int)RfbClientRecPtrField.RefCount]);
            Assert.Equal(30952, offsets[(int)RfbClientRecPtrField.RefCountMutex]);
            Assert.Equal(30992, offsets[(int)RfbClientRecPtrField.DeleteCond]);
            Assert.Equal(31040, offsets[(int)RfbClientRecPtrField.OutputMutex]);
            Assert.Equal(31080, offsets[(int)RfbClientRecPtrField.UpdateMutex]);
            Assert.Equal(31120, offsets[(int)RfbClientRecPtrField.UpdateCond]);
            Assert.Equal(31168, offsets[(int)RfbClientRecPtrField.ZrleData]);
            Assert.Equal(31176, offsets[(int)RfbClientRecPtrField.ZywrleLevel]);
            Assert.Equal(31180, offsets[(int)RfbClientRecPtrField.ZywrleBuf]);
            Assert.Equal(47564, offsets[(int)RfbClientRecPtrField.ProgressiveSliceY]);
            Assert.Equal(47568, offsets[(int)RfbClientRecPtrField.Extensions]);
            Assert.Equal(47576, offsets[(int)RfbClientRecPtrField.ZrleBeforeBuf]);
            Assert.Equal(47584, offsets[(int)RfbClientRecPtrField.PaletteHelper]);
            Assert.Equal(47592, offsets[(int)RfbClientRecPtrField.SendMutex]);
            Assert.Equal(47632, offsets[(int)RfbClientRecPtrField.BeforeEncBuf]);
            Assert.Equal(47640, offsets[(int)RfbClientRecPtrField.BeforeEncBufSize]);
            Assert.Equal(47648, offsets[(int)RfbClientRecPtrField.AfterEncBuf]);
            Assert.Equal(47656, offsets[(int)RfbClientRecPtrField.AfterEncBufSize]);
            Assert.Equal(47660, offsets[(int)RfbClientRecPtrField.AfterEncBufLen]);
            Assert.Equal(47664, offsets[(int)RfbClientRecPtrField.TightEncoding]);
            Assert.Equal(47668, offsets[(int)RfbClientRecPtrField.TurboSubsampLevel]);
            Assert.Equal(47672, offsets[(int)RfbClientRecPtrField.TurboQualityLevel]);
            Assert.Equal(47680, offsets[(int)RfbClientRecPtrField.Sslctx]);
            Assert.Equal(47688, offsets[(int)RfbClientRecPtrField.Wsctx]);
            Assert.Equal(47696, offsets[(int)RfbClientRecPtrField.Wspath]);
        }

        /// <summary>
        /// Tests the <see cref="RfbClientRefPtr.GetFieldOffsets(OSPlatform, bool)"/> method for 64-bit OS X.
        /// </summary>
        [Fact]
        public void GetFieldOffsetsTest_OSX64()
        {
            var offsets = RfbClientRecPtr.GetFieldOffsets(OSPlatform.OSX, is64Bit: true);

            Assert.Equal(0, offsets[(int)RfbClientRecPtrField.Screen]);
            Assert.Equal(8, offsets[(int)RfbClientRecPtrField.ScaledScreen]);
            Assert.Equal(16, offsets[(int)RfbClientRecPtrField.PalmVNC]);
            Assert.Equal(24, offsets[(int)RfbClientRecPtrField.ClientData]);
            Assert.Equal(32, offsets[(int)RfbClientRecPtrField.ClientGoneHook]);
            Assert.Equal(40, offsets[(int)RfbClientRecPtrField.Sock]);
            Assert.Equal(48, offsets[(int)RfbClientRecPtrField.Host]);
            Assert.Equal(56, offsets[(int)RfbClientRecPtrField.ProtocolMajorVersion]);
            Assert.Equal(60, offsets[(int)RfbClientRecPtrField.ProtocolMinorVersion]);
            Assert.Equal(64, offsets[(int)RfbClientRecPtrField.Client_thread]);
            Assert.Equal(72, offsets[(int)RfbClientRecPtrField.State]);
            Assert.Equal(76, offsets[(int)RfbClientRecPtrField.ReverseConnection]);
            Assert.Equal(77, offsets[(int)RfbClientRecPtrField.OnHold]);
            Assert.Equal(78, offsets[(int)RfbClientRecPtrField.ReadyForSetColourMapEntries]);
            Assert.Equal(79, offsets[(int)RfbClientRecPtrField.UseCopyRect]);
            Assert.Equal(80, offsets[(int)RfbClientRecPtrField.PreferredEncoding]);
            Assert.Equal(84, offsets[(int)RfbClientRecPtrField.CorreMaxWidth]);
            Assert.Equal(88, offsets[(int)RfbClientRecPtrField.CorreMaxHeight]);
            Assert.Equal(92, offsets[(int)RfbClientRecPtrField.ViewOnly]);
            Assert.Equal(93, offsets[(int)RfbClientRecPtrField.AuthChallenge]);
            Assert.Equal(112, offsets[(int)RfbClientRecPtrField.CopyRegion]);
            Assert.Equal(120, offsets[(int)RfbClientRecPtrField.CopyDX]);
            Assert.Equal(124, offsets[(int)RfbClientRecPtrField.CopyDY]);
            Assert.Equal(128, offsets[(int)RfbClientRecPtrField.ModifiedRegion]);
            Assert.Equal(136, offsets[(int)RfbClientRecPtrField.RequestedRegion]);
            Assert.Equal(144, offsets[(int)RfbClientRecPtrField.StartDeferring]);
            Assert.Equal(160, offsets[(int)RfbClientRecPtrField.StartPtrDeferring]);
            Assert.Equal(176, offsets[(int)RfbClientRecPtrField.LastPtrX]);
            Assert.Equal(180, offsets[(int)RfbClientRecPtrField.LastPtrY]);
            Assert.Equal(184, offsets[(int)RfbClientRecPtrField.LastPtrButtons]);
            Assert.Equal(192, offsets[(int)RfbClientRecPtrField.TranslateFn]);
            Assert.Equal(200, offsets[(int)RfbClientRecPtrField.TranslateLookupTable]);
            Assert.Equal(208, offsets[(int)RfbClientRecPtrField.Format]);
            Assert.Equal(224, offsets[(int)RfbClientRecPtrField.UpdateBuf]);
            Assert.Equal(30224, offsets[(int)RfbClientRecPtrField.Ublen]);
            Assert.Equal(30232, offsets[(int)RfbClientRecPtrField.StatEncList]);
            Assert.Equal(30240, offsets[(int)RfbClientRecPtrField.StatMsgList]);
            Assert.Equal(30248, offsets[(int)RfbClientRecPtrField.RawBytesEquivalent]);
            Assert.Equal(30252, offsets[(int)RfbClientRecPtrField.BytesSent]);
            Assert.Equal(30256, offsets[(int)RfbClientRecPtrField.CompStream]);
            Assert.Equal(30368, offsets[(int)RfbClientRecPtrField.CompStreamInited]);
            Assert.Equal(30372, offsets[(int)RfbClientRecPtrField.ZlibCompressLevel]);
            Assert.Equal(30376, offsets[(int)RfbClientRecPtrField.TightQualityLevel]);
            Assert.Equal(30384, offsets[(int)RfbClientRecPtrField.ZsStruct]);
            Assert.Equal(30832, offsets[(int)RfbClientRecPtrField.ZsActive]);
            Assert.Equal(30836, offsets[(int)RfbClientRecPtrField.ZsLevel]);
            Assert.Equal(30852, offsets[(int)RfbClientRecPtrField.TightCompressLevel]);
            Assert.Equal(30856, offsets[(int)RfbClientRecPtrField.CompStreamInitedLZO]);
            Assert.Equal(30864, offsets[(int)RfbClientRecPtrField.LzoWrkMem]);
            Assert.Equal(30872, offsets[(int)RfbClientRecPtrField.FileTransfer]);
            Assert.Equal(30896, offsets[(int)RfbClientRecPtrField.LastKeyboardLedState]);
            Assert.Equal(30900, offsets[(int)RfbClientRecPtrField.EnableSupportedMessages]);
            Assert.Equal(30901, offsets[(int)RfbClientRecPtrField.EnableSupportedEncodings]);
            Assert.Equal(30902, offsets[(int)RfbClientRecPtrField.EnableServerIdentity]);
            Assert.Equal(30903, offsets[(int)RfbClientRecPtrField.EnableKeyboardLedState]);
            Assert.Equal(30904, offsets[(int)RfbClientRecPtrField.EnableLastRectEncoding]);
            Assert.Equal(30905, offsets[(int)RfbClientRecPtrField.EnableCursorShapeUpdates]);
            Assert.Equal(30906, offsets[(int)RfbClientRecPtrField.EnableCursorPosUpdates]);
            Assert.Equal(30907, offsets[(int)RfbClientRecPtrField.UseRichCursorEncoding]);
            Assert.Equal(30908, offsets[(int)RfbClientRecPtrField.CursorWasChanged]);
            Assert.Equal(30909, offsets[(int)RfbClientRecPtrField.CursorWasMoved]);
            Assert.Equal(30912, offsets[(int)RfbClientRecPtrField.CursorX]);
            Assert.Equal(30916, offsets[(int)RfbClientRecPtrField.CursorY]);
            Assert.Equal(30920, offsets[(int)RfbClientRecPtrField.UseNewFBSize]);
            Assert.Equal(30921, offsets[(int)RfbClientRecPtrField.NewFBSizePending]);
            Assert.Equal(30928, offsets[(int)RfbClientRecPtrField.Prev]);
            Assert.Equal(30936, offsets[(int)RfbClientRecPtrField.Next]);
            Assert.Equal(30944, offsets[(int)RfbClientRecPtrField.RefCount]);
            Assert.Equal(30952, offsets[(int)RfbClientRecPtrField.RefCountMutex]);
            Assert.Equal(31016, offsets[(int)RfbClientRecPtrField.DeleteCond]);
            Assert.Equal(31064, offsets[(int)RfbClientRecPtrField.OutputMutex]);
            Assert.Equal(31128, offsets[(int)RfbClientRecPtrField.UpdateMutex]);
            Assert.Equal(31192, offsets[(int)RfbClientRecPtrField.UpdateCond]);
            Assert.Equal(31240, offsets[(int)RfbClientRecPtrField.ZrleData]);
            Assert.Equal(31248, offsets[(int)RfbClientRecPtrField.ZywrleLevel]);
            Assert.Equal(31252, offsets[(int)RfbClientRecPtrField.ZywrleBuf]);
            Assert.Equal(47636, offsets[(int)RfbClientRecPtrField.ProgressiveSliceY]);
            Assert.Equal(47640, offsets[(int)RfbClientRecPtrField.Extensions]);
            Assert.Equal(47648, offsets[(int)RfbClientRecPtrField.ZrleBeforeBuf]);
            Assert.Equal(47656, offsets[(int)RfbClientRecPtrField.PaletteHelper]);
            Assert.Equal(47664, offsets[(int)RfbClientRecPtrField.SendMutex]);
            Assert.Equal(47728, offsets[(int)RfbClientRecPtrField.BeforeEncBuf]);
            Assert.Equal(47736, offsets[(int)RfbClientRecPtrField.BeforeEncBufSize]);
            Assert.Equal(47744, offsets[(int)RfbClientRecPtrField.AfterEncBuf]);
            Assert.Equal(47752, offsets[(int)RfbClientRecPtrField.AfterEncBufSize]);
            Assert.Equal(47756, offsets[(int)RfbClientRecPtrField.AfterEncBufLen]);
            Assert.Equal(47760, offsets[(int)RfbClientRecPtrField.TightEncoding]);
            Assert.Equal(47764, offsets[(int)RfbClientRecPtrField.TurboSubsampLevel]);
            Assert.Equal(47768, offsets[(int)RfbClientRecPtrField.TurboQualityLevel]);
            Assert.Equal(47776, offsets[(int)RfbClientRecPtrField.Sslctx]);
            Assert.Equal(47784, offsets[(int)RfbClientRecPtrField.Wsctx]);
            Assert.Equal(47792, offsets[(int)RfbClientRecPtrField.Wspath]);
            Assert.Equal(47800, offsets[(int)RfbClientRecPtrField.Pipe_notify_client_thread]);
            Assert.Equal(47808, offsets[(int)RfbClientRecPtrField.ClientFramebufferUpdateRequestHook]);
            Assert.Equal(47816, offsets[(int)RfbClientRecPtrField.UseExtDesktopSize]);
            Assert.Equal(47820, offsets[(int)RfbClientRecPtrField.RequestedDesktopSizeChange]);
            Assert.Equal(47824, offsets[(int)RfbClientRecPtrField.LastDesktopSizeChangeError]);
        }
    }
}