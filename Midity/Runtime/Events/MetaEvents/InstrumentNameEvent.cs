namespace Midity
{
    [System.Serializable]
    public sealed class InstrumentNameEvent : MTrkEvent
    {
        public const byte EventNumber = 0x04;
        public string name;
        
        public InstrumentNameEvent(){}

        public InstrumentNameEvent(uint ticks, string name) : base(ticks)
        {
            this.name = name;
        }
    }
}
