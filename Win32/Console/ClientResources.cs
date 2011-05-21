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
        //private static ZipFile m_ClientZIP = new ZipFile(new MemoryStream(Properties.Resources.ClientData));

        public static byte[] GetResourceData(string path)
        {
            System.Diagnostics.Debugger.Log(1, "Resource", "The resource application://" + path + " was loaded (as " + ClientResources.GetResourceType(path) + ").\r\n");

            // Read the file as binary data.
            BinaryReader reader = new BinaryReader(File.Open("../../../../../HTML/" + path, FileMode.Open));
            // TODO: Using an integer for length means we can't read big files...
            byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
            return data;
            /*

            ZipEntry entry = ClientResources.m_ClientZIP.GetEntry(path);
            if (entry == null)
                return "";

            Stream stream = ClientResources.m_ClientZIP.GetInputStream(entry);
            int buffer = -1;
            string data = "";
            while ((buffer = stream.ReadByte()) != -1)
                data += Convert.ToChar(buffer);
            return data;*/
        }

        public static string GetResourceType(string path)
        {
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
