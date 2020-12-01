namespace Midity
{
    [System.Serializable]
    public sealed class QueueEvent : MTrkEvent
    {
        public const byte EventNumber = 0x07;
        public string text;
        
        public QueueEvent(){}

        public QueueEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }
    }
}
