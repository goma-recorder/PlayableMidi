using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class LyricEvent : MTrkEvent
    {
        public const byte EventNumber = 0x05;
        public string lyric;

        internal LyricEvent(uint ticks, string lyric) : base(ticks)
        {
            this.lyric = lyric;
        }

        public LyricEvent(string lyric) : this(0, lyric)
        {
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(lyric);
            return typeof(LyricEvent);
        }
    }
}