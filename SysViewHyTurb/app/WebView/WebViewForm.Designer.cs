namespace SysViewHyTurb
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainwebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // MainwebBrowser
            // 
            this.MainwebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainwebBrowser.Location = new System.Drawing.Point(0, 0);
            this.MainwebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.MainwebBrowser.Name = "MainwebBrowser";
            this.MainwebBrowser.Size = new System.Drawing.Size(971, 504);
            this.MainwebBrowser.TabIndex = 0;
            this.MainwebBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MainwebBrowser_PreviewKeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 504);
            this.Controls.Add(this.MainwebBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "SysViewHyTurb";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser MainwebBrowser;
    }
}

