using System;

namespace Hadoop
{
    public interface RecordReader
    {
        bool Next(out byte[] key, out byte[] value);

        float Progress
        {
            get;
        }
    }
}
