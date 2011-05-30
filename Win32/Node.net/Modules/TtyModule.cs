using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class TtyModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'tty' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public TtyModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
