﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;

namespace Console.Protocols
{
    public class TermKitIconDefaultProtocolFactory : ISchemeHandlerFactory
    {
        public ISchemeHandler Create()
        {
            return new TermKitIconDefaultProtocol();
        }
    }
}
