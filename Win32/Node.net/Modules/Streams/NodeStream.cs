using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS;
using Node.net.Modules.Buffers;

namespace Node.net.Modules.Streams
{
    public abstract class NodeStream : AutoWrapObject
    {
        protected NodeStream(IronJS.Environment env) : base(env) { }

        public abstract bool writable { get; }
        public abstract bool readable { get; }
        public abstract void setEncoding(string encoding);
        public abstract void pause();
        public abstract void resume();
        public abstract bool write(string data);
        public abstract bool write(string data, string encoding);
        public abstract bool write(string data, string encoding, object fd);
        public abstract bool write(NodeBuffer buffer);
        public abstract void end();
        public abstract void end(string data, string encoding);
        public abstract void end(NodeBuffer buffer);
        public abstract void destroy();
        public abstract void destroySoon();
        public abstract void pipe(NodeWritableStream destination);
        public abstract void pipe(NodeWritableStream destination, IronJS.ArrayObject options);

        /// <summary>
        /// Class for passing event arguments for OnData.
        /// </summary>
        public class DataEventArgs : AutoWrapEventArgs
        {
            private byte[] m_Data = null;
            private Encoding m_Encoding = null;

            public byte[] Data
            {
                get { return this.m_Data; }
            }

            public DataEventArgs(byte[] bytes, int offset, int length)
                : this(bytes, offset, length, null)
            {
            }

            public DataEventArgs(byte[] bytes, int offset, int length, Encoding encoding)
            {
                this.m_Data = new byte[length];
                for (int i = 0; i < length; i += 1)
                    this.m_Data[i] = bytes[offset + i];
                this.m_Encoding = encoding;
            }

            public override BoxedValue[] GetParameters()
            {
                if (this.m_Encoding != null)
                {
                    // Return a string.
                    return new BoxedValue[]
                        {
                            BoxedValue.Box(this.m_Encoding.GetString(this.m_Data))
                        };
                }
                else
                {
                    // TODO: Return a raw buffer.
                    return new BoxedValue[]
                        {
                        };
                }
            }
        }

        /// <summary>
        /// Class for passing event arguments for OnPipe.
        /// </summary>
        public class PipeEventArgs : AutoWrapEventArgs
        {
            private NodeReadableStream m_Src = null;

            public PipeEventArgs(NodeReadableStream src)
            {
                this.m_Src = src;
            }

            public override BoxedValue[] GetParameters()
            {
                return new BoxedValue[]
                    {
                        BoxedValue.Box(this.m_Src)
                    };
            }
        }

    }
}
