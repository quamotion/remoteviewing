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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RemoteViewing.Windows.Forms
{
    /// <summary>
    /// Displays the framebuffer sent from a VNC server, and allows input to be sent back.
    /// </summary>
    [Category("Network")]
    [Description("Displays the framebuffer sent from a VNC server, and allows input to be sent back.")]
    public partial class VncControl : UserControl
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names must not contain underscore", Justification = "P/Invoke conventions")]
        private const int WM_CLIPBOARDUPDATE = 0x31d;

        private int buttons;
        private int x;
        private int y;

        private Bitmap bitmap;
        private VncClient client;
        private string expectedClipboard = string.Empty;
        private HashSet<KeySym> keysyms = new HashSet<KeySym>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VncControl"/> class.
        /// </summary>
        public VncControl()
        {
            this.AllowInput = true;
            this.AllowRemoteCursor = true;
            this.Client = new VncClient();

            this.InitializeComponent();
        }

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

        private enum TransformDirection
        {
            FromDevice,
            ToDevice
        }

        /// <summary>
        /// Gets or sets a value indicating whether the local cursor is shown, when showing the remote cursor is activated. (false by default)
        /// </summary>
        public bool HideLocalCursor { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="VncClient"/> being interacted with.
        ///
        /// By default, this is a new instance.
        /// Call <see cref="VncClient.Connect(string, int, VncClientConnectOptions)"/>
        /// on it to get things up and running quickly.
        /// </summary>
        public VncClient Client
        {
            get
            {
                return this.client;
            }

            set
            {
                if (this.client == value)
                {
                    return;
                }

                if (this.client != null)
                {
                    this.client.Bell -= this.HandleBell;
                    this.client.Connected -= this.HandleConnected;
                    this.client.ConnectionFailed -= this.HandleConnectionFailed;
                    this.client.Closed -= this.HandleClosed;
                    this.client.FramebufferChanged -= this.HandleFramebufferChanged;
                    this.client.RemoteClipboardChanged -= this.HandleRemoteClipboardChanged;
                }

                this.client = value;
                if (this.client != null)
                {
                    this.client.Bell += this.HandleBell;
                    this.client.Connected += this.HandleConnected;
                    this.client.ConnectionFailed += this.HandleConnectionFailed;
                    this.client.Closed += this.HandleClosed;
                    this.client.FramebufferChanged += this.HandleFramebufferChanged;
                    this.client.RemoteClipboardChanged += this.HandleRemoteClipboardChanged;
                }

                this.ClearInputState();
                this.UpdateFramebuffer(true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control should send input to the server, or act only as a viewer.
        ///
        /// By default, this is <c>true</c>.
        /// </summary>
        public bool AllowInput
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the local cursor is allowed to be hidden.
        ///
        /// By default, this is <c>true</c>.
        /// </summary>
        public bool AllowRemoteCursor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether clipboard changes on the remote VNC server will alter the local clipboard.
        /// </summary>
        public bool AllowClipboardSharingFromServer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether local clipboard changes will be sent to the remote VNC server.
        /// </summary>
        public bool AllowClipboardSharingToServer
        {
            get;
            set;
        }

        private float ScaleFactor { get; set; } = 1.0f;

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
                    if (this.client != null && clipboard != this.expectedClipboard)
                    {
                        this.expectedClipboard = clipboard;
                        this.client.SendLocalClipboardChange(clipboard);
                    }
                }
            }

            base.WndProc(ref m);
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.ClearInputState();
        }

        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            this.x = e.X;
            this.y = e.Y;
            if (e.Delta < 0)
            {
                this.SendMouseScroll(false);
            }
            else if (e.Delta > 0)
            {
                this.SendMouseScroll(true);
            }
        }

        /// <summary>
        /// Invalidate the graphic to avoid strange artifacts when resizing the control.
        /// </summary>
        /// <param name="e">The empty event args.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.ScaleFactor = this.GetScaleFactor(this.client?.Framebuffer);

            this.Invalidate();
        }

        [DllImport("user32", EntryPoint = "AddClipboardFormatListener", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr handle);

        [DllImport("user32", EntryPoint = "RemoveClipboardFormatListener", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr handle);

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

        private void ClearInputState()
        {
            this.buttons = 0;
            foreach (var keysym in this.keysyms)
            {
                this.SendKeyUpdate(keysym, false);
            }

            this.keysyms.Clear();
        }

        private void UpdateFramebuffer(bool force, VncFramebuffer framebuffer)
        {
            if (framebuffer == null)
            {
                return;
            }

            int w = framebuffer.Width, h = framebuffer.Height;

            if (this.bitmap == null || this.bitmap.Width != w || this.bitmap.Height != h || force)
            {
                this.bitmap = new Bitmap(w, h, PixelFormat.Format32bppRgb);
                VncBitmap.CopyFromFramebuffer(framebuffer, new VncRectangle(0, 0, w, h), this.bitmap, 0, 0);

                this.ScaleFactor = this.GetScaleFactor(framebuffer);
                this.Invalidate();
            }
        }

        private void UpdateFramebuffer(bool force)
        {
            if (this.client == null)
            {
                return;
            }

            var framebuffer = this.client.Framebuffer;
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
                    this.expectedClipboard = string.Empty;
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

                    if (this.client == null)
                    {
                        return;
                    }

                    var framebuffer = this.client.Framebuffer;
                    if (framebuffer == null)
                    {
                        return;
                    }

                    lock (framebuffer.SyncRoot)
                    {
                        this.UpdateFramebuffer(false, framebuffer);

                        if (this.bitmap != null)
                        {
                            for (int i = 0; i < e.RectangleCount; i++)
                            {
                                var rect = e.GetRectangle(i);
                                VncBitmap.CopyFromFramebuffer(framebuffer, rect, this.bitmap, rect.X, rect.Y);
                            }
                        }
                    }

                    for (int i = 0; i < e.RectangleCount; i++)
                    {
                        var rect = e.GetRectangle(i);
                        var transformedRect = this.Transform(
                            new Rectangle(rect.X, rect.Y, rect.Width, rect.Height),
                            TransformDirection.FromDevice);
                        this.Invalidate(transformedRect);
                    }
                }));
        }

        private void HandleRemoteClipboardChanged(object sender, RemoteClipboardChangedEventArgs e)
        {
            if (this.AllowClipboardSharingFromServer)
            {
                if (e.Contents.Length != 0 && this.expectedClipboard != e.Contents)
                {
                    try
                    {
                        Clipboard.SetText(e.Contents);
                        this.expectedClipboard = e.Contents;
                    }
                    catch (ExternalException)
                    {
                    }
                }
            }
        }

        private void SendKeyUpdate(KeySym keysym, bool pressed)
        {
            if (this.client != null && this.AllowInput)
            {
                this.client.SendKeyEvent(keysym, pressed);
            }
        }

        private void SendMouseUpdate()
        {
            if (this.client != null && this.AllowInput)
            {
                var devicePoint = this.TransformPoint(this.x, this.y, TransformDirection.ToDevice);
                this.client.SendPointerEvent(devicePoint.X, devicePoint.Y, this.buttons);
            }
        }

        private Rectangle Transform(Rectangle rectangle, TransformDirection direction)
        {
            var upperLeft = this.TransformPoint(rectangle.Left, rectangle.Top, direction);
            var bottomRight = this.TransformPoint(rectangle.Right, rectangle.Bottom, direction);
            return Rectangle.FromLTRB(upperLeft.X, upperLeft.Y, bottomRight.X, bottomRight.Y);
        }

        private Point TransformPoint(int xPosition, int yPosition, TransformDirection direction)
        {
            var scaleFactor = this.ScaleFactor;
            if (scaleFactor >= 1.0f)
            {
                return new Point(xPosition, yPosition);
            }

            scaleFactor = direction == TransformDirection.ToDevice ? 1.0f / scaleFactor : scaleFactor;

            var transformedPoint = new Point((int)(xPosition * scaleFactor), (int)(yPosition * scaleFactor));
            return transformedPoint;
        }

        private float GetScaleFactor(VncFramebuffer framebuffer)
        {
            if (framebuffer == null)
            {
                return 1.0f;
            }

            var scaleFactor = this.GetScaleFactor(framebuffer.Width, framebuffer.Height, this.Width, this.Height);
            return scaleFactor;
        }

        private float GetScaleFactor(int remoteWidth, int remoteHeight, int controlWidth, int controlHeight)
        {
            var widthScaleFactor = (float)controlWidth / remoteWidth;
            var heightScaleFactor = (float)controlHeight / remoteHeight;
            var scaleFactor = Math.Min(widthScaleFactor, heightScaleFactor);
            var decreaseScaleFactor = scaleFactor > 1.0f ? 1.0f : scaleFactor;
            return decreaseScaleFactor;
        }

        private void VncControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.AllowInput)
            {
                e.IsInputKey = true;
            }
        }

        private void VncControl_KeyDown(object sender, KeyEventArgs e)
        {
            var keysym = VncKeysym.FromKeyCode(e.KeyCode);
            if (keysym < 0)
            {
                return;
            }

            this.SendKeyUpdate(keysym, true);
            this.keysyms.Add(keysym);
        }

        private void VncControl_KeyUp(object sender, KeyEventArgs e)
        {
            var keysym = VncKeysym.FromKeyCode(e.KeyCode);
            if (keysym < 0)
            {
                return;
            }

            this.SendKeyUpdate(keysym, false);
            this.keysyms.Remove(keysym);
        }

        private void VncControl_MouseEnter(object sender, EventArgs e)
        {
            if (this.AllowRemoteCursor && this.HideLocalCursor)
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
            this.x = e.X;
            this.y = e.Y;
            this.buttons |= GetMouseMask(e.Button);
            this.SendMouseUpdate();
        }

        private void VncControl_MouseUp(object sender, MouseEventArgs e)
        {
            this.x = e.X;
            this.y = e.Y;
            this.buttons &= ~GetMouseMask(e.Button);
            this.SendMouseUpdate();
        }

        private void VncControl_MouseMove(object sender, MouseEventArgs e)
        {
            this.x = e.X;
            this.y = e.Y;
            this.SendMouseUpdate();
        }

        private void SendMouseScroll(bool down)
        {
            int mask = down ? 1 << 4 : 1 << 3;
            this.buttons |= mask;
            this.SendMouseUpdate();
            this.buttons &= ~mask;
            this.SendMouseUpdate();
        }

        private void VncControl_Paint(object sender, PaintEventArgs e)
        {
            if (this.bitmap == null)
            {
                return;
            }

            var scaleFactor = this.ScaleFactor;
            if (scaleFactor < 1.0)
            {
                e.Graphics.ScaleTransform((float)scaleFactor, (float)scaleFactor);
            }

            e.Graphics.DrawImageUnscaled(this.bitmap, 0, 0);
        }
    }
}
#endif
