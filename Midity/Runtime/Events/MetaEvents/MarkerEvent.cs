namespace Midity
{
    [System.Serializable]
    public sealed class MarkerEvent : MTrkEvent
    {
        public const byte status = 0x06;
        public string text;
    }
}
