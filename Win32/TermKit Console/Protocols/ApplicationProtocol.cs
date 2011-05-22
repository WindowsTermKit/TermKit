using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;
using System.IO;

namespace Console.Protocols
{
    public class ApplicationProtocol : ISchemeHandler
    {
        public bool ProcessRequest(IRequest request, ref string mimeType, ref System.IO.Stream stream)
        {
            Uri uri = new Uri(request.Url);
            
            // Handle the application:/// protocol.
            mimeType = ClientResources.GetResourceType(uri.AbsolutePath);
            stream = new MemoryStream(ClientResources.GetResourceData(uri.AbsolutePath));

            return true;
        }
    }
}
