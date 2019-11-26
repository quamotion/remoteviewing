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

#if NET461
using RemoteViewing.Vnc;
using System;
using System.Windows.Forms;

namespace RemoteViewing.Windows.Forms
{
    /// <summary>
    /// Helps with Windows Forms keyboard interaction.
    /// </summary>
    /// <remarks>
    /// See http://www.realvnc.com/docs/rfbproto.pdf for common keys.
    /// </remarks>
    public static class VncKeysym
    {
        /// <summary>
        /// Converts Windows Forms <see cref="Keys"/> to X11 keysyms.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The keysym.</returns>
        public static KeySym FromKeyCode(Keys key)
        {
            switch (key)
            {
                case Keys.Back: return KeySym.Backspace;
                case Keys.Tab: return KeySym.Tab;
                case Keys.Enter: return KeySym.Return;
                case Keys.Escape: return KeySym.Escape;
                case Keys.Insert: return KeySym.Insert;
                case Keys.Delete: return KeySym.Delete;
                case Keys.Home: return KeySym.Home;
                case Keys.End: return KeySym.End;
                case Keys.PageUp: return KeySym.PageUp;
                case Keys.PageDown: return KeySym.PageDown;
                case Keys.Left: return KeySym.Left;
                case Keys.Up: return KeySym.Up;
                case Keys.Right: return KeySym.Right;
                case Keys.Down: return KeySym.Down;
                case Keys.F1: return KeySym.F1;
                case Keys.F2: return KeySym.F2;
                case Keys.F3: return KeySym.F3;
                case Keys.F4: return KeySym.F4;
                case Keys.F5: return KeySym.F5;
                case Keys.F6: return KeySym.F6;
                case Keys.F7: return KeySym.F7;
                case Keys.F8: return KeySym.F8;
                case Keys.F9: return KeySym.F9;
                case Keys.F10: return KeySym.F10;
                case Keys.F11: return KeySym.F11;
                case Keys.F12: return KeySym.F12;
                case Keys.F13: return KeySym.F13;
                case Keys.F14: return KeySym.F14;
                case Keys.F15: return KeySym.F15;
                case Keys.F16: return KeySym.F16;
                case Keys.F17: return KeySym.F17;
                case Keys.F18: return KeySym.F18;
                case Keys.F19: return KeySym.F19;
                case Keys.F20: return KeySym.F20;
                case Keys.F21: return KeySym.F21;
                case Keys.F22: return KeySym.F22;
                case Keys.F23: return KeySym.F23;
                case Keys.F24: return KeySym.F24;
                case Keys.ShiftKey: return KeySym.ShiftRight;
                case Keys.ControlKey: return KeySym.ControlRight;
                case Keys.Menu: return KeySym.AltRight;
                /* <-- It appears Windows Forms doesn't send LShiftKey and RShiftKey. Instead it only sends ShiftKey...
                case Keys.LShiftKey: return 0xffe1;
                case Keys.RShiftKey: return 0xffe2;
                case Keys.LControlKey: return 0xffe3;
                case Keys.RControlKey: return 0xffe4;
                case Keys.LMenu: return 0xffe9;
                case Keys.RMenu: return 0xffea;
                */
                case Keys.OemOpenBrackets: return KeySym.BracketLeft;
                case Keys.OemCloseBrackets: return KeySym.Bracketright;
                case (Keys)220: return KeySym.Backslash;
                case Keys.Oemcomma: return KeySym.Comma;
                case Keys.OemPeriod: return KeySym.Period;
                case Keys.OemSemicolon: return KeySym.Semicolon;
                case Keys.OemQuestion: return KeySym.Question;
                case Keys.OemMinus:
                case Keys.Subtract: return KeySym.Minus;
                case Keys.Oemplus:
                case Keys.Add: return KeySym.Plus;
                case Keys.OemQuotes: return KeySym.Apostrophe;
                case Keys.Oemtilde: return KeySym.AsciiTilde;
                case Keys.Pause: return KeySym.Pause;
                case Keys.Scroll: return KeySym.ScrollLock;
                case Keys.PrintScreen: return KeySym.Print;
                case Keys.NumPad0: return KeySym.NumPad0;
                case Keys.NumPad1: return KeySym.NumPad1;
                case Keys.NumPad2: return KeySym.NumPad2;
                case Keys.NumPad3: return KeySym.NumPad3;
                case Keys.NumPad4: return KeySym.NumPad4;
                case Keys.NumPad5: return KeySym.NumPad5;
                case Keys.NumPad6: return KeySym.NumPad6;
                case Keys.NumPad7: return KeySym.NumPad7;
                case Keys.NumPad8: return KeySym.NumPad8;
                case Keys.NumPad9: return KeySym.NumPad9;
                case Keys.A: return KeySym.A;
                case Keys.B: return KeySym.B;
                case Keys.C: return KeySym.C;
                case Keys.D: return KeySym.D;
                case Keys.E: return KeySym.E;
                case Keys.F: return KeySym.F;
                case Keys.G: return KeySym.G;
                case Keys.H: return KeySym.H;
                case Keys.I: return KeySym.I;
                case Keys.J: return KeySym.J;
                case Keys.K: return KeySym.K;
                case Keys.L: return KeySym.L;
                case Keys.M: return KeySym.M;
                case Keys.N: return KeySym.N;
                case Keys.O: return KeySym.O;
                case Keys.P: return KeySym.P;
                case Keys.Q: return KeySym.Q;
                case Keys.R: return KeySym.R;
                case Keys.S: return KeySym.S;
                case Keys.T: return KeySym.T;
                case Keys.U: return KeySym.U;
                case Keys.V: return KeySym.V;
                case Keys.W: return KeySym.W;
                case Keys.X: return KeySym.X;
                case Keys.Y: return KeySym.Y;
                case Keys.Z: return KeySym.Z;
                default:
                    // TODO: This is wildly incomplete.
                    return (KeySym)key;
            }
        }
    }
}
#endif
