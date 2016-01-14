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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (vncControl.Client.IsConnected)
            {
                vncControl.Client.Close();
            }
            else
            {
                var hostname = txtHostname.Text.Trim();
                if (hostname == "")
                {
                    MessageBox.Show(this, "Hostname isn't set.", "Hostname",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int port;
                if (!int.TryParse(txtPort.Text, out port) || port < 1 || port > 65535)
                {
                    MessageBox.Show(this, "Port must be between 1 and 65535.", "Port",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var options = new Vnc.VncClientConnectOptions();
                if (txtPassword.Text != "") { options.Password = txtPassword.Text.ToCharArray(); }

                try
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        try { vncControl.Client.Connect(hostname, port, options); }
                        finally { Cursor = Cursors.Default; }
                    }
                    catch (Vnc.VncException ex)
                    {
                        MessageBox.Show(this,
                                        "Connection failed (" + ex.Reason.ToString() + ").",
                                        "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show(this,
                                        "Connection failed (" + ex.SocketErrorCode.ToString() + ").",
                                        "Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    vncControl.Focus();
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

        private void vncControl_Connected(object sender, EventArgs e)
        {
            btnConnect.Text = "Close";
        }

        private void vncControl_Closed(object sender, EventArgs e)
        {
            btnConnect.Text = "Connect";
        }

        private void vncControl_ConnectionFailed(object sender, EventArgs e)
        {

        }
    }
}
