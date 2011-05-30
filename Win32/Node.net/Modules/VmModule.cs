using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class VmModule : HostModule 
    {
        /// <summary>
        /// The public constructor for the 'vm' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public VmModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
