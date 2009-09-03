using System;
using System.Collections.Generic;
using System.Text;

namespace Hadoop.Pipes
{
    public interface DownwardProtocol
    {
        void Start(int protocol);

        JobConf JobConf
        {
            set;
        }

        void SetInputTypes(String keyType, String valueType);

        void RunMap(string split, int numReduces,
                    bool pipedInput);

        void MapItem(byte[] key, byte[] val);

        void RunReduce(int reduce, bool pipedOutput);

        void ReduceKey(byte[] key);

        void ReduceValue(byte[] val);

        void Abort();

        void Close();
    }
}
