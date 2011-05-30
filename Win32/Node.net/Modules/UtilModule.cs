using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS;
using IronJS.Native;

namespace Node.net.Modules
{
    internal class UtilModule : HostModule
    {
        /// <summary>
        /// The public constructor for the 'util' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public UtilModule(IronJS.Environment env)
            : base(env)
        {
            this.Put("debug", Utils.createHostFunction<System.Func<string, CommonObject>>(env, this.debug));
            this.Put("log", Utils.createHostFunction<System.Func<string, CommonObject>>(env, this.log));
            this.Put("inspect", Utils.createHostFunction<System.Func<string, bool, int, CommonObject>>(env, this.inspect));
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/util.html#util.debug</see>
        /// </summary>
        public CommonObject debug(string message)
        {
            Console.WriteLine(message);
            return null;
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/util.html#util.log</see>
        /// </summary>
        public CommonObject log(string message)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + message);
            return null;
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/util.html#util.inspect</see>
        /// </summary>
        public CommonObject inspect(string message)
        {
            return this.inspect(message, false, 2);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/util.html#util.inspect</see>
        /// </summary>
        public CommonObject inspect(string message, bool showHidden)
        {
            return this.inspect(message, showHidden, 2);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/util.html#util.inspect</see>
        /// </summary>
        public CommonObject inspect(string message, bool showHidden, int depth)
        {
            Console.WriteLine("FIXME: util.inspect is not implemented.");
            return null;
        }
    }
}
