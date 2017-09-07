using RemoteViewing.Utility;
using System;

namespace RemoteViewing.Vnc.Server
{
    /// <summary>
    /// WinApiMouseEventFlags
    /// </summary>
    [Flags]
    public enum WinApiMouseEventFlags : int
    {
        /// <summary>
        /// LEFTDOWN
        /// </summary>
        LEFTDOWN = 0x00000002,

        /// <summary>
        /// LEFTUP
        /// </summary>
        LEFTUP = 0x00000004,

        /// <summary>
        /// MIDDLEDOWN
        /// </summary>
        MIDDLEDOWN = 0x00000020,

        /// <summary>
        /// MIDDLEUP
        /// </summary>
        MIDDLEUP = 0x00000040,

        /// <summary>
        /// MOVE
        /// </summary>
        MOVE = 0x00000001,

        /// <summary>
        /// ABSOLUTE
        /// </summary>
        ABSOLUTE = 0x00008000,

        /// <summary>
        /// RIGHTDOWN
        /// </summary>
        RIGHTDOWN = 0x00000008,

        /// <summary>
        /// RIGHTUP
        /// </summary>
        RIGHTUP = 0x00000010,

        /// <summary>
        /// WHEEL
        /// </summary>
        WHEEL = 0x00000800,

        /// <summary>
        /// XDOWN
        /// </summary>
        XDOWN = 0x00000080,

        /// <summary>
        /// XUP
        /// </summary>
        XUP = 0x00000100
    }

    /// <summary>
    /// X11MouseEventFlags
    /// </summary>
    [Flags]
    public enum X11MouseEventFlags : byte
    {
        /// <summary>
        /// LEFTDOWN
        /// </summary>
        LEFTDOWN = 1,

        /// <summary>
        /// MIDDLEDOWN
        /// </summary>
        MIDDLEDOWN = 2,

        /// <summary>
        /// RIGHTDOWN
        /// </summary>
        RIGHTDOWN = 4,

        /// <summary>
        /// SCROLLDOWN
        /// </summary>
        SCROLLDOWN = 8,

        /// <summary>
        /// SCROLLUP
        /// </summary>
        SCROLLUP = 16
    }

    /// <summary>
    /// This class can be used to add Windows mouse functionality to the VNC server part.
    /// </summary>
    public class VncMouse
    {
        /*
         * The way VNC clients send the mouse state to the server is not directly compatible
         * with the way Windows mouse control works through it's API. VNC uses the X11
         * convention where the state of the mouse is send (i.e. left mouse button IS down)
         * rather then sending the changes in the state of the mouse (i.e. left mouse button
         * changed from unpressed to pressed) which is the way the Windows API works. Therefor
         * the field X11MouseState is used to keep track of the state to detect changes.
         */
        private byte x11MouseState;

        /// <summary>
        /// Initializes a new instance of the <see cref="VncMouse"/> class.
        /// Class that provides virtual mouse functionality to the VNC server.
        /// </summary>
        public VncMouse()
        {
        }

        /// <summary>
        /// Callback function for mouse updates.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        public void OnMouseUpdate(object sender, PointerChangedEventArgs e)
        {
            byte newState = (byte)e.PressedButtons;

            User32.SetCursorPos(e.X, e.Y);

            // Case left button pressed
            if (!this.IsButtonDown(this.x11MouseState, X11MouseEventFlags.LEFTDOWN) &&
                this.IsButtonDown(newState, X11MouseEventFlags.LEFTDOWN))
            {
                this.x11MouseState = this.SetButtonDown(this.x11MouseState, X11MouseEventFlags.LEFTDOWN);

                User32.MouseEvent((int)WinApiMouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
            }

            // Case left button released
            if (this.IsButtonDown(this.x11MouseState, X11MouseEventFlags.LEFTDOWN) &&
                !this.IsButtonDown(newState, X11MouseEventFlags.LEFTDOWN))
            {
                this.x11MouseState = this.SetButtonUp(this.x11MouseState, X11MouseEventFlags.LEFTDOWN);

                User32.MouseEvent((int)WinApiMouseEventFlags.LEFTUP, 0, 0, 0, 0);
            }

            // Case left button pressed
            if (!this.IsButtonDown(this.x11MouseState, X11MouseEventFlags.RIGHTDOWN) &&
                this.IsButtonDown(newState, X11MouseEventFlags.RIGHTDOWN))
            {
                this.x11MouseState = this.SetButtonDown(this.x11MouseState, X11MouseEventFlags.RIGHTDOWN);

                User32.MouseEvent((int)WinApiMouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
            }

            // Case left button released
            if (this.IsButtonDown(this.x11MouseState, X11MouseEventFlags.RIGHTDOWN) &&
                !this.IsButtonDown(newState, X11MouseEventFlags.RIGHTDOWN))
            {
                this.x11MouseState = this.SetButtonUp(this.x11MouseState, X11MouseEventFlags.RIGHTDOWN);

                User32.MouseEvent((int)WinApiMouseEventFlags.RIGHTUP, 0, 0, 0, 0);
            }

            // Case middle button pressed
            if (!this.IsButtonDown(this.x11MouseState, X11MouseEventFlags.MIDDLEDOWN) &&
                this.IsButtonDown(newState, X11MouseEventFlags.MIDDLEDOWN))
            {
                this.x11MouseState = this.SetButtonDown(this.x11MouseState, X11MouseEventFlags.MIDDLEDOWN);

                User32.MouseEvent((int)WinApiMouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);
            }

            // Case middle button released
            if (this.IsButtonDown(this.x11MouseState, X11MouseEventFlags.MIDDLEDOWN) &&
                !this.IsButtonDown(newState, X11MouseEventFlags.MIDDLEDOWN))
            {
                this.x11MouseState = this.SetButtonUp(this.x11MouseState, X11MouseEventFlags.MIDDLEDOWN);

                User32.MouseEvent((int)WinApiMouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
            }

            // Case scroll up
            if (this.IsButtonDown(newState, X11MouseEventFlags.SCROLLDOWN))
            {
                User32.MouseEvent((int)WinApiMouseEventFlags.WHEEL, 0, 0, 120, 0);
            }

            // Case scroll down
            if (this.IsButtonDown(newState, X11MouseEventFlags.SCROLLUP))
            {
                User32.MouseEvent((int)WinApiMouseEventFlags.WHEEL, 0, 0, -120, 0);
            }
        }

        private bool IsButtonDown(byte state, X11MouseEventFlags flag)
        {
            return (state & (byte)flag) != 0;
        }

        private byte SetButtonDown(byte state, X11MouseEventFlags flags)
        {
            return state |= (byte)flags;
        }

        private byte SetButtonUp(byte state, X11MouseEventFlags flags)
        {
            return state &= (byte)~flags;
        }
    }
}
