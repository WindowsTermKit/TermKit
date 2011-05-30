using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class TlsModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'tls' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public TlsModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
