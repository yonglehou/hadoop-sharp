using System;

namespace Hadoop
{
    public interface MapContext : TaskContext
    {
        string InputSplit
        {
            get;
        }

        string InputKeyClass
        {
            get;
        }

        string InputValueClass
        {
            get;
        }
    }
}
