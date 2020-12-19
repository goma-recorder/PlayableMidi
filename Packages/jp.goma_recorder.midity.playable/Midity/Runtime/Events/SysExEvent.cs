using System;
using System.Collections.Generic;
using System.Linq;

namespace Midity
{
    [System.Serializable]
    public class SysExEvent : MTrkEvent
    {
        public byte[] data;
        
        public SysExEvent(){}

        public SysExEvent(uint ticks,byte[] data) : base(ticks)
        {
            this.data = data;
        }

        protected override Type ToString(List<string> list)
        {
            list.AddRange(data.Select(d => d.ToString()));
            return typeof(SysExEvent);
        }
    }
}

