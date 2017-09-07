using System.Runtime.InteropServices;

namespace RemoteViewing.Utility
{
    /// <summary>
    /// Class that holds functions from user32.dll
    /// </summary>
    public static class User32
    {
        /// <summary>
        /// Function that can be used to simulate a mouse.
        /// </summary>
        /// <param name="dwFlags">Flags</param>
        /// <param name="dx">X-coordinate</param>
        /// <param name="dy">Y-coordinate</param>
        /// <param name="dwData">Data</param>
        /// <param name="dwExtraInfo">Extra info for X buttons</param>
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void MouseEvent(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /// <summary>
        /// Function that sets the position of the pointer.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Success code</returns>
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
    }
}
