using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Console
{
    public static class ClientIcons
    {
        public static byte[] GetPathIcon(string path)
        {
            if (Directory.Exists(path))
                return ClientResources.GetResourceData("Images/file.png");
            else
                return ClientResources.GetResourceData("Images/folder.png");
        }

        public static byte[] GetDefaultIcon()
        {
            return ClientResources.GetResourceData("Images/file.png");
        }
    }
}
