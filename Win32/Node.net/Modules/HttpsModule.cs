using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class HttpsModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'https' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public HttpsModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
