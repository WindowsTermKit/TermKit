using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class OsModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'os' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public OsModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
