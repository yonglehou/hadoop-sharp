using System;

namespace Hadoop
{
    public interface TaskContext
    {
        JobConf JobConf
        {
            get;
        }

        byte[] InputKey
        {
            get;
        }

        byte[] InputValue
        {
            get;
        }

        void Emit(byte[] key, byte[] value);

        void Progress();

        string Status
        {
            set;
        }

        Counter GetCounter(string group, string name);

        void IncrementCounter(Counter counter, long amount);
    }
}
