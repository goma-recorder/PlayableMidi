using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class SmpteOffsetEvent : MTrkEvent
    {
        public const byte EventNumber = 0x54;
        public byte ff;
        public byte fr;
        public byte hr;
        public byte mn;
        public byte se;

        internal SmpteOffsetEvent(uint ticks, byte hr, byte mn, byte se, byte fr, byte ff) : base(ticks)
        {
            this.hr = hr;
            this.mn = mn;
            this.se = se;
            this.fr = fr;
            this.ff = ff;
        }

        public SmpteOffsetEvent(byte hr, byte mn, byte se, byte fr, byte ff) : this(0, hr, mn, se, fr, ff)
        {
        }

        protected override Type ToString(List<string> list)
        {
            return typeof(SmpteOffsetEvent);
        }
    }
}