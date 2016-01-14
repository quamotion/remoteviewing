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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RemoteViewing.Vnc;

namespace RemoteViewing.Windows.Forms
{
    /// <summary>
    /// Displays the framebuffer sent from a VNC server, and allows input to be sent back.
    /// </summary>
    [Category("Network")]
    [Description("Displays the framebuffer sent from a VNC server, and allows input to be sent back.")]
    public partial class VncControl : UserControl
    {
        /// <summary>
        /// Occurs when the VNC client has successfully connected to the remote server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Occurs when the VNC client has failed to connect to the remote server.
        /// </summary>
        public event EventHandler ConnectionFailed;

        /// <summary>
        /// Occurs when the VNC client is disconnected.
        /// </summary>
        public event EventHandler Closed;

        const int WM_CLIPBOARDUPDATE = 0x31d;

        int _buttons, _x, _y;

        Bitmap _bitmap;
        VncClient _client;
        string _expectedClipboard = "";
        HashSet<int> _keysyms = new HashSet<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VncControl"/>.
        /// </summary>
        public VncControl()
        {
            AllowInput = true;
            AllowRemoteCursor = true;
            Client = new VncClient();

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                try { AddClipboardFormatListener(Handle); }
                catch { }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!DesignMode)
            {
                try { RemoveClipboardFormatListener(Handle); }
                catch { }
            }

            base.OnHandleDestroyed(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (AllowClipboardSharingToServer && m.Msg == WM_CLIPBOARDUPDATE)
            {
                string clipboard = "";
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        clipboard = Clipboard.GetText();
                    }
                }
                catch (ExternalException)
                {

                }

                if (clipboard.Length != 0)
                {
                    if (_client != null && clipboard != _expectedClipboard)
                    {
                        _expectedClipboard = clipboard;
                        _client.SendLocalClipboardChange(clipboard);
                    }
                }
            }

            base.WndProc(ref m);
        }

        void ClearInputState()
        {
            _buttons = 0;
            foreach (var keysym in _keysyms) { SendKeyUpdate(keysym, false); }
            _keysyms.Clear();
        }

        void UpdateFramebuffer(bool force, VncFramebuffer framebuffer)
        {
            if (framebuffer == null) { return; }
            int w = framebuffer.Width, h = framebuffer.Height;

            if (_bitmap == null || _bitmap.Width != w || _bitmap.Height != h || force)
            {
                _bitmap = new Bitmap(w, h, PixelFormat.Format32bppRgb);
                VncBitmap.CopyFromFramebuffer(framebuffer, new VncRectangle(0, 0, w, h), _bitmap, 0, 0);
                ClientSize = new Size(w, h); Invalidate();
            }
        }

        void UpdateFramebuffer(bool force)
        {
            if (_client == null) { return; }

            var framebuffer = _client.Framebuffer;
            UpdateFramebuffer(force, framebuffer);
        }

        void HandleBell(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play();
        }

        void HandleConnected(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
                {
                    _expectedClipboard = "";
                    ClearInputState();

                    var ev = Connected;
                    if (ev != null) { ev(this, EventArgs.Empty); }
                }));
        }

        void HandleConnectionFailed(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                ClearInputState();

                var ev = ConnectionFailed;
                if (ev != null) { ev(this, EventArgs.Empty); }
            }));
        }

        void HandleClosed(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
                {
                    ClearInputState();

                    var ev = Closed;
                    if (ev != null) { ev(this, EventArgs.Empty); }
                }));
        }

        void HandleFramebufferChanged(object sender, FramebufferChangedEventArgs e)
        {
            BeginInvoke(new Action(() =>
                {
                    if (DesignMode) { return; }

                    if (_client == null) { return; }

                    var framebuffer = _client.Framebuffer;
                    if (framebuffer == null) { return; }

                    lock (framebuffer.SyncRoot)
                    {
                        UpdateFramebuffer(false, framebuffer);

                        if (_bitmap != null)
                        {
                            for (int i = 0; i < e.RectangleCount; i++)
                            {
                                var rect = e.GetRectangle(i);
                                VncBitmap.CopyFromFramebuffer(framebuffer, rect, _bitmap, rect.X, rect.Y);
                            }
                        }
                    }

                    for (int i = 0; i < e.RectangleCount; i++)
                    {
                        var rect = e.GetRectangle(i);
                        Invalidate(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
                    }
                }));
        }

        void HandleRemoteClipboardChanged(object sender, RemoteClipboardChangedEventArgs e)
        {
            if (AllowClipboardSharingFromServer)
            {
                if (e.Contents.Length != 0 && _expectedClipboard != e.Contents)
                {
                    try
                    {
                        Clipboard.SetText(e.Contents);
                        _expectedClipboard = e.Contents;
                    }
                    catch (ExternalException)
                    {

                    }
                }
            }
        }

        static int GetMouseMask(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left: return 1 << 0;
                case MouseButtons.Middle: return 1 << 1;
                case MouseButtons.Right: return 1 << 2;
                default: return 0;
            }
        }

        void SendKeyUpdate(int keysym, bool pressed)
        {
            if (_client != null && AllowInput) { _client.SendKeyEvent(keysym, pressed); }
        }

        void SendMouseUpdate()
        {
            if (_client != null && AllowInput) { _client.SendPointerEvent(_x, _y, _buttons); }
        }

        private void VncControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (AllowInput) { e.IsInputKey = true; }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            ClearInputState();
        }

        private void VncControl_KeyDown(object sender, KeyEventArgs e)
        {
            int keysym = VncKeysym.FromKeyCode(e.KeyCode); if (keysym < 0) { return; }
            SendKeyUpdate(keysym, true); _keysyms.Add(keysym);
        }

        private void VncControl_KeyUp(object sender, KeyEventArgs e)
        {
            int keysym = VncKeysym.FromKeyCode(e.KeyCode); if (keysym < 0) { return; }
            SendKeyUpdate(keysym, false); _keysyms.Remove(keysym);
        }

        private void VncControl_MouseEnter(object sender, EventArgs e)
        {
            if (AllowRemoteCursor) { Cursor.Hide(); }
        }

        private void VncControl_MouseLeave(object sender, EventArgs e)
        {
            if (AllowRemoteCursor) { Cursor.Show(); }
        }

        private void VncControl_MouseDown(object sender, MouseEventArgs e)
        {
            _x = e.X; _y = e.Y; _buttons |= GetMouseMask(e.Button);
            SendMouseUpdate();
        }

        private void VncControl_MouseUp(object sender, MouseEventArgs e)
        {
            _x = e.X; _y = e.Y; _buttons &= ~GetMouseMask(e.Button);
            SendMouseUpdate();
        }

        private void VncControl_MouseMove(object sender, MouseEventArgs e)
        {
            _x = e.X; _y = e.Y;
            SendMouseUpdate();
        }

        void SendMouseScroll(bool down)
        {
            int mask = down ? (1 << 4) : (1 << 3);
            _buttons |= mask; SendMouseUpdate();
            _buttons &= ~mask; SendMouseUpdate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            _x = e.X; _y = e.Y;
            if (e.Delta < 0) { SendMouseScroll(false); }
            else if (e.Delta > 0) { SendMouseScroll(true); }
        }

        private void VncControl_Paint(object sender, PaintEventArgs e)
        {
            if (_bitmap == null) { return; }
            e.Graphics.DrawImageUnscaled(_bitmap, 0, 0);
        }

        [DllImport("user32", EntryPoint = "AddClipboardFormatListener", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr handle);

        [DllImport("user32", EntryPoint = "RemoveClipboardFormatListener", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr handle);

        /// <summary>
        /// The <see cref="VncClient"/> being interacted with.
        /// 
        /// By default, this is a new instance.
        /// Call <see cref="VncClient.Connect(string, int, VncClientConnectOptions)"/>
        /// on it to get things up and running quickly.
        /// </summary>
        public VncClient Client
        {
            get { return _client; }
            set
            {
                if (_client == value) { return; }

                if (_client != null)
                {
                    _client.Bell -= HandleBell;
                    _client.Connected -= HandleConnected;
                    _client.ConnectionFailed -= HandleConnectionFailed;
                    _client.Closed -= HandleClosed;
                    _client.FramebufferChanged -= HandleFramebufferChanged;
                    _client.RemoteClipboardChanged -= HandleRemoteClipboardChanged;
                }

                _client = value;

                if (_client != null)
                {
                    _client.Bell += HandleBell;
                    _client.Connected += HandleConnected;
                    _client.ConnectionFailed += HandleConnectionFailed;
                    _client.Closed += HandleClosed;
                    _client.FramebufferChanged += HandleFramebufferChanged;
                    _client.RemoteClipboardChanged += HandleRemoteClipboardChanged;
                }

                ClearInputState();
                UpdateFramebuffer(true);
            }
        }

        /// <summary>
        /// Whether the control should send input to the server, or act only as a viewer.
        /// 
        /// By default, this is <c>true</c>.
        /// </summary>
        public bool AllowInput
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the local cursor is allowed to be hidden.
        /// 
        /// By default, this is <c>true</c>.
        /// </summary>
        public bool AllowRemoteCursor
        {
            get;
            set;
        }

        /// <summary>
        /// If enabled, clipboard changes on the remote VNC server will alter the local clipboard.
        /// </summary>
        public bool AllowClipboardSharingFromServer
        {
            get;
            set;
        }

        /// <summary>
        /// If enabled, local clipboard changes will be sent to the remote VNC server.
        /// </summary>
        public bool AllowClipboardSharingToServer
        {
            get;
            set;
        }
    }
}
