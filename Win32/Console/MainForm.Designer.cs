namespace Console
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
            this.c_WebKit = new WebKit.WebKitBrowser();
            this.SuspendLayout();
            // 
            // c_WebKit
            // 
            this.c_WebKit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.c_WebKit.BackColor = System.Drawing.Color.Black;
            this.c_WebKit.Location = new System.Drawing.Point(0, 0);
            this.c_WebKit.Name = "c_WebKit";
            this.c_WebKit.Size = new System.Drawing.Size(806, 495);
            this.c_WebKit.TabIndex = 0;
            this.c_WebKit.Url = new System.Uri("", System.UriKind.Relative);
            this.c_WebKit.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.c_WebKit_Navigating);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 494);
            this.Controls.Add(this.c_WebKit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "TermKit (running on Windows 7 x86-mode)";
            this.ResumeLayout(false);

        }

        #endregion

        private WebKit.WebKitBrowser c_WebKit;



    }
}

