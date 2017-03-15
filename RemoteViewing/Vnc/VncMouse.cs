using RemoteViewing.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteViewing.Vnc.Server {
    /// <summary>
    /// This class can be used to add Windows mouse functionality to the VNC server part. 
    /// </summary>
    public class VncMouse {
        /*
         * The way VNC clients send the mouse state to the server is not directly compatible
         * with the way Windows mouse control works through it's API. VNC uses the X11 
         * convention where the state of the mouse is send (i.e. left mouse button IS down) 
         * rather then sending the changes in the state of the mouse (i.e. left mouse button
         * changed from unpressed to pressed) which is the way the Windows API works. Therefor
         * the field X11MouseState is used to keep track of the state to detect changes.
         */
        private byte X11MouseState;

        public VncMouse() {

        }

        /// <summary>
        /// Callback function for mouse updates.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        public void OnMouseUpdate (object sender, PointerChangedEventArgs e) {
            byte newState = (byte) e.PressedButtons;
            uint winApiState = 0;

            // Case left button pressed
            if (!IsButtonDown(X11MouseState, X11MouseEventFlags.LEFTDOWN) &&
                IsButtonDown(newState, X11MouseEventFlags.LEFTDOWN)) {

                X11MouseState = SetButtonDown(X11MouseState, X11MouseEventFlags.LEFTDOWN);
                winApiState += (uint)WinApiMouseEventFlags.LEFTDOWN;
                //winApiState += (uint)WinApiMouseEventFlags.MOVE;

                Debug.WriteLine("Left mouse button pressed at {0}, {1}", e.X, e.Y);
            }

            // Case left button released
            if (IsButtonDown(X11MouseState, X11MouseEventFlags.LEFTDOWN) &&
                !IsButtonDown(newState, X11MouseEventFlags.LEFTDOWN)) {

                X11MouseState = SetButtonUp(X11MouseState, X11MouseEventFlags.LEFTDOWN);
                winApiState += (uint)WinApiMouseEventFlags.LEFTUP;
                //winApiState += (uint)WinApiMouseEventFlags.MOVE;

                Debug.WriteLine("Left mouse button released at {0}, {1}", e.X, e.Y);
            }

            User32.mouse_event(winApiState, (uint) e.X, (uint) e.Y, 0, 0);
        }

        //private uint MakeWinApiMouseFlags(byte ) {
        //
        //}

        private bool IsButtonDown(byte state, X11MouseEventFlags flag) {
            return (state & (byte)flag) != 0;
        }

        static byte SetButtonDown(byte state, X11MouseEventFlags flags) {
            return state |= (byte)flags;
        }

        static byte SetButtonUp(byte state, X11MouseEventFlags flags) {
            return state &= (byte)~flags;
        }
    }


    [Flags]
    enum WinApiMouseEventFlags : uint {
        LEFTDOWN = 0x00000002,
        LEFTUP = 0x00000004,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP = 0x00000040,
        MOVE = 0x00000001,
        ABSOLUTE = 0x00008000,
        RIGHTDOWN = 0x00000008,
        RIGHTUP = 0x00000010,
        WHEEL = 0x00000800,
        XDOWN = 0x00000080,
        XUP = 0x00000100
    }

    [Flags]
    enum X11MouseEventFlags : byte {
        LEFTDOWN = 1,
        MIDDLEDOWN = 2,
        RIGHTDOWN = 4,
        SCROLLDOWN = 8,
        SCROLLUP = 16
    }

    //Use the values of this enum for the 'dwData' parameter
    //to specify an X button when using MouseEventFlags.XDOWN or
    //MouseEventFlags.XUP for the dwFlags parameter.
    enum MouseEventDataXButtons : uint {
        XBUTTON1 = 0x00000001,
        XBUTTON2 = 0x00000002
    }

}
