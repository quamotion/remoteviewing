namespace RemoteViewing.Windows.Forms
{
#if NET461
    partial class VncControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // VncControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.Name = "VncControl";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.VncControl_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VncControl_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VncControl_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VncControl_MouseDown);
            this.MouseEnter += new System.EventHandler(this.VncControl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.VncControl_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VncControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VncControl_MouseUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.VncControl_PreviewKeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
#endif
}
