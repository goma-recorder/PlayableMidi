using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class InstrumentNameEvent : MTrkEvent
    {
        public const byte EventNumber = 0x04;
        public string name;

        internal InstrumentNameEvent(uint ticks, string name) : base(ticks)
        {
            this.name = name;
        }

        public InstrumentNameEvent(string name) : this(0, name)
        {
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(name);
            return typeof(InstrumentNameEvent);
        }
    }
}