using System;

namespace Midity
{
    [System.Serializable]
    public abstract class MTrkEvent
    {
        public uint ticks;
        
        public MTrkEvent(){}

        public MTrkEvent(uint ticks)
        {
            this.ticks = ticks;
        }
    }
}
