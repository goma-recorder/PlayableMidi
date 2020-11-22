namespace Midity
{
    [System.Serializable]
    public sealed class ChannelPrefixEvent : MTrkEvent
    {
        public const byte status = 0x20;
        public byte data;
    }
}
