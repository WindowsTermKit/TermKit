using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class CryptoModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'crypto' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public CryptoModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
