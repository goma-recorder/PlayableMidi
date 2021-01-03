using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class MarkerEvent : MTrkEvent
    {
        public const byte EventNumber = 0x06;
        public string text;

        internal MarkerEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }

        public MarkerEvent(string text) : this(0, text)
        {
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(text);
            return typeof(MarkerEvent);
        }
    }
}