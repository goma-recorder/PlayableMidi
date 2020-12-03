namespace Midity
{
    [System.Serializable]
    public sealed class TrackNameEvent : MTrkEvent
    {
        public const byte EventNumber = 0x03;
        public string name;
        
        public TrackNameEvent(){}

        public TrackNameEvent(uint ticks, string name) : base(ticks)
        {
            this.name = name;
        }
    }
}
