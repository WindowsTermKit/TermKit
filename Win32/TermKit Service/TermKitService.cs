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
        Process m_NodeJS = null;

        public TermKitService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (this.m_NodeJS == null)
            {
            }
        }

        protected override void OnStop()
        {
        }
    }
}
