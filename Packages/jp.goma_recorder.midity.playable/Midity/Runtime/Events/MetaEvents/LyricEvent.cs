namespace Midity
{
    [System.Serializable]
    public sealed class LyricEvent : MTrkEvent
    {
        public const byte EventNumber = 0x05;
        public string lyric;
        
        public LyricEvent(){}

        public LyricEvent(uint ticks, string lyric) : base(ticks)
        {
            this.lyric = lyric;
        }
    }
}
