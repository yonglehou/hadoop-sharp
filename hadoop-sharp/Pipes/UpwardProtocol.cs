using System;
using System.Collections.Generic;
using System.Text;

namespace Hadoop.Pipes
{
    public interface UpwardProtocol
    {
        void Output(byte[] key, byte[] value);

        void PartitionedOutput(int reduce, byte[] key, byte[] value);

        string Status
        {
            set;
        }

        float Progress
        {
            set;
        }

        void Done();

        void RegisterCounter(int id, string group, string name);

        void IncrementCounter(Counter counter, long amount);
    }
}