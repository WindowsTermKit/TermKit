using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
	internal class DnsModule : HostModule
	{
        /// <summary>
        /// The public constructor for the 'dns' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public DnsModule(IronJS.Environment env)
            : base(env)
        {
        }
	}
}
