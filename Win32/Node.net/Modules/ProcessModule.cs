using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Node.net.Modules
{
    internal class ProcessModule : HostModule
    {
        private StreamWriter m_stdout = null;
        private StreamWriter m_stderr = null;
        private StreamReader m_stdin = null;

        /// <summary>
        /// The events for this class.
        /// </summary>
        public event EventHandler OnExit;
        public event EventHandler<UnhandledExceptionEventArgs> OnUnhandledException;

        /// <summary>
        /// Standard output.
        /// </summary>
        public StreamWriter stdout
        {
            get { return this.m_stdout; }
        }

        /// <summary>
        /// Standard error output.
        /// </summary>
        public StreamWriter stderr
        {
            get { return this.m_stderr; }
        }

        /// <summary>
        /// Standard input.
        /// </summary>
        public StreamReader stdin
        {
            get { return this.m_stdin; }
        }

        /// <summary>
        /// The public constructor for the 'process' module.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        public ProcessModule(IronJS.Environment env)
            : base(env)
        {
            // Open the console streams.
            this.m_stdout = Console.OpenStandardOutput();
            this.m_stderr = Console.OpenStandardError();
            this.m_stdin = Console.OpenStandardInput();
        }

        /// <summary>
        /// Class for passing event arguments for OnUnhandledException.
        /// </summary>
        private class UnhandledExceptionEventArgs : AutoWrapEventArgs
        {
            public Exception Exception;

            public UnhandledExceptionEventArgs(Exception exp)
            {
                this.Exception = exp;
            }

            public override IronJS.BoxedValue[] GetParameters()
            {
                return new IronJS.BoxedValue[]
                    {
                        IronJS.BoxedValue.Box(this.Exception.ToString())
                    };
            }
        }
    }
}
