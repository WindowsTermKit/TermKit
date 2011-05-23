using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Console
{
    public static class ClientIcons
    {
        [DllImport("shell32.dll", EntryPoint = "ExtractIconA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);

        public static byte[] GetPathIcon(string path)
        {
            string iconid = null;

            try
            {
                if (Directory.Exists(path))
                    iconid = "Folder";
                else
                {
                    // Get the extension of the path.
                    string ext = Path.GetExtension(path);

                    // Find out the image identifier from the Windows registry.
                    RegistryKey handler = Registry.ClassesRoot.OpenSubKey(ext);
                    iconid = (string)handler.GetValue("");
                    if (iconid == null)
                        return ClientResources.GetResourceData("Images/file_big.png");
                }

                // Get the actual icon from the registry.
                return ClientIcons.GetIconByRegistryID(iconid);
            }
            catch (NullReferenceException e)
            {
                return ClientResources.GetResourceData("Images/file_big.png");
            }
        }

        public static byte[] GetDefaultIcon(string ext)
        {
            string iconid = null;

            try
            {
                if (ext == "...")
                    iconid = "Folder";
                else
                {
                    // Find out the image identifier from the Windows registry.
                    RegistryKey handler = Registry.ClassesRoot.OpenSubKey("." + ext);
                    iconid = (string)handler.GetValue("");
                    if (iconid == null)
                        return ClientResources.GetResourceData("Images/file_big.png");
                }

                // Get the actual icon from the registry.
                return ClientIcons.GetIconByRegistryID(iconid);
            }
            catch (NullReferenceException e)
            {
                return ClientResources.GetResourceData("Images/file_big.png");
            }
        }

        private static byte[] GetIconByRegistryID(string iconid)
        {
            // Now find the actual path to the image.
            string[] s = ((string)Registry.ClassesRoot.OpenSubKey(iconid).OpenSubKey("DefaultIcon").GetValue("")).Split(new char[] { ',' });
            s[0] = s[0].Trim('"');
            if (s[0] == "%1") { s = new string[] { "%SystemRoot%\\system32\\shell32.dll", "2" }; }
            else if (s.Length == 1) { s = new string[] { s[0], "0" }; }
            string iconpath = s[0];
            int iconindex = Convert.ToInt32(s[1]);

            // Now get the icon.
            IntPtr ptr = ClientIcons.ExtractIcon(0, iconpath, iconindex);
            Icon ico = Icon.FromHandle(ptr);

            // Translate the icon data into PNG.
            Bitmap b = ico.ToBitmap();
            MemoryStream ms = new MemoryStream();
            b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }
    }
}
