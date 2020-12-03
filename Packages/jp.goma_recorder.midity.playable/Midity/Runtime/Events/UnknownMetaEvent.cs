namespace Midity
{
    [System.Serializable]
    public sealed class UnknownMetaEvent : MTrkEvent
    {
        public byte eventNumber;
        public byte[] data;
        
        public UnknownMetaEvent(){}

        public UnknownMetaEvent(uint ticks, byte eventNumber, byte[] data) : base(ticks)
        {
            this.eventNumber = eventNumber;
            this.data = data;
        }
    }
}
