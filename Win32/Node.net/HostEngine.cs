using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IronJS.Hosting;
using IronJS.Compiler;
using IronJS;
using System.Threading;

namespace Node.net
{
    public class HostEngine
    {
        private CSharp.Context m_Context = null;
        private Dictionary<string, HostModule> m_Modules = null;

        public HostEngine()
        {
            // Set up the context.
            this.m_Context = new CSharp.Context();
            this.m_Context.CreatePrintFunction();

            // Register all of the available modules.
            this.m_Modules = new Dictionary<string, HostModule>();
            this.m_Modules.Add("assert",            new Modules.AssertModule(this.m_Context.Environment));
            this.m_Modules.Add("child_processes",   new Modules.ChildProcessesModule(this.m_Context.Environment));
            this.m_Modules.Add("crypto",            new Modules.CryptoModule(this.m_Context.Environment));
            this.m_Modules.Add("dgram",             new Modules.DgramModule(this.m_Context.Environment));
            this.m_Modules.Add("dns",               new Modules.DnsModule(this.m_Context.Environment));
            this.m_Modules.Add("events",            new Modules.EventsModule(this.m_Context.Environment));
            this.m_Modules.Add("fs",                new Modules.FsModule(this.m_Context.Environment));
            this.m_Modules.Add("http",              new Modules.HttpModule(this.m_Context.Environment));
            this.m_Modules.Add("https",             new Modules.HttpsModule(this.m_Context.Environment));
            this.m_Modules.Add("net",               new Modules.NetModule(this.m_Context.Environment));
            this.m_Modules.Add("os",                new Modules.OsModule(this.m_Context.Environment));
            this.m_Modules.Add("path",              new Modules.PathModule(this.m_Context.Environment));
            this.m_Modules.Add("tls",               new Modules.TlsModule(this.m_Context.Environment));
            this.m_Modules.Add("tty",               new Modules.TtyModule(this.m_Context.Environment));
            this.m_Modules.Add("url",               new Modules.UrlModule(this.m_Context.Environment));
            this.m_Modules.Add("util",              new Modules.UtilModule(this.m_Context.Environment));
            this.m_Modules.Add("vm",                new Modules.VmModule(this.m_Context.Environment));
        }

        /// <summary>
        /// Executes the given stream as a Javascript file under Node.js.
        /// </summary>
        /// <param name="stream">The data stream of the Javascript file.</param>
        /// <returns>The return code of the application.</returns>
        public int Execute(Stream stream)
        {
            // Read the entire contents of the stream into the string.
            List<byte> bytes = new List<byte>();
            int buffer = -1;
            while ((buffer = stream.ReadByte()) != -1)
                bytes.Add((byte)buffer);
            string data = Encoding.UTF8.GetString(bytes.ToArray());
            
            // Create the require function.
            ArrayObject pathsTable = new ArrayObject(this.m_Context.Environment, 0);
            FunctionObject fo = IronJS.Native.Utils.createHostFunction<System.Func<string, IronJS.CommonObject>>(this.m_Context.Environment, this.Require);
            fo.Put("paths", pathsTable);
            this.m_Context.SetGlobal<IronJS.FunctionObject>("require", fo);

            // Execute the data.
            this.m_Context.Execute(data);

            // Enter our main loop while we wait until everything has closed.
            while (!EventManager.IsFinished())
                Thread.Sleep(100);

            // Return (there is no data).
            return 0;
        }

        /// <summary>
        /// Function to handle require() function used in Node.js scripts.
        /// </summary>
        /// <param name="path">The requested module.</param>
        /// <returns>The returned object.</returns>
        private IronJS.CommonObject Require(string path)
        {
            // Detect built-in classes.
            if (this.m_Modules.Keys.Contains(path))
                return this.m_Modules[path];

            // Handle other libraries based on paths.
            return null;
        }

        /// <summary>
        /// Function for registering C# functions with the Javascript engine.
        /// </summary>
        /// <typeparam name="T">The type of function (i.e. a delegate)</typeparam>
        /// <param name="c">The context to register with.</param>
        /// <param name="name">The name to register the function to in the global namespace.</param>
        /// <param name="func">The function to register.</param>
        private void Register<T>(CSharp.Context c, string name, T func)
        {
            // This doesn't work, the delegate mangling causes the parameter and return information
            // to be lost (so IronJS crashes when attempting to inspect it).
            Delegate d = (Delegate)(object)(T)func;
            c.SetGlobal<IronJS.FunctionObject>(name, IronJS.Native.Utils.createHostFunction<Delegate>(c.Environment, d));
        }
    }
}
