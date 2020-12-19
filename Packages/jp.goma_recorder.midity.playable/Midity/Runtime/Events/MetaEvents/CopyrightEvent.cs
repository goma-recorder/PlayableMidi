using System;
using System.Collections.Generic;
using System.Linq;

namespace Midity
{
    [System.Serializable]
    public sealed class CopyrightEvent : MTrkEvent
    {
        public const byte EventNumber = 0x02;
        public string text;
        public CopyrightEvent(){}

        public CopyrightEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(text);
            return typeof(CopyrightEvent);
        }
    }
}
