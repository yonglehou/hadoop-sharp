using System;

namespace Hadoop
{
    public interface RecordWriter
    {
        void Emit(byte[] key, byte[] value);
    }
}
