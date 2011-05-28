using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Drawing;

namespace Console
{
    public static class ClientResources
    {
        private static ZipFile m_ClientZIP = new ZipFile(new MemoryStream(Properties.Resources.ClientData));

        public static byte[] GetResourceData(string path)
        {
            if (File.Exists("../../../../HTML/" + path))
            {
                System.Diagnostics.Debugger.Log(1, "Resource", "The resource application://termkit" + path + " was loaded from disk (as " + ClientResources.GetResourceType(path) + ").\r\n");

                // Use the most up-to-date version of the file.
                BinaryReader reader = new BinaryReader(File.Open("../../../../HTML/" + path, FileMode.Open));
                List<byte> data = new List<byte>();
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                    data.Add(reader.ReadByte());
                return data.ToArray();
            }
            else
            {
                System.Diagnostics.Debugger.Log(1, "Resource", "The resource application://termkit" + path + " was loaded from ZIP (as " + ClientResources.GetResourceType(path) + ").\r\n");

                // Read the resources from the ZIP file.
                ZipEntry entry = ClientResources.m_ClientZIP.GetEntry(ZipEntry.CleanName(path));
                if (entry == null)
                    return new byte[] { };

                Stream stream = ClientResources.m_ClientZIP.GetInputStream(entry);
                int buffer = -1;
                List<byte> data = new List<byte>();
                while ((buffer = stream.ReadByte()) != -1)
                    data.Add((byte)buffer);
                return data.ToArray();
            }
        }

        public static string GetResourceType(string path)
        {
            if (path.LastIndexOf('.') == -1) return "text/plain";
            switch (path.Substring(path.LastIndexOf('.')))
            {
                case ".js": return "text/javascript";
                case ".css": return "text/css";
                case ".html": return "text/html";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                default: return "text/plain";
            }
        }
    }
}
