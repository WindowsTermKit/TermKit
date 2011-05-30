using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class UrlModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'url' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public UrlModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
