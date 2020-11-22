namespace Midity
{
    [System.Serializable]
    public sealed class InstrumentNameEvent : MTrkEvent
    {
        public const byte status = 0x04;
        public string name;
    }
}
