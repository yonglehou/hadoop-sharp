using System;
using System.IO;

using Hadoop.IO;

namespace Hadoop.Pipes
{
    public class BinaryProtocol : Protocol
    {
        private Stream down;
        private UpwardProtocol uplink;
        private DownwardProtocol handler;

        public BinaryProtocol(Stream down, DownwardProtocol handler, Stream up)
        {
            this.down = down;
            this.uplink = new BinaryUpwardProtocol(up);
            this.handler = handler;
        }

        public UpwardProtocol Uplink { get { return uplink; } }

        public void NextEvent()
        {
            int cmd = WritableUtils.ReadVInt(down);

            if (cmd < 0)
            {
                down.Close();
                return;
            }

            BinaryMessageTypes message = (BinaryMessageTypes)cmd;
            byte[] val;

            switch (message)
            {
                case BinaryMessageTypes.START_MESSAGE:
                    handler.Start(WritableUtils.ReadVInt(down));
                    break;
                case BinaryMessageTypes.SET_JOB_CONF:
                    int entries = WritableUtils.ReadVInt(down);
                    JobConf conf = new JobConf();
                    for (int i = 0; i < entries; ++i)
                    {
                        // TODO Fix
                        WritableUtils.ReadString(down);
                    }
                    //handler.SetJobConf(conf);
                    break;
                case BinaryMessageTypes.SET_INPUT_TYPES:
                    string keyType = WritableUtils.ReadString(down);
                    string valueType = WritableUtils.ReadString(down);
                    handler.SetInputTypes(keyType, valueType);
                    break;
                case BinaryMessageTypes.RUN_MAP:
                    string split = WritableUtils.ReadString(down);
                    int numReduces = WritableUtils.ReadVInt(down);
                    bool piped = WritableUtils.ReadVInt(down) == 1;
                    handler.RunMap(split, numReduces, piped);
                    break;
                case BinaryMessageTypes.MAP_ITEM:
                    byte[] key = WritableUtils.ReadBytes(down);
                    val = WritableUtils.ReadBytes(down);
                    handler.MapItem(key, val);
                    break;
                case BinaryMessageTypes.RUN_REDUCE:
                    handler.RunReduce(WritableUtils.ReadVInt(down), WritableUtils.ReadVInt(down) == 1);
                    break;
                case BinaryMessageTypes.REDUCE_KEY:
                    key = WritableUtils.ReadBytes(down);
                    handler.ReduceKey(key);
                    break;
                case BinaryMessageTypes.REDUCE_VALUE:
                    val = WritableUtils.ReadBytes(down);
                    handler.ReduceValue(val);
                    break;
                case BinaryMessageTypes.CLOSE:
                    handler.Close();
                    break;
                case BinaryMessageTypes.ABORT:
                    handler.Abort();
                    break;
                default:
                    throw new ApplicationException("Unknown binary command: " + cmd);
            }
        }
    }
}
