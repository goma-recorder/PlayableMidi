using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class EndPointEvent : MTrkEvent
    {
        public const byte EventNumber = 0x2f;

        public EndPointEvent(){}
        public EndPointEvent(uint ticks):base(ticks){}

        protected override Type ToString(List<string> list)
        {
            return typeof(EndPointEvent);
        }
    }
}
