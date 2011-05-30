using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
