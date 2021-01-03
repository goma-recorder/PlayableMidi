using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class TrackNameEvent : MTrkEvent
    {
        public const byte EventNumber = 0x03;
        public string name;

        internal TrackNameEvent(uint ticks, string name) : base(ticks)
        {
            this.name = name;
        }

        public TrackNameEvent(string name) : this(0, name)
        {
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(name);
            return typeof(TrackNameEvent);
        }
    }
}