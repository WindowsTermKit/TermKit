using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IronJS.Hosting;
using IronJS.Compiler;

namespace Node.net
{
    public class HostEngine
    {
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

            // Set up the context.
            CSharp.Context c = new CSharp.Context();
            c.CreatePrintFunction();
            
            // Create the require function.
            IronJS.ArrayObject pathsTable = new IronJS.ArrayObject(c.Environment, 0);
            IronJS.FunctionObject fo = IronJS.Native.Utils.createHostFunction<Func<string, IronJS.CommonObject>>(c.Environment, this.Require);
            fo.Put("paths", pathsTable);
            c.SetGlobal<IronJS.FunctionObject>("require", fo);

            // Execute the data.
            object o = c.Execute(data);
            return Convert.ToInt32(o);
        }

        /// <summary>
        /// Function to handle require() function used in Node.js scripts.
        /// </summary>
        /// <param name="path">The requested module.</param>
        /// <returns>The returned object.</returns>
        private IronJS.CommonObject Require(string path)
        {
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
