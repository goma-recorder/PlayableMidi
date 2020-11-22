namespace Midity
{
    [System.Serializable]
    public sealed class BeatEvent : MTrkEvent
    {
        public const byte status = 0x58;
        public byte[] data = new byte[4];
    }
}
