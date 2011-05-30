using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Node.net.Modules.Streams;

namespace Node.net.Modules
{
    internal class ProcessModule : HostModule
    {
        private NodeWritableStream m_stdout = null;
        private NodeWritableStream m_stderr = null;
        private NodeReadableStream m_stdin = null;

        /// <summary>
        /// The events for this class.
        /// </summary>
        public event EventHandler OnExit;
        public event EventHandler<UnhandledExceptionEventArgs> OnUnhandledException;

        /// <summary>
        /// Standard output.
        /// </summary>
        public NodeWritableStream stdout
        {
            get { return this.m_stdout; }
        }

        /// <summary>
        /// Standard error output.
        /// </summary>
        public NodeWritableStream stderr
        {
            get { return this.m_stderr; }
        }

        /// <summary>
        /// Standard input.
        /// </summary>
        public NodeReadableStream stdin
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
            this.m_stdout = new NodeWritableStream(env, new StreamWriter(Console.OpenStandardOutput()));
            this.m_stderr = new NodeWritableStream(env, new StreamWriter(Console.OpenStandardError()));
            this.m_stdin = new NodeReadableStream(env, new StreamReader(Console.OpenStandardInput()));
        }

        /// <summary>
        /// Class for passing event arguments for OnUnhandledException.
        /// </summary>
        public class UnhandledExceptionEventArgs : AutoWrapEventArgs
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
