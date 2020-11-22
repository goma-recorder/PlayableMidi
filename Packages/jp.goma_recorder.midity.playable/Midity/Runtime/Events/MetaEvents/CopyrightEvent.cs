namespace Midity
{
    [System.Serializable]
    public sealed class CopyrightEvent : MTrkEvent
    {
        public const byte status = 0x02;
        public string text;
    }
}
