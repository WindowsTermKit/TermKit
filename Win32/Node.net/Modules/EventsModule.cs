using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net.Modules
{
    internal class EventsModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'events' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public EventsModule(IronJS.Environment env)
            : base(env)
        {
        }
    }
}
