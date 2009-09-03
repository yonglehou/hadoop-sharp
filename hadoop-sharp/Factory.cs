using System;

namespace Hadoop
{
    public abstract class Factory
    {
        public abstract Mapper CreateMapper(MapContext context);
        public abstract Reducer CreateReducer(ReduceContext context);

        public virtual Reducer CreateCombiner(MapContext context)
        {
            return null;
        }

        public virtual Partitioner CreatePartitioner(MapContext context)
        {
            return null;
        }

        public virtual RecordReader CreateRecordReader(MapContext context)
        {
            return null;
        }

        public virtual RecordWriter CreateRecordWriter(ReduceContext context)
        {
            return null;
        }
    }
}
