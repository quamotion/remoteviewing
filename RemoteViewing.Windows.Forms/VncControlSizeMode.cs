#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2016 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
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

namespace RemoteViewing.Windows.Forms
{
    /// <summary>
    /// Specifies how the remote screen is positioned and sized in the <see cref="VncControl"/>.
    /// </summary>
    public enum VncControlSizeMode
    {
        /// <summary>
        /// The <see cref="VncControl"/> will contain the upper-left portion of the screen.
        /// </summary>
        Clip,

        /// <summary>
        /// The screen is resized to fit the <see cref="VncControl"/>. The aspect ratio is allowed to be incorrect.
        /// </summary>
        Stretch,

        /// <summary>
        /// The <see cref="VncControl"/> is resized to fit the screen.
        /// </summary>
        AutoSize,

        /// <summary>
        /// The <see cref="VncControl"/> will contain the center portion of the screen.
        /// </summary>
        Center,

        /// <summary>
        /// The screen is resized to fit the <see cref="VncControl"/>. Correct aspect ratio is maintained.
        /// </summary>
        Zoom
    }
}
