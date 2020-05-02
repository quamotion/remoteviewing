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
    /// Defines the capabilities with which a build of libvncserver has been compiled.
    /// </summary>
    public struct NativeCapabilities
    {
        /// <summary>
        /// The default Unix (Linux and macOS) capabilities.
        /// </summary>
        public static NativeCapabilities Unix = new NativeCapabilities()
        {
            HaveLibZ = true,
            HaveLibPng = true,
            HaveLibJpeg = true,
            HaveLibPthread = true,
        };

        /// <summary>
        /// libvncserver has been compiled with libz support.
        /// </summary>
        public bool HaveLibZ;

        /// <summary>
        /// libvncserver has been compiled with libpng support.
        /// </summary>
        public bool HaveLibPng;

        /// <summary>
        /// libvncserver has been compiled with libjpeg support.
        /// </summary>
        public bool HaveLibJpeg;

        /// <summary>
        /// libvncserver has been compiled with WIN32 threading support.
        /// </summary>
        public bool HaveWin32Threads;

        /// <summary>
        /// libvncserver has been compile with libpthread support.
        /// </summary>
        public bool HaveLibPthread;
    }
}
