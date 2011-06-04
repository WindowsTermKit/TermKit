using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Node.net.Modules.Buffers;

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
        internal NodeReadableStream(IronJS.Environment env, StreamReader reader)
            : base(env)
        {
            this.m_Reader = reader;
            EventManager.Add(this);

            // Define the handler to respond to asynchronous operations.
            AsyncCallback handler = null;
            handler = (result) =>
                {
                    if (result.IsCompleted)
                    {
                        int bytes = this.m_Reader.BaseStream.EndRead(result);

                        if (bytes == 0)
                        {
                            // There is no data at the moment.
                        }
                        else if (!this.m_Paused && (this.OnData != null || this.m_PipeDestination != null))
                        {
                            // Send it now.
                            if (this.m_PipeDestination != null)
                                this.m_PipeDestination.write(new NodeBuffer(this.Env, this.m_Buffer, 0, bytes)); // TODO: Support writing as string as well?
                            if (this.OnData != null)
                                this.OnData(this, new DataEventArgs(this.m_Buffer, 0, bytes, this.m_Encoding));
                        }
                        else
                        {
                            // Queue it up.
                            this.m_Queue.Add(new DataEventArgs(this.m_Buffer, 0, bytes, this.m_Encoding));
                        }

                        if (this.m_Reader.BaseStream.Length != this.m_Reader.BaseStream.Position || this.m_Queue.Count > 0)
                            this.m_Reader.BaseStream.BeginRead(this.m_Buffer, 0, 256, handler, null);
                        else
                        {
                            if (this.OnEnd != null)
                                this.OnEnd(this, new EventArgs());
                            EventManager.Remove(this);
                        }
                    }
                };

            // Starts an asynchronous reading operation.
            if (this.m_Reader.BaseStream.Length != this.m_Reader.BaseStream.Position)
                this.m_Reader.BaseStream.BeginRead(this.m_Buffer, 0, 256, handler, null);
            else
            {
                if (this.OnEnd != null)
                    this.OnEnd(this, new EventArgs());
                EventManager.Remove(this);
            }
        }

        /// <summary>
        /// Recieve notification that an event has been attached.
        /// </summary>
        private void OnAttach(string name)
        {
            switch (name)
            {
                case "data":
                    // Send any data messages that may be in the queue.
                    lock (this.m_ResumeLock)
                    {
                        List<DataEventArgs> copy = this.m_Queue.Where(r => true).ToList();
                        this.m_Queue.Clear();
                        foreach (DataEventArgs e in copy)
                        {
                            if (this.m_PipeDestination != null)
                                this.m_PipeDestination.write(new NodeBuffer(this.Env, e.Data, 0, e.Data.Length)); // TODO: Support writing as string as well?
                            if (this.OnData != null)
                                this.OnData(this, e);
                        }
                    }
                    return;
            }
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
                List<DataEventArgs> copy = this.m_Queue.Where(r => true).ToList();
                this.m_Queue.Clear();
                foreach (DataEventArgs e in copy)
                {
                    if (this.m_PipeDestination != null)
                        this.m_PipeDestination.write(new NodeBuffer(this.Env, e.Data, 0, e.Data.Length)); // TODO: Support writing as string as well?
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
        public override bool write(string data)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override bool write(string data, string encoding)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override bool write(string data, string encoding, object fd)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.write</see>
        /// </summary>
        public override bool write(NodeBuffer buffer)
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
            if (this.OnClose != null)
                this.OnClose(this, new EventArgs());
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
            pipe(destination, null);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/streams.html#stream.pipe</see>
        /// </summary>
        public override void pipe(NodeWritableStream destination, IronJS.ArrayObject options)
        {
            this.m_PipeDestination = destination;
            this.m_PipeDestination.DoPipe(this);
        }
    }
}
