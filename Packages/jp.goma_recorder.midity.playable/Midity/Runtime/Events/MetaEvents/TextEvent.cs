namespace Midity
{
    [System.Serializable]
    public sealed class TextEvent : MTrkEvent
    {
        public const byte EventNumber = 0x01;
        public string text;

        public TextEvent(){}

        public TextEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }
    }
}
