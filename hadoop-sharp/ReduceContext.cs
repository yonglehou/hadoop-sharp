using System;

namespace Hadoop
{
    public interface ReduceContext : TaskContext
    {
        bool NextValue();
    }
}
