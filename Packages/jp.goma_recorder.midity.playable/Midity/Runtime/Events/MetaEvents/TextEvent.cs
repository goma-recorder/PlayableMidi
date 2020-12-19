using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class TextEvent : MTrkEvent
    {
        public const byte EventNumber = 0x01;
        public string text;

        public TextEvent(){}

        public TextEvent(uint ticks, string text) : base(ticks)
        {
            this.text = text;
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(text);
            return typeof(TextEvent);
        }
    }
}
