using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Node.net.Modules.Buffers;

namespace Node.net.Modules.Streams
{
    public class NodeWritableStream : NodeStream
    {
        private StreamWriter m_Writer = null;
        private NodeReadableStream m_PipeSource = null;

        public event EventHandler OnDrain;
        public event EventHandler OnError;
        public event EventHandler OnClose;
        public event EventHandler<PipeEventArgs> OnPipe;

        /// <summary>
        /// Constructs a new NodeWritableStream using the specified .NET stream writer.
        /// </summary>
        /// <param name="reader">The .NET StreamWriter to wrap.</param>
        internal NodeWritableStream(IronJS.Environment env, StreamWriter writer)
            : base(env)
        {
            this.m_Writer = writer;
            EventManager.Add(this);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.writeable</see>
        /// </summary>
        public override bool writable
        {
            get { return this.m_Writer.BaseStream.CanWrite; }
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.readable</see>
        /// </summary>
        public override bool readable
        {
            get { return false; }
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.setEncoding</see>
        /// </summary>
        public override void setEncoding(string encoding)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.pause</see>
        /// </summary>
        public override void pause()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.resume</see>
        /// </summary>
        public override void resume()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override bool write(string data)
        {
            return this.write(data, "utf8", null);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override bool write(string data, string encoding)
        {
            return this.write(data, encoding, null);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override bool write(string data, string encoding, object fd)
        {
            Encoding e = null;
            switch (encoding)
            {
                case "ascii":
                    e = Encoding.ASCII;
                    break;
                case "utf8":
                    e = Encoding.UTF8;
                    break;
                case "base64":
                    e = new Base64Encoding();
                    break;
            }
            if (e == null)
                throw new NotSupportedException(); // TODO: Should this raise OnError?

            // Send it across via a NodeBuffer.
            byte[] encoded = e.GetBytes(data);
            return this.write(new NodeBuffer(this.Env, encoded, 0, encoded.Length));
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override bool write(NodeBuffer buffer)
        {
            this.m_Writer.BaseStream.Write(buffer.Data, 0, buffer.Data.Length);

            // TODO: How do we handle kernel buffering?
            return true;
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.end</see>
        /// </summary>
        public override void end()
        {
            this.write(new NodeBuffer(this.Env, 0).AddEOF()); // 0x04
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.end</see>
        /// </summary>
        public override void end(string data, string encoding)
        {
            Encoding e = null;
            switch (encoding)
            {
                case "ascii":
                    e = Encoding.ASCII;
                    break;
                case "utf8":
                    e = Encoding.UTF8;
                    break;
                case "base64":
                    e = new Base64Encoding();
                    break;
            }
            if (e == null)
                throw new NotSupportedException(); // TODO: Should this raise OnError?

            // Send it across via a NodeBuffer.
            byte[] encoded = e.GetBytes(data);
            this.end(new NodeBuffer(this.Env, encoded, 0, encoded.Length));
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.end</see>
        /// </summary>
        public override void end(NodeBuffer buffer)
        {
            this.write(buffer.AddEOF());
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.destroy</see>
        /// </summary>
        public override void destroy()
        {
            this.m_Writer.Close();
            if (this.OnClose != null)
                this.OnClose(this, new EventArgs());
            EventManager.Remove(this);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.destroySoon</see>
        /// </summary>
        public override void destroySoon()
        {
            // TODO: What does this function do?
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.pipe</see>
        /// </summary>
        public override void pipe(NodeWritableStream destination)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.pipe</see>
        /// </summary>
        public override void pipe(NodeWritableStream destination, IronJS.ArrayObject options)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Triggers the OnPipe event (called by the original NodeReadableStream which
        /// sends itself).
        /// </summary>
        /// <param name="src">The NodeReadableStream that is the source of the binding.</param>
        internal void DoPipe(NodeReadableStream src)
        {
            this.m_PipeSource = src;
            if (this.OnPipe != null)
                this.OnPipe(this, new PipeEventArgs(src));
        }
    }
}
