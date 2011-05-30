using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class ChildProcessesModule :HostModule
    {
        /// <summary>
        /// The public constructor for the 'child_processes' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public ChildProcessesModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
