using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Console
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Set the initial document content for the TermKit window.
            this.c_WebKit.Navigate("local:init");
        }

        private void c_WebKit_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = true;
            if (e.Url.ToString() == "local:init")
                this.c_WebKit.DocumentText = Console.Properties.Resources.HTMLInitializing;
            else
                e.Cancel = false;
        }
    }
}
