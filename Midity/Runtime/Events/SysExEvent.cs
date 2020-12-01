namespace Midity
{
    [System.Serializable]
    public class SysExEvent : MTrkEvent
    {
        public byte[] data;
        
        public SysExEvent(){}

        public SysExEvent(uint ticks,byte[] data) : base(ticks)
        {
            this.data = data;
        }
    }
}

