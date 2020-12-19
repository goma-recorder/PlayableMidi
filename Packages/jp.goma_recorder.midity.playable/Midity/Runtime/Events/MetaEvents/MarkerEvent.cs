using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class MarkerEvent : MTrkEvent
    {
        public const byte EventNumber = 0x06;
        public string text;
        
        public MarkerEvent(){}

        public MarkerEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(text);
            return typeof(MarkerEvent);
        }
    }
}
