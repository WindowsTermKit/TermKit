using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;

namespace Server
{
    public class ConsoleWebServer
    {
        private HttpListener m_Listener = null;
        private Thread m_Thread = null;
        private bool m_Running = false;

        public ConsoleWebServer()
        {
            this.m_Running = true;
            this.m_Thread = new Thread(this.Listen);
            this.m_Thread.Start();
        }

        private void Listen()
        {
            // Set up the server.
            this.m_Listener = new HttpListener();
            this.m_Listener.Prefixes.Add("http://localhost:33333/");
            this.m_Listener.Start();

            // Continously check for requests while we want to run.
            while (this.m_Running)
            {
                HttpListenerContext context = this.m_Listener.GetContext();
                Thread t = new Thread(() =>
                {
                    this.HandleRequest(context);
                });
                t.Start();
            }

            // Stop the server.
            this.m_Listener.Stop();
        }

        private string DetectContentType(string path)
        {
            switch (path.Substring(path.LastIndexOf('.')))
            {
                case ".html":
                case ".htm": return "text/html";
                case ".js": return "text/javascript";
                case ".css": return "text/css";
                default: return "text/plain";
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            // Detect the path.
            string path = context.Request.Url.AbsolutePath;

            // Find the data to return for this request.
            try
            {
                StreamReader reader = new StreamReader("C:/Server Storage/Projects/TermKit/HTML/" + path, Encoding.UTF8);
                string data = reader.ReadToEnd();
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.ContentType = this.DetectContentType(path);
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.Close();
            }
            catch (FileNotFoundException e)
            {
                return;
            }
        }

        public void Stop()
        {
            this.m_Running = false;
        }
    }
}
