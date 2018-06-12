#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2018 Quamotion bvba <http://quamotion.mobi>
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

using System.Runtime.InteropServices;

namespace RemoteViewing.VMware
{
    /// <summary>
    /// The AVIMAINHEADER structure defines global information in an AVI file.
    /// </summary>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd318180(v=vs.85).aspx"/>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AviMainHeader
    {
        /// <summary>
        /// Specifies the number of microseconds between frames. This value indicates the overall timing for the file.
        /// </summary>
        public int MicroSecPerFrame;

        /// <summary>
        /// Specifies the approximate maximum data rate of the file. This value indicates the number of bytes
        /// per second the system must handle to present an AVI sequence as specified by the other parameters
        /// contained in the main header and stream header chunks.
        /// </summary>
        public int MaxBytesPerSec;

        /// <summary>
        /// Specifies the alignment for data, in bytes. Pad the data to multiples of this value.
        /// </summary>
        public int PaddingGranularity;

        /// <summary>
        /// Contains a bitwise combination of zero or more of the <see cref="MainHeaderFlags"/>.
        /// </summary>
        public MainHeaderFlags Flags; // the ever-present flags

        /// <summary>
        /// Specifies the total number of frames of data in the file.
        /// </summary>
        public int TotalFrames;

        /// <summary>
        /// Specifies the initial frame for interleaved files. Noninterleaved files should specify zero.
        /// If you are creating interleaved files, specify the number of frames in the file prior to the initial
        /// frame of the AVI sequence in this member.
        /// To give the audio driver enough audio to work with, the audio data in an interleaved file must be skewed
        /// from the video data.Typically, the audio data should be moved forward enough frames to allow approximately
        /// 0.75 seconds of audio data to be preloaded.The dwInitialRecords member should be set to the number of frames
        /// the audio is skewed.Also set the same value for the dwInitialFrames member of the AVISTREAMHEADER structure
        /// in the audio stream header.
        /// </summary>
        public int InitialFrames;

        /// <summary>
        /// Specifies the number of streams in the file. For example, a file with audio and video has two streams.
        /// </summary>
        public int Streams;

        /// <summary>
        /// Specifies the suggested buffer size for reading the file. Generally, this size should be large enough to
        /// contain the largest chunk in the file. If set to zero, or if it is too small, the playback software will
        /// have to reallocate memory during playback, which will reduce performance. For an interleaved file, the
        /// buffer size should be large enough to read an entire record, and not just a chunk.
        /// </summary>
        public int SuggestedBufferSize;

        /// <summary>
        /// Specifies the width of the AVI file in pixels.
        /// </summary>
        public int Width;

        /// <summary>
        /// Specifies the height of the AVI file in pixels.
        /// </summary>
        public int Height;

        private readonly int reserved1;
        private readonly int reserved2;
        private readonly int reserved3;
        private readonly int reserved4;
    }
}
