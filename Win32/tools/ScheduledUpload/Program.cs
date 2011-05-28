using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using System.Text;
using ICSharpCode.SharpZipLib.Core;

namespace Nightly_Upload_Tool
{
    public static class Program
    {
        private static NotifyIcon c_Icon;
        private static ContextMenuStrip c_Context;
        private static Thread c_UploadThread;
        private delegate void EmptyDelegate();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // Set up the program.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create the context menu.
            Program.c_Context = new ContextMenuStrip();
            Program.c_Context.Items.Add("Stop Upload", null, Program.c_Context_StopUpload_Click);

            // Create the system tray icon for controlling the app while we upload.
            Program.c_Icon = new NotifyIcon();
            Program.c_Icon.Icon = Properties.Resources.TrayIcon;
            Program.c_Icon.ContextMenuStrip = Program.c_Context;
            Program.c_Icon.Visible = true;
            Program.c_Icon.ShowBalloonTip(3000, "Uploading Nightly Build...", "The nightly build of TermKit for Windows is now being uploaded to the server.", ToolTipIcon.Info);

            // Create the thread and run it.
            Program.c_UploadThread = new Thread(Program.c_UploadThread_Run);
            Program.c_UploadThread.Start();

            // Run the application.
            Application.Run();
        }

        /// <summary>
        /// Stops the build upload.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void c_Context_StopUpload_Click(object sender, EventArgs e)
        {
            Program.c_UploadThread.Abort();
            Program.c_Icon.Visible = false;
            Application.Exit();
        }

        /// <summary>
        /// Adds a set of files to the ZIP stream.
        /// </summary>
        /// <param name="zip">The ZIP stream.</param>
        /// <param name="pattern">The filename pattern to match.</param>
        private static void AddFiles(ZipOutputStream zip, string pattern)
        {
            foreach (FileInfo i in (new DirectoryInfo(Environment.CurrentDirectory).GetFiles(pattern)))
            {
                if (i.Name.EndsWith(".vshost.exe"))
                    continue;

                Program.SetStatus("Compressing " + i.Name + "...");

                ZipEntry ze = new ZipEntry(ZipEntry.CleanName(i.Name));
                ze.DateTime = i.LastWriteTime;
                ze.Size = i.Length;
                zip.PutNextEntry(ze);

                byte[] buffer = new byte[4096];
                using (FileStream reader = File.OpenRead(i.FullName))
                {
                    StreamUtils.Copy(reader, zip, buffer);
                }
                zip.CloseEntry();
            }
        }

        private static void SetStatus(string p)
        {
            Program.c_Icon.Text = p;
        }

        /// <summary>
        /// Uploads the build data to the server.
        /// </summary>
        private static void c_UploadThread_Run()
        {
            // Initalize the process.
            WebClient client = new WebClient();
            MemoryStream stream = new MemoryStream();
            ZipOutputStream zip = new ZipOutputStream(stream);
            zip.SetLevel(9);
            
            // Add all the files.
            Program.AddFiles(zip, "*.dll");
            Program.AddFiles(zip, "*.pdb");
            Program.AddFiles(zip, "*.exe");
            zip.IsStreamOwner = true;
            zip.Close();

            // Send the data.
            Program.SetStatus("Uploading " + ((double)stream.ToArray().Length / 1024 / 1024).ToString("F") + "MB...");
            client.UploadDataCompleted += new UploadDataCompletedEventHandler(client_UploadDataCompleted);
            client.UploadDataAsync(new Uri("http://www.redpointsoftware.com.au/builds/upload/termkit"), "PUT", stream.ToArray());
        }

        static void client_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            try
            {
                Program.SetStatus(Encoding.ASCII.GetString(e.Result));
                Program.SetStatus("Upload Complete.");
                Program.c_Icon.ShowBalloonTip(1000, "Upload Complete", "The nightly build of TermKit for Windows is complete.", ToolTipIcon.Info);
                Program.c_Icon.BalloonTipClosed += new EventHandler(c_Icon_BalloonTipClosed);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Program.SetStatus("Upload Failed.");
                Program.c_Icon.ShowBalloonTip(1000, "Upload Failed", "The nightly build of TermKit for Windows failed.", ToolTipIcon.Info);
                Program.c_Icon.BalloonTipClosed += new EventHandler(c_Icon_BalloonTipClosed);
            }
        }

        static void c_Icon_BalloonTipClosed(object sender, EventArgs e)
        {
            Program.c_Icon.Visible = false;
            Application.Exit();
        }
    }
}
