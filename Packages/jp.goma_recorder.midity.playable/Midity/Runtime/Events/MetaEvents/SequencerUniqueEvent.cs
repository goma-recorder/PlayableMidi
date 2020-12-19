using System;
using System.Collections.Generic;
using System.Linq;

namespace Midity
{
    [System.Serializable]
    public sealed class SequencerUniqueEvent : MTrkEvent
    {
        public const byte EventNumber = 0x7f;
        public byte[] data;
        
        public SequencerUniqueEvent(){}

        public SequencerUniqueEvent(uint ticks, byte[] data) : base(ticks)
        {
            this.data = data;
        }

        protected override Type ToString(List<string> list)
        {
            list.AddRange(data.Select(n => n.ToString()));
            return typeof(SequencerUniqueEvent);
        }
    }
}
