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
    /// The <see cref="AviStreamHeader"/> structure contains information about one stream in an AVI file.
    /// </summary>
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd318183(v=vs.85).aspx"/>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AviStreamHeader
    {
        /// <summary>
        /// Contains a <see cref="FourCC"/> that specifies the type of the data contained in the stream.
        /// </summary>
        public FourCC Type;

        /// <summary>
        /// Optionally, contains a <see cref="FourCC"/> that identifies a specific data handler.
        /// The data handler is the preferred handler for the stream. For audio and video streams,
        /// this specifies the codec for decoding the stream.
        /// </summary>
        public FourCC Handler;

        /// <summary>
        /// Contains any flags for the data stream. The bits in the high-order word of these flags are
        /// specific to the type of data contained in the stream.
        /// </summary>
        public int Flags;

        /// <summary>
        /// Specifies priority of a stream type. For example, in a file with multiple audio streams,
        /// the one with the highest priority might be the default stream.
        /// </summary>
        public short Priority;

        /// <summary>
        /// Language tag.
        /// </summary>
        public short Language;

        /// <summary>
        /// Specifies how far audio data is skewed ahead of the video frames in interleaved files.
        /// Typically, this is about 0.75 seconds. If you are creating interleaved files, specify the
        /// number of frames in the file prior to the initial frame of the AVI sequence in this member.
        /// For more information, see the remarks for the dwInitialFrames member of the <see cref="AviMainHeader"/> structure.
        /// </summary>
        public int InitialFrames;

        /// <summary>
        /// Used with <see cref="Rate"/> to specify the time scale that this stream will use.
        /// Dividing <see cref="Rate"/> by <see cref="Scale"/> gives the number of samples per second.
        /// For video streams, this is the frame rate. For audio streams, this rate corresponds to the
        /// time needed to play nBlockAlign bytes of audio, which for PCM audio is the just the sample rate.
        /// </summary>
        public int Scale;

        /// <summary>
        /// See <see cref="Scale"/>.
        /// </summary>
        public int Rate;

        /// <summary>
        /// Specifies the starting time for this stream. The units are defined by the <see cref="Rate"/> and
        /// <see cref="Scale"/> members in the main file header. Usually, this is zero, but it can specify a
        /// delay time for a stream that does not start concurrently with the file.
        /// </summary>
        public int Start;

        /// <summary>
        /// Specifies the length of this stream. The units are defined by the <see cref="Rate"/> and
        /// <see cref="Scale"/> members of the stream's header.
        /// </summary>
        public int Length;

        /// <summary>
        /// Specifies how large a buffer should be used to read this stream. Typically, this contains a
        /// value corresponding to the largest chunk present in the stream. Using the correct buffer size
        /// makes playback more efficient. Use zero if you do not know the correct buffer size.
        /// </summary>
        public int SuggestedBufferSize;

        /// <summary>
        /// Specifies an indicator of the quality of the data in the stream. Quality is represented as a number
        /// between 0 and 10,000. For compressed data, this typically represents the value of the quality
        /// parameter passed to the compression software. If set to –1, drivers use the default quality value.
        /// </summary>
        public int Quality;

        /// <summary>
        /// Specifies the size of a single sample of data. This is set to zero if the samples can vary in size.
        /// If this number is nonzero, then multiple samples of data can be grouped into a single chunk within
        /// the file. If it is zero, each sample of data (such as a video frame) must be in a separate chunk.
        /// For video streams, this number is typically zero, although it can be nonzero if all video frames are
        /// the same size. For audio streams, this number should be the same as the nBlockAlign member of the
        /// WAVEFORMATEX structure describing the audio.
        /// </summary>
        public int SampleSize;

        /// <summary>
        /// Gets the upper-left corner for the video stream.
        /// </summary>
        public short Left;

        /// <summary>
        /// Gets the upper-left corner for the video stream.
        /// </summary>
        public short Top;

        /// <summary>
        /// Gets the lower-right corner of the video stream.
        /// </summary>
        public short Right;

        /// <summary>
        /// Gets the lower-right corner of the video stream.
        /// </summary>
        public short Bottom;
    }
}
