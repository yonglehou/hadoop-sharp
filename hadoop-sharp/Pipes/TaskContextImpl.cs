using System;
using System.Collections.Generic;

namespace Hadoop.Pipes
{
    public class TaskContextImpl : MapContext, ReduceContext, DownwardProtocol
    {
        private bool done;
        private JobConf jobConf;
        private byte[] key;
        private byte[] newKey;
        private byte[] valuee;
        private bool hasTask;
        private bool isNewKey;
        private bool isNewValue;
        private string inputKeyClass;
        private string inputValueClass;
        private string status;
        private float progressFloat;
        private DateTime lastProgress;
        private bool statusSet;
        private Protocol protocol;
        private UpwardProtocol uplink;
        private string inputSplit;
        private RecordReader reader;
        private Mapper mapper;
        private Reducer reducer;
        private RecordWriter writer;
        private Partitioner partitioner;
        private int numReducers;
        private Factory factory;
        private List<int> registeredCounterIds = new List<int>();

        public TaskContextImpl(Factory factory)
        {
            this.factory = factory;
        }

        public void SetProtocol(Protocol protocol, UpwardProtocol uplink)
        {
            this.protocol = protocol;
            this.uplink = uplink;
        }

        public bool Done
        {
            get
            {
                return done;
            }
        }

        public void WaitForTask()
        {
            while (!done && !hasTask)
            {
                protocol.NextEvent();
            }
        }

        public void Start(int protocol)
        {
            if (protocol != 0)
                throw new Exception("Protocol version " + protocol + " not supported");
        }

        public void SetInputTypes(string inputKeyClass, string inputValueClass)
        {
            this.inputKeyClass = inputKeyClass;
            this.inputValueClass = inputValueClass;
        }

        public void RunMap(string inputSplit, int numReducers, bool pipedInputs)
        {
            this.inputSplit = inputSplit;

            reader = factory.CreateRecordReader(this);

            if (reader != null)
            {
                valuee = new byte[0];
            }

            mapper = factory.CreateMapper(this);
            this.numReducers = numReducers;

            if (numReducers != 0)
            {
                reducer = factory.CreateCombiner(this);
                partitioner = factory.CreatePartitioner(this);
            }

            if (reducer != null)
            {
                long spillSize = 100;
				
                if (jobConf.ContainsKey("io.sort.mb"))
                {
                    spillSize = Convert.ToInt64(jobConf["io.sort.mb"]);
                }

                // TODO writer  = new CombineRunner (
            }

            hasTask = true;
        }

        public bool NextKey()
        {
            if (reader == null)
            {
                while (!isNewKey)
                {
                    NextValue();
                    if (done)
                    {
                        return false;
                    }
                }
                key = newKey;
            }
            else
            {
                if (!reader.Next(out key, out valuee))
                {
                    done = true;
                    return false;
                }

                progressFloat = reader.Progress;
            }

            isNewKey = false;

            if (mapper != null)
            {
                mapper.Map(this);
            }
            else
            {
                reducer.Reduce(this);
            }

            return true;
        }

        public void MapItem(byte[] key, byte[] value)
        {
            newKey = key;
            valuee = value;
            isNewKey = true;
        }

        public void Close()
        {
            done = true;
        }

        public void Abort()
        {
            throw new Exception("Aborted by user");
        }

        public JobConf JobConf
        {
            set { jobConf = value; }
            get { return jobConf; }
        }

        public string InputKeyClass
        {
            get { return inputKeyClass; }
        }

        public string InputSplit
        {
            get { return inputSplit; }
        }

        public string InputValueClass
        {
            get { return inputValueClass; }
        }

        public byte[] InputKey
        {
            get { return key; }
        }

        public byte[] InputValue
        {
            get { return valuee; }
        }

        public Counter GetCounter(string group, string name)
        {
            int id = registeredCounterIds.Count;
            registeredCounterIds.Add(id);
            uplink.RegisterCounter(id, group, name);
            return new Counter(id);
        }

        public void Emit(byte[] key, byte[] value)
        {
            Progress();

            if (writer != null)
            {
                writer.Emit(key, value);
            }
            else if (partitioner != null)
            {
                int part = partitioner.Partition(key, numReducers);
                uplink.PartitionedOutput(part, key, value);
            }
            else
            {
                uplink.Output(key, value);
            }
        }

        public void RunReduce(int reduce, bool pipedOutput)
        {
            reducer = factory.CreateReducer(this);
            writer = factory.CreateRecordWriter(this);

            hasTask = true;
        }

        public void ReduceKey(byte[] key)
        {
            isNewKey = true;
            newKey = key;
        }

        public void ReduceValue(byte[] value)
        {
            isNewValue = true;
            valuee = value;
        }

        public bool NextValue()
        {
            if (isNewKey || done)
            {
                return false;
            }

            isNewValue = false;
            Progress();
            protocol.NextEvent();
            return isNewValue;
        }

        public void IncrementCounter(Counter counter, long amount)
        {
            uplink.IncrementCounter(counter, amount);
        }

        public string Status
        {
            set
            {
                status = value;
                statusSet = true;
                Progress();
            }
        }

        public void Progress()
        {
            if (uplink != null)
            {
                DateTime now = DateTime.Now;

                if ((now - lastProgress).TotalMilliseconds > 1000)
                {
                    lastProgress = now;

                    if (statusSet)
                    {
                        uplink.Status = status;
                        statusSet = false;
                    }

                    uplink.Progress = progressFloat;
                }
            }
        }
    }
}