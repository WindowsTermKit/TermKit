using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using CefSharp;
using Console.Protocols;
using System.Diagnostics;

namespace Console
{
    public partial class MainForm : Form, IBeforeResourceLoad
    {
        private CefWebBrowser c_WebBrowser = null;

        public MainForm()
        {
            InitializeComponent();

            // Create the new CefWebBrowser control.
            this.c_WebBrowser = new CefWebBrowser("application://termkit/index.html");
            this.c_WebBrowser.Dock = DockStyle.Fill;
            this.c_WebBrowser.Visible = true;
            this.c_WebBrowser.BeforeResourceLoadHandler = this;
            this.c_WebBrowser.ConsoleMessage += new ConsoleMessageEventHandler(c_WebBrowser_ConsoleMessage);
            this.Controls.Add(this.c_WebBrowser);

            // Register the schemes.
            CEF.RegisterScheme("application", new ApplicationProtocolFactory());
            CEF.RegisterScheme("termkit-icon-preview", new TermKitIconPreviewProtocolFactory());
            CEF.RegisterScheme("termkit-icon-default", new TermKitIconDefaultProtocolFactory());
        }

        void c_WebBrowser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            System.Diagnostics.Debugger.Log(1, "Console", e.Source + ":" + e.Line + " - " + e.Message + "\r\n");
        }

        public void HandleBeforeResourceLoad(CefWebBrowser browserControl, IRequestResponse requestResponse)
        {
            System.Diagnostics.Debugger.Log(1, "Resource", "The resource " + requestResponse.Request.Url + " was requested.\r\n");
        }
    }
}
