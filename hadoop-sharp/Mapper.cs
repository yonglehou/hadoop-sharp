using System;

namespace Hadoop
{
    public interface Mapper
    {
        void Map(MapContext context);
    }
}
