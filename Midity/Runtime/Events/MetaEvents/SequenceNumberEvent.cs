using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class SequenceNumberEvent : MTrkEvent
    {
        public const byte EventNumber = 0x00;
        public ushort number;
        public SequenceNumberEvent(){}

        public SequenceNumberEvent(uint ticks,ushort number):base(ticks)
        {
            this.number = number;
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(number.ToString());
            return typeof(SequenceNumberEvent);
        }
    }
}
