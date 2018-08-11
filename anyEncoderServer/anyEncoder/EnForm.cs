namespace anyEncoder
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class EnForm : Form
    {
        private IContainer components = null;
        private encoder en;
        private TextBox logbox;
        public int taskid;

        public EnForm()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EnForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.en.close();
            }
            catch
            {
            }
        }

        private void EnForm_Load(object sender, EventArgs e)
        {
            if (func.chkdog2() || func.chksn())
            {
                this.en = new encoder(this.taskid);
                this.en.formobj = this;
                this.en.logbox = this.logbox;
            }
        }

        private void InitializeComponent()
        {
            this.logbox = new TextBox();
            base.SuspendLayout();
            this.logbox.Location = new Point(0, 0);
            this.logbox.Multiline = true;
            this.logbox.Name = "logbox";
            this.logbox.ScrollBars = ScrollBars.Vertical;
            this.logbox.Size = new Size(0x1ee, 290);
            this.logbox.TabIndex = 0;
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.ClientSize = new Size(0x1ee, 290);
            base.Controls.Add(this.logbox);
            base.MinimizeBox = false;
            base.Name = "EnForm";
            this.Text = "编码进程";
            base.FormClosed += new FormClosedEventHandler(this.EnForm_FormClosed);
            base.Load += new EventHandler(this.EnForm_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

