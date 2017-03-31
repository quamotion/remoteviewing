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

        private const int WM_CLIPBOARDUPDATE = 0x31d;

        private int _buttons;

        private int _x;

        private int _y;

        private Bitmap _bitmap;
        private VncClient _client;
        private string _expectedClipboard = string.Empty;
        private HashSet<int> _keysyms = new HashSet<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VncControl"/>.
        /// </summary>
        public VncControl()
        {
            this.AllowInput = true;
            this.AllowRemoteCursor = true;
            this.Client = new VncClient();

            this.InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.DesignMode)
            {
                try
                {
                    AddClipboardFormatListener(this.Handle);
                }
                catch
                {
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!this.DesignMode)
            {
                try
                {
                    RemoveClipboardFormatListener(this.Handle);
                }
                catch
                {
                }
            }

            base.OnHandleDestroyed(e);
        }

        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            if (this.AllowClipboardSharingToServer && m.Msg == WM_CLIPBOARDUPDATE)
            {
                string clipboard = string.Empty;
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
                    if (this._client != null && clipboard != this._expectedClipboard)
                    {
                        this._expectedClipboard = clipboard;
                        this._client.SendLocalClipboardChange(clipboard);
                    }
                }
            }

            base.WndProc(ref m);
        }

        private void ClearInputState()
        {
            this._buttons = 0;
            foreach (var keysym in this._keysyms)
            {
                this.SendKeyUpdate(keysym, false);
            }

            this._keysyms.Clear();
        }

        private void UpdateFramebuffer(bool force, VncFramebuffer framebuffer)
        {
            if (framebuffer == null)
            {
                return;
            }

            int w = framebuffer.Width, h = framebuffer.Height;

            if (this._bitmap == null || this._bitmap.Width != w || this._bitmap.Height != h || force)
            {
                this._bitmap = new Bitmap(w, h, PixelFormat.Format32bppRgb);
                VncBitmap.CopyFromFramebuffer(framebuffer, new VncRectangle(0, 0, w, h), this._bitmap, 0, 0);
                this.ClientSize = new Size(w, h);
                this.Invalidate();
            }
        }

        private void UpdateFramebuffer(bool force)
        {
            if (this._client == null)
            {
                return;
            }

            var framebuffer = this._client.Framebuffer;
            this.UpdateFramebuffer(force, framebuffer);
        }

        private void HandleBell(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play();
        }

        private void HandleConnected(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
                {
                    this._expectedClipboard = string.Empty;
                    this.ClearInputState();

                    var ev = this.Connected;
                    if (ev != null)
                    {
                        ev(this, EventArgs.Empty);
                    }
                }));
        }

        private void HandleConnectionFailed(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.ClearInputState();

                var ev = this.ConnectionFailed;
                if (ev != null)
                {
                    ev(this, EventArgs.Empty);
                }
            }));
        }

        private void HandleClosed(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
                {
                    this.ClearInputState();

                    var ev = this.Closed;
                    if (ev != null)
                    {
                        ev(this, EventArgs.Empty);
                    }
                }));
        }

        private void HandleFramebufferChanged(object sender, FramebufferChangedEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
                {
                    if (this.DesignMode)
                    {
                        return;
                    }

                    if (this._client == null)
                    {
                        return;
                    }

                    var framebuffer = this._client.Framebuffer;
                    if (framebuffer == null)
                    {
                        return;
                    }

                    lock (framebuffer.SyncRoot)
                    {
                        this.UpdateFramebuffer(false, framebuffer);

                        if (this._bitmap != null)
                        {
                            for (int i = 0; i < e.RectangleCount; i++)
                            {
                                var rect = e.GetRectangle(i);
                                VncBitmap.CopyFromFramebuffer(framebuffer, rect, this._bitmap, rect.X, rect.Y);
                            }
                        }
                    }

                    for (int i = 0; i < e.RectangleCount; i++)
                    {
                        var rect = e.GetRectangle(i);
                        this.Invalidate(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
                    }
                }));
        }

        private void HandleRemoteClipboardChanged(object sender, RemoteClipboardChangedEventArgs e)
        {
            if (this.AllowClipboardSharingFromServer)
            {
                if (e.Contents.Length != 0 && this._expectedClipboard != e.Contents)
                {
                    try
                    {
                        Clipboard.SetText(e.Contents);
                        this._expectedClipboard = e.Contents;
                    }
                    catch (ExternalException)
                    {
                    }
                }
            }
        }

        private static int GetMouseMask(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left: return 1 << 0;
                case MouseButtons.Middle: return 1 << 1;
                case MouseButtons.Right: return 1 << 2;
                default: return 0;
            }
        }

        private void SendKeyUpdate(int keysym, bool pressed)
        {
            if (this._client != null && this.AllowInput)
            {
                this._client.SendKeyEvent(keysym, pressed);
            }
        }

        private void SendMouseUpdate()
        {
            if (this._client != null && this.AllowInput)
            {
                this._client.SendPointerEvent(this._x, this._y, this._buttons);
            }
        }

        private void VncControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.AllowInput)
            {
                e.IsInputKey = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.ClearInputState();
        }

        private void VncControl_KeyDown(object sender, KeyEventArgs e)
        {
            int keysym = VncKeysym.FromKeyCode(e.KeyCode);
            if (keysym < 0)
            {
                return;
            }

            this.SendKeyUpdate(keysym, true);
            this._keysyms.Add(keysym);
        }

        private void VncControl_KeyUp(object sender, KeyEventArgs e)
        {
            int keysym = VncKeysym.FromKeyCode(e.KeyCode);
            if (keysym < 0)
            {
                return;
            }

            this.SendKeyUpdate(keysym, false);
            this._keysyms.Remove(keysym);
        }

        private void VncControl_MouseEnter(object sender, EventArgs e)
        {
            if (this.AllowRemoteCursor)
            {
                Cursor.Hide();
            }
        }

        private void VncControl_MouseLeave(object sender, EventArgs e)
        {
            if (this.AllowRemoteCursor)
            {
                Cursor.Show();
            }
        }

        private void VncControl_MouseDown(object sender, MouseEventArgs e)
        {
            this._x = e.X;
            this._y = e.Y;
            this._buttons |= GetMouseMask(e.Button);
            this.SendMouseUpdate();
        }

        private void VncControl_MouseUp(object sender, MouseEventArgs e)
        {
            this._x = e.X;
            this._y = e.Y;
            this._buttons &= ~GetMouseMask(e.Button);
            this.SendMouseUpdate();
        }

        private void VncControl_MouseMove(object sender, MouseEventArgs e)
        {
            this._x = e.X;
            this._y = e.Y;
            this.SendMouseUpdate();
        }

        private void SendMouseScroll(bool down)
        {
            int mask = down ? (1 << 4) : (1 << 3);
            this._buttons |= mask;
            this.SendMouseUpdate();
            this._buttons &= ~mask;
            this.SendMouseUpdate();
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            this._x = e.X;
            this._y = e.Y;
            if (e.Delta < 0)
            {
                this.SendMouseScroll(false);
            }
            else if (e.Delta > 0)
            {
                this.SendMouseScroll(true);
            }
        }

        private void VncControl_Paint(object sender, PaintEventArgs e)
        {
            if (this._bitmap == null)
            {
                return;
            }

            e.Graphics.DrawImageUnscaled(this._bitmap, 0, 0);
        }

        [DllImport("user32", EntryPoint = "AddClipboardFormatListener", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr handle);

        [DllImport("user32", EntryPoint = "RemoveClipboardFormatListener", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr handle);

        /// <summary>
        /// The <see cref="VncClient"/> being interacted with.
        ///
        /// By default, this is a new instance.
        /// Call <see cref="VncClient.Connect(string, int, VncClientConnectOptions)"/>
        /// on it to get things up and running quickly.
        /// </summary>
        public VncClient Client
        {
            get
            {
                return this._client;
            }

            set
            {
                if (this._client == value)
                {
                    return;
                }

                if (this._client != null)
                {
                    this._client.Bell -= this.HandleBell;
                    this._client.Connected -= this.HandleConnected;
                    this._client.ConnectionFailed -= this.HandleConnectionFailed;
                    this._client.Closed -= this.HandleClosed;
                    this._client.FramebufferChanged -= this.HandleFramebufferChanged;
                    this._client.RemoteClipboardChanged -= this.HandleRemoteClipboardChanged;
                }

                this._client = value;
                if (this._client != null)
                {
                    this._client.Bell += this.HandleBell;
                    this._client.Connected += this.HandleConnected;
                    this._client.ConnectionFailed += this.HandleConnectionFailed;
                    this._client.Closed += this.HandleClosed;
                    this._client.FramebufferChanged += this.HandleFramebufferChanged;
                    this._client.RemoteClipboardChanged += this.HandleRemoteClipboardChanged;
                }

                this.ClearInputState();
                this.UpdateFramebuffer(true);
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
