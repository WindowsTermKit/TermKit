using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class NetModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'net' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public NetModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
