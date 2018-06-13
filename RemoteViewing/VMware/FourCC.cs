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

namespace RemoteViewing.VMware
{
    /// <summary>
    /// Well-known FourCC codes.
    /// </summary>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd375802(v=vs.85).aspx"/>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd318189(v=vs.85).aspx"/>
    internal enum FourCC : int
    {
        /// <summary>
        /// The RIFF header.
        /// </summary>
        Riff = 0x46464952,

        /// <summary>
        /// The 'hdrl' list defines the format of the data and is the first required LIST chunk.
        /// </summary>
        Hdrl = 0x6c726468,

        /// <summary>
        /// AVI files are identified by the FOURCC 'AVI ' in the RIFF header.
        /// </summary>
        Avi = 0x20495641,

        /// <summary>
        /// The 'hdrl' list begins with the main AVI header, which is contained in an 'avih' chunk.
        /// </summary>
        Avih = 0x68697661,

        /// <summary>
        /// One or more 'strl' lists follow the main header. A 'strl' list is required for each data stream.
        /// Each 'strl' list contains information about one stream in the file.
        /// </summary>
        Strl = 0x6c727473,

        /// <summary>
        /// A stream header chunk ('strh')
        /// </summary>
        Strh = 0x68727473,

        /// <summary>
        /// A stream format chunk ('strf')
        /// </summary>
        Strf = 0x66727473,

        /// <summary>
        /// A stream name chunk ('strn').
        /// </summary>
        Strn = 0x6e727473,

        /// <summary>
        /// A video stream.
        /// </summary>
        Vids = 0x73646976,

        /// <summary>
        /// The OpenDML Index
        /// </summary>
        Indx = 0x78646e69,

        /// <summary>
        /// An AVI list.
        /// </summary>
        List = 0x5453494c,

        /// <summary>
        /// The OpenDML list.
        /// </summary>
        Odml = 0x6c6d646f,

        /// <summary>
        /// The OpenDML header
        /// </summary>
        Dmlh = 0x686c6d64,

        /// <summary>
        /// Data can be aligned in an AVI file by inserting 'JUNK' chunks as needed. Applications should ignore the contents of a 'JUNK' chunk.
        /// </summary>
        Junk = 0x4b4e554a,

        /// <summary>
        /// The 'movi' list contains the data for the AVI sequence.
        /// </summary>
        Movi = 0x69766f6d,

        /// <summary>
        /// Compressed video frame.
        /// </summary>
        Dc = 0x63640000,

        /// <summary>
        /// First compressed video frame.
        /// </summary>
        Dc00 = 0x63643030,

        /// <summary>
        /// Standard index block.
        /// </summary>
        Ix = 0x00007869,

        /// <summary>
        /// The VMware codec.
        /// </summary>
        VMnc = 0x636e4d56,
    }
}
