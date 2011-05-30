using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS;

namespace Node.net.Modules.Buffers
{
    public class NodeBuffer : HostModule
    {
        private Encoding m_Encoding = Encoding.UTF8;
        private byte[] m_Data = null;

        public byte[] Data
        {
            get { return this.m_Data; }
        }

        private NodeBuffer(IronJS.Environment env) : base(env)
        {
        }

        /// <summary>
        /// The internal constructor for a NodeBuffer.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        internal NodeBuffer(IronJS.Environment env, uint size)
            : this(env)
        {
            this.m_Data = new byte[size];
        }

        /// <summary>
        /// The internal constructor for a NodeBuffer that copies information
        /// from an existing byte[] array.
        /// </summary>
        /// <param name="env">The JavaScript environment.</param>
        internal NodeBuffer(IronJS.Environment env, byte[] bytes, int offset, int length)
            : this(env)
        {
            this.m_Data = new byte[length];
            for (int i = 0; i < length; i += 1)
                this.m_Data[i] = bytes[offset + length];
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/buffers.html#new_Buffer</see>
        /// </summary>
        public static NodeBuffer Create(IronJS.Environment env, uint size)
        {
            return new NodeBuffer(env, size);
        }

        /// <summary>
        /// <see>http://nodejs.org/docs/v0.4.8/api/buffers.html#new_Buffer</see>
        /// </summary>
        public static NodeBuffer Create(IronJS.Environment env, ArrayObject array)
        {
            NodeBuffer n = new NodeBuffer(env, array.Length);
            for (int i = 0; i < array.Length; i += 1)
                n.Data[i] = (byte)(uint)array.Get(i).Number;
            return n;
        }

        /// <summary>
        /// Copies the NodeBuffer but adds an EOF (0x04) byte to the end of it.
        /// </summary>
        /// <returns>A copy of the NodeBuffer with an EOF at the end.</returns>
        public NodeBuffer AddEOF()
        {
            List<byte> bytes = this.Data.ToList();
            bytes.Add(0x04);
            return new NodeBuffer(this.Env, bytes.ToArray(), 0, bytes.Count);
        }
    }
}
