using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class SmpteOffsetEvent : MTrkEvent
    {
        public const byte EventNumber = 0x54;
        public byte hr;
        public byte mn;
        public byte se;
        public byte fr;
        public byte ff;
        
        public SmpteOffsetEvent(){}

        public SmpteOffsetEvent(uint ticks,byte hr, byte mn, byte se, byte fr, byte ff) : base(ticks)
        {
            this.hr = hr;
            this.mn = mn;
            this.se = se;
            this.fr = fr;
            this.ff = ff;
        }

        protected override Type ToString(List<string> list)
        {
            return typeof(SmpteOffsetEvent);
        }
    }
}
