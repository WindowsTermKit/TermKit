using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class AssertModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'assert' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public AssertModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
