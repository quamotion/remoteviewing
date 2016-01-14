namespace RemoteViewing.Example
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            RemoteViewing.Vnc.VncClient vncClient1 = new RemoteViewing.Vnc.VncClient();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.topTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblHostname = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtHostname = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.vncControl = new RemoteViewing.Windows.Forms.VncControl();
            this.mainTableLayoutPanel.SuspendLayout();
            this.topTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.AutoSize = true;
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(this.topTableLayoutPanel, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.vncControl, 0, 1);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 2;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(636, 452);
            this.mainTableLayoutPanel.TabIndex = 0;
            // 
            // topTableLayoutPanel
            // 
            this.topTableLayoutPanel.AutoSize = true;
            this.topTableLayoutPanel.ColumnCount = 7;
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00001F));
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.99998F));
            this.topTableLayoutPanel.Controls.Add(this.txtPassword, 6, 0);
            this.topTableLayoutPanel.Controls.Add(this.lblPassword, 5, 0);
            this.topTableLayoutPanel.Controls.Add(this.btnConnect, 0, 0);
            this.topTableLayoutPanel.Controls.Add(this.lblHostname, 1, 0);
            this.topTableLayoutPanel.Controls.Add(this.lblPort, 3, 0);
            this.topTableLayoutPanel.Controls.Add(this.txtHostname, 2, 0);
            this.topTableLayoutPanel.Controls.Add(this.txtPort, 4, 0);
            this.topTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.topTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topTableLayoutPanel.Name = "topTableLayoutPanel";
            this.topTableLayoutPanel.RowCount = 1;
            this.topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topTableLayoutPanel.Size = new System.Drawing.Size(636, 29);
            this.topTableLayoutPanel.TabIndex = 0;
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Location = new System.Drawing.Point(543, 3);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(90, 20);
            this.txtPassword.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPassword.Location = new System.Drawing.Point(481, 0);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(13, 0, 3, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 29);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnConnect
            // 
            this.btnConnect.AutoSize = true;
            this.btnConnect.Location = new System.Drawing.Point(3, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(57, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblHostname
            // 
            this.lblHostname.AutoSize = true;
            this.lblHostname.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHostname.Location = new System.Drawing.Point(76, 0);
            this.lblHostname.Margin = new System.Windows.Forms.Padding(13, 0, 3, 0);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(58, 29);
            this.lblHostname.TabIndex = 1;
            this.lblHostname.Text = "Hostname:";
            this.lblHostname.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPort.Location = new System.Drawing.Point(341, 0);
            this.lblPort.Margin = new System.Windows.Forms.Padding(13, 0, 3, 0);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 29);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            this.lblPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtHostname
            // 
            this.txtHostname.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHostname.Location = new System.Drawing.Point(140, 3);
            this.txtHostname.Name = "txtHostname";
            this.txtHostname.Size = new System.Drawing.Size(185, 20);
            this.txtHostname.TabIndex = 1;
            this.txtHostname.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            this.txtPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPort.Location = new System.Drawing.Point(376, 3);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(89, 20);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "5900";
            // 
            // vncControl
            // 
            this.vncControl.AllowClipboardSharingFromServer = true;
            this.vncControl.AllowClipboardSharingToServer = true;
            this.vncControl.AllowInput = true;
            this.vncControl.AllowRemoteCursor = true;
            this.vncControl.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.vncControl.BackColor = System.Drawing.Color.Black;
            vncClient1.MaxUpdateRate = 15D;
            vncClient1.UserData = null;
            this.vncControl.Client = vncClient1;
            this.vncControl.Location = new System.Drawing.Point(201, 154);
            this.vncControl.Name = "vncControl";
            this.vncControl.Size = new System.Drawing.Size(234, 173);
            this.vncControl.TabIndex = 4;
            this.vncControl.Connected += new System.EventHandler(this.vncControl_Connected);
            this.vncControl.ConnectionFailed += new System.EventHandler(this.vncControl_ConnectionFailed);
            this.vncControl.Closed += new System.EventHandler(this.vncControl_Closed);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 452);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Name = "MainForm";
            this.Text = "RemoteViewing - Example VNC Client";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.topTableLayoutPanel.ResumeLayout(false);
            this.topTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel topTableLayoutPanel;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblHostname;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtHostname;
        private System.Windows.Forms.TextBox txtPort;
        private Windows.Forms.VncControl vncControl;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
    }
}