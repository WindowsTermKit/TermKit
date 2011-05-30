using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class HttpModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'http' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public HttpModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
