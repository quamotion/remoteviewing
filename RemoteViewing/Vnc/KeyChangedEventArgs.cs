#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/remoteviewing>
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

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// Provides data for the <see cref="Server.VncServerSession.KeyChanged"/> event.
    /// </summary>
    public class KeyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="keysym">The X11 keysym of the key. For many keys this is the ASCII value.</param>
        /// <param name="pressed"><c>true</c> for a key press event, or <c>false</c> for a key release event.</param>
        [CLSCompliant(false)]
        public KeyChangedEventArgs(KeySym keysym, bool pressed)
        {
            this.Keysym = keysym;
            this.Pressed = pressed;
        }

        /// <summary>
        /// Gets the X11 keysym of the key. For many keys this is the ASCII value.
        /// </summary>
        [CLSCompliant(false)]
        public KeySym Keysym
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether a key was pressed.
        /// </summary>
        /// <value>
        /// <c>true</c> for a key press event, or <c>false</c> for a key release event.
        /// </value>
        public bool Pressed
        {
            get;
            private set;
        }
    }
}
