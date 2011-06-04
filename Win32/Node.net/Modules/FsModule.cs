using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS;
using System.IO;
using Node.net.Modules.Streams;

namespace Node.net.Modules
{
    internal class FsModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'fs' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public FsModule(IronJS.Environment env)
            : base(env)
        {
        }

        public CommonObject open(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            return new NodeReadableStream(this.Env, reader);
        }
    }
}
