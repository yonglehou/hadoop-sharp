using System;

namespace Hadoop
{
    public class Counter
    {
        private int id;

        public Counter(int id)
        {
            this.id = id;
        }

        public Counter(Counter counter)
        {
            this.id = counter.id;
        }

        public int Id
        {
            get { return id; }
        }
    }
}
