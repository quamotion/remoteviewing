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
using System.Net.Sockets;
using System.Windows.Forms;

namespace RemoteViewing.Example
{
    /// <summary>
    /// The main form used in this sample application.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (this.vncControl.Client.IsConnected)
            {
                this.vncControl.Client.Close();
            }
            else
            {
                var hostname = this.txtHostname.Text.Trim();
                if (hostname == string.Empty)
                {
                    MessageBox.Show(
                        this,
                        "Hostname isn't set.",
                        "Hostname",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                int port;
                if (!int.TryParse(this.txtPort.Text, out port) || port < 1 || port > 65535)
                {
                    MessageBox.Show(
                        this,
                        "Port must be between 1 and 65535.",
                        "Port",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var options = new Vnc.VncClientConnectOptions();
                if (this.txtPassword.Text != string.Empty)
                {
                    options.Password = this.txtPassword.Text.ToCharArray();
                }

                try
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        try
                        {
                            this.vncControl.Client.Connect(hostname, port, options);
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                        }
                    }
                    catch (Vnc.VncException ex)
                    {
                        MessageBox.Show(
                            this,
                            "Connection failed (" + ex.Reason.ToString() + ").",
                            "Connect",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show(
                            this,
                            "Connection failed (" + ex.SocketErrorCode.ToString() + ").",
                            "Connect",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    this.vncControl.Focus();
                }
                finally
                {
                    if (options.Password != null)
                    {
                        Array.Clear(options.Password, 0, options.Password.Length);
                    }
                }
            }
        }

        private void OnConnected(object sender, EventArgs e)
        {
            this.btnConnect.Text = "Close";
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.btnConnect.Text = "Connect";
        }

        private void OnConnectionFailed(object sender, EventArgs e)
        {
        }
    }
}
