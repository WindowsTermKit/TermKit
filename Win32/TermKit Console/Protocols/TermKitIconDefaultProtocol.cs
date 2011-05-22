using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;
using System.IO;

namespace Console.Protocols
{
    public class TermKitIconDefaultProtocol : ISchemeHandler
    {
        public bool ProcessRequest(IRequest request, ref string mimeType, ref System.IO.Stream stream)
        {
            Uri uri = new Uri(request.Url.Replace("://", ":///").TrimEnd(new char[] { '/' }));

            // Handle the termkit-icon-default:/// protocol.
            mimeType = "image/png";
            stream = new MemoryStream(ClientIcons.GetDefaultIcon(uri.AbsolutePath.TrimStart(new char[] { '/' })));

            return true;
        }
    }
}
