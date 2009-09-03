using System;

namespace Hadoop
{
    public interface Reducer
    {
        void Reduce(ReduceContext context);
    }
}
