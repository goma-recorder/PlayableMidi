using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class TrackNameEvent : MTrkEvent
    {
        public const byte EventNumber = 0x03;
        public string name;
        
        public TrackNameEvent(){}

        public TrackNameEvent(uint ticks, string name) : base(ticks)
        {
            this.name = name;
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(name);
            return typeof(TrackNameEvent);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
