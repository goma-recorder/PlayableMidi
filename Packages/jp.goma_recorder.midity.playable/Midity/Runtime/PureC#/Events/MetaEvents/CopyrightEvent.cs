using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class CopyrightEvent : MTrkEvent
    {
        public const byte EventNumber = 0x02;
        public string text;

        internal CopyrightEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }

        public CopyrightEvent(string text) : this(0, text)
        {
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(text);
            return typeof(CopyrightEvent);
        }
    }
}