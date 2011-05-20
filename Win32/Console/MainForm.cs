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
            this.c_WebKit.Navigate("file:///C:/Server Storage/Projects/TermKit/HTML/index.html");//http://localhost:33333/index.html");
        }
    }
}
