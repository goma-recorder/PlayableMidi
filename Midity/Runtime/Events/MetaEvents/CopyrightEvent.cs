namespace Midity
{
    [System.Serializable]
    public sealed class CopyrightEvent : MTrkEvent
    {
        public const byte EventNumber = 0x02;
        public string text;
        public CopyrightEvent(){}

        public CopyrightEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }
    }
}
