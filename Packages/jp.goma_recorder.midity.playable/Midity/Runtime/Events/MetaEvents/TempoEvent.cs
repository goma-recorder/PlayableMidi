namespace Midity
{
    [System.Serializable]
    public sealed class TempoEvent : MTrkEvent
    {
        public const byte status = 0x51;
        public uint tickTempo;
        public float tempo => 60000000f / tickTempo;
    }
}
