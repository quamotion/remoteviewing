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
    /// Provides data for the <see cref="Server.VncServerSession.PointerChanged"/> event.
    /// </summary>
    public class PointerChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointerChangedEventArgs"/> class.
        /// </summary>
        /// <param name="x">The X coordinate of the mouse.</param>
        /// <param name="y">The Y coordinate of the mouse.</param>
        /// <param name="pressedButtons">
        ///     A bit mask of pressed mouse buttons, in X11 convention: 1 is left, 2 is middle, and 4 is right.
        ///     Mouse wheel scrolling is treated as a button event: 8 for up and 16 for down.
        /// </param>
        public PointerChangedEventArgs(int x, int y, int pressedButtons)
        {
            this.X = x;
            this.Y = y;
            this.PressedButtons = pressedButtons;
        }

        /// <summary>
        /// Gets the X coordinate of the mouse.
        /// </summary>
        public int X
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Y coordinate of the mouse.
        /// </summary>
        public int Y
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets aA bit mask of pressed mouse buttons, in X11 convention: 1 is left, 2 is middle, and 4 is right.
        /// Mouse wheel scrolling is treated as a button event: 8 for up and 16 for down.
        /// </summary>
        public int PressedButtons
        {
            get;
            private set;
        }
    }
}
