namespace Midity
{
    [System.Serializable]
    public sealed class TextEvent : MTrkEvent
    {
        public const byte status = 0x01;
        public string text;
    }
}
