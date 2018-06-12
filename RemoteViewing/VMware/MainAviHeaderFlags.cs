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

using System;

namespace RemoteViewing.VMware
{
    /// <summary>
    /// Flags for the <see cref="AviMainHeader"/> struct.
    /// </summary>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd318180(v=vs.85).aspx"/>
    [Flags]
    internal enum MainHeaderFlags : uint
    {
        /// <summary>
        /// Indicates the AVI file has an index.
        /// </summary>
        HasIndex = 0x10,

        /// <summary>
        /// Indicates that application should use the index, rather than the physical ordering of
        /// the chunks in the file, to determine the order of presentation of the data. For example,
        /// this flag could be used to create a list of frames for editing.
        /// </summary>
        MustUseIndex = 0x20,

        /// <summary>
        /// Indicates the AVI file is interleaved.
        /// </summary>
        IsInterleaved = 0x100,

        /// <summary>
        /// Use Chunk Type to find key frames
        /// </summary>
        TrustChunkType = 0x800,

        /// <summary>
        /// Indicates the AVI file is a specially allocated file used for capturing real-time video. Applications
        /// should warn the user before writing over a file with this flag set because the user probably
        /// defragmented this file.
        /// </summary>
        WasCaptureFile = 0x10000,

        /// <summary>
        /// Indicates the AVI file contains copyrighted data and software. When this flag is used, software should
        /// not permit the data to be duplicated.
        /// </summary>
        Copyrighted = 0x200000,
    }
}
