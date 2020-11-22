namespace Midity
{
    [System.Serializable]
    public sealed class QueueEvent : MTrkEvent
    {
        public const byte status = 0x07;
        public string text;
    }
}
