namespace Midity
{
    [System.Serializable]
    public sealed class ChannelPrefixEvent : MTrkEvent
    {
        public const byte EventNumber = 0x20;
        public byte data;
        
        public ChannelPrefixEvent(){}

        public ChannelPrefixEvent(uint ticks, byte data) : base(ticks)
        {
            this.data = data;
        }
    }
}
