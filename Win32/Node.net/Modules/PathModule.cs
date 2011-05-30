using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class PathModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'path' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public PathModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
