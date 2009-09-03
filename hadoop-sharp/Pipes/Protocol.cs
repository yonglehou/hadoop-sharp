using System;

namespace Hadoop.Pipes
{
    public interface Protocol
    {
        void NextEvent();
        
        UpwardProtocol Uplink
        {
            get;
        }
    }
}
