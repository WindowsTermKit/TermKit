using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Server
{
    public partial class TermKitService : ServiceBase
    {
        private ConsoleWebServer m_FileServer = null;

        public TermKitService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (this.m_FileServer == null)
                this.m_FileServer = new ConsoleWebServer();
        }

        protected override void OnStop()
        {
            if (this.m_FileServer != null)
            {
                this.m_FileServer.Stop();
                this.m_FileServer = null;
            }
        }
    }
}
