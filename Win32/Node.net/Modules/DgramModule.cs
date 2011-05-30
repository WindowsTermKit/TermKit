using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class DgramModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'dgram' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public DgramModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
