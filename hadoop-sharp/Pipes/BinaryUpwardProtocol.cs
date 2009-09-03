using System;
using System.IO;
using System.Collections.Generic;

using Hadoop.IO;

namespace Hadoop.Pipes
{
    public class BinaryUpwardProtocol : UpwardProtocol
    {
        protected Stream stream;

        public BinaryUpwardProtocol(Stream stream)
        {
            this.stream = stream;
        }

        public void Output(byte[] key, byte[] val)
        {
            WriteMessage(BinaryMessageTypes.OUTPUT);
            WritableUtils.WriteBytes(stream, key);
            WritableUtils.WriteBytes(stream, val);
        }

        private void WriteMessage(BinaryMessageTypes t)
        {
            stream.WriteByte((byte)t);
        }

        public void PartitionedOutput(int reduce, byte[] key, byte[] val)
        {
            WriteMessage(BinaryMessageTypes.PARTITIONED_OUTPUT);
            WritableUtils.WriteVInt(stream, reduce);
            WritableUtils.WriteBytes(stream, key);
            WritableUtils.WriteBytes(stream, val);
        }

        public string Status
        {
            set
            {
                WriteMessage(BinaryMessageTypes.STATUS);
                WritableUtils.WriteString(stream, value);
            }
        }

        public float Progress
        {
            set
            {
                WriteMessage(BinaryMessageTypes.PROGRESS);
                byte[] buffer = BitConverter.GetBytes(value);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        public void Done()
        {
            WriteMessage(BinaryMessageTypes.DONE);
        }

        public void RegisterCounter(int id, String group, String name)
        {
            WriteMessage(BinaryMessageTypes.REGISTER_COUNTER);
            WritableUtils.WriteVInt(stream, id);
            WritableUtils.WriteString(stream, group);
            WritableUtils.WriteString(stream, name);
        }

        public void IncrementCounter(Counter counter, long amount)
        {
            WriteMessage(BinaryMessageTypes.INCREMENT_COUNTER);
            WritableUtils.WriteVInt(stream, counter.Id);
            WritableUtils.WriteVLong(stream, amount);
        }
    }
}
