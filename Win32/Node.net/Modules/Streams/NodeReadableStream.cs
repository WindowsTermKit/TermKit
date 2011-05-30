using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Node.net.Modules.Streams
{
    public class NodeReadableStream : NodeStream
    {
        private StreamReader m_Reader = null;
        private Encoding m_Encoding = null;
        private byte[] m_Buffer = new byte[256];
        private List<DataEventArgs> m_Queue = new List<DataEventArgs>();
        private bool m_Paused = false;
        private object m_ResumeLock = new object();
        private NodeWritableStream m_PipeDestination = null;

        public event EventHandler<DataEventArgs> OnData;
        //public event EventHandler OnDrain;
        //public event EventHandler OnPipe;
        public event EventHandler OnEnd;
        public event EventHandler OnError;
        public event EventHandler OnClose;
        public event EventHandler OnFd;

        /// <summary>
        /// Constructs a new NodeReadableStream using the specified .NET stream reader.
        /// </summary>
        /// <param name="reader">The .NET StreamReader to wrap.</param>
        internal NodeReadableStream(StreamReader reader)
        {
            this.m_Reader = reader;

            // Define the handler to respond to asynchronous operations.
            AsyncCallback handler = (result) =>
                {
                    if (result.IsCompleted)
                    {
                        int bytes = this.m_Reader.BaseStream.EndRead(result);
                        if (!this.m_Paused)
                        {
                            // Send it now.
                            if (this.m_PipeDestination != null)
                                this.m_PipeDestination.write(new NodeBuffer(this.m_Buffer, 0, bytes)); // TODO: Support writing as string as well?
                            if (this.OnData != null)
                                this.OnData(this, new DataEventArgs(this.m_Buffer, 0, bytes, this.m_Encoding));
                        }
                        else
                        {
                            // Queue it up.
                            this.m_Queue.Add(new DataEventArgs(this.m_Buffer, 0, bytes, this.m_Encoding));
                        }
                        if (this.m_Reader.BaseStream.CanRead)
                            this.m_Reader.BaseStream.BeginRead(this.m_Buffer, 0, 256, handler, null);
                    }
                };

            // Starts an asynchronous reading operation.
            if (this.m_Reader.BaseStream.CanRead)
                this.m_Reader.BaseStream.BeginRead(this.m_Buffer, 0, 256, handler, null);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.writeable</see>
        /// </summary>
        public override bool writable
        {
            get { return false; }
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.readable</see>
        /// </summary>
        public override bool readable
        {
            get { return this.m_Reader.BaseStream.CanRead && !this.m_Reader.EndOfStream; }
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.setEncoding</see>
        /// </summary>
        public override void setEncoding(string encoding)
        {
            switch (encoding)
            {
                case "ascii":
                    this.m_Encoding = Encoding.ASCII;
                    return;
                case "utf8":
                    this.m_Encoding = Encoding.UTF8;
                    return;
                case "base64":
                    this.m_Encoding = new Base64Encoding();
                    return;
            }
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.pause</see>
        /// </summary>
        public override void pause()
        {
            this.m_Paused = true;
            if (this.m_PipeDestination != null)
                this.m_PipeDestination.pause();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.resume</see>
        /// </summary>
        public override void resume()
        {
            lock (this.m_ResumeLock)
            {
                List<DataEventArgs> copy = this.m_Queue.Where(r => true);
                this.m_Queue.Clear();
                foreach (DataEventArgs e in copy)
                {
                    if (this.m_PipeDestination != null)
                        this.m_PipeDestination.write(new NodeBuffer(e.Data, 0, e.Data.Length)); // TODO: Support writing as string as well?
                    if (this.OnData != null)
                        this.OnData(this, e);
                }
            }
            if (this.m_PipeDestination != null)
                this.m_PipeDestination.resume();
            this.m_Paused = false;
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override void write(string data)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override void write(string data, string encoding)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override void write(string data, string encoding, object fd)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override void write(NodeBuffer buffer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.end</see>
        /// </summary>
        public override void end()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.end</see>
        /// </summary>
        public override void end(string data, string encoding)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.end</see>
        /// </summary>
        public override void end(NodeBuffer buffer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.destroy</see>
        /// </summary>
        public override void destroy()
        {
            this.m_Reader.Close();
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
        public override void pipe(NodeStream destination)
        {
            pipe(destination, null);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.pipe</see>
        /// </summary>
        public override void pipe(NodeStream destination, IronJS.Array options)
        {
            this.m_PipeDestination = destination;
        }
    }
}
