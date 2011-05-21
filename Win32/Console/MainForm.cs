using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebKit.JSCore;
using System.Web;

namespace Console
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Set the initial document content for the TermKit window.
            this.c_WebKit.Navigate("application:///index.html");
        }

        private void c_WebKit_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            return;
        }

        private void c_WebKit_ResourceRequested(object sender, WebKit.ResourceRequestedEventArgs e)
        {
            System.Diagnostics.Debugger.Log(1, "Resource", "The resource " + e.Request + " was requested.\r\n");
            Uri uri = new Uri(e.Request);

            switch (uri.Scheme + ":///")
            {
                case "application:///":
                    e.Response.Override = true;
                    e.Response.ContentData = ClientResources.GetResourceData(uri.AbsolutePath);
                    e.Response.ContentType = ClientResources.GetResourceType(uri.AbsolutePath);
                    return;
                case "termkit-icon-default:///":
                    e.Response.Override = true;
                    e.Response.ContentData = ClientIcons.GetDefaultIcon();
                    e.Response.ContentType = "image/png";
                    return;
                case "termkit-icon-preview:///":
                    e.Response.Override = true;
                    e.Response.ContentData = ClientIcons.GetPathIcon(HttpUtility.UrlDecode(uri.AbsolutePath).Substring(1));
                    e.Response.ContentType = "image/png";
                    return;
            }
        }

        private void c_WebKit_ShowJavaScriptAlertPanel(object sender, WebKit.ShowJavaScriptAlertPanelEventArgs e)
        {
            MessageBox.Show(e.Message, "Javascript");
        }

        private void c_WebKit_ShowJavaScriptConfirmPanel(object sender, WebKit.ShowJavaScriptConfirmPanelEventArgs e)
        {
            e.ReturnValue = (MessageBox.Show(e.Message, "Javascript", MessageBoxButtons.YesNo) == DialogResult.Yes);
        }

        private void c_WebKit_ShowJavaScriptPromptPanel(object sender, WebKit.ShowJavaScriptPromptPanelEventArgs e)
        {
            MessageBox.Show("Prompt requested!", "Javascript");
        }
    }
}
