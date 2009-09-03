using System;

namespace Hadoop
{
    public interface Partitioner
    {
        int Partition(byte[] key, int numOfReduces);
    }
}
