using System;
using System.Collections.Generic;
using System.Linq;

namespace Midity
{
    public sealed class UnknownMetaEvent : MTrkEvent
    {
        public readonly byte[] data;
        public readonly byte eventNumber;

        internal UnknownMetaEvent(uint ticks, byte eventNumber, byte[] data) : base(ticks)
        {
            this.eventNumber = eventNumber;
            this.data = data;
        }

        protected override Type ToString(List<string> list)
        {
            list.AddRange(data.Select(d => d.ToString()));
            return typeof(UnknownMetaEvent);
        }
    }
}