using System;
using System.Collections.Generic;
using System.Text;

namespace Midity
{
    [System.Serializable]
    public abstract class MTrkEvent
    {
        public uint ticks;

        public MTrkEvent()
        {
        }

        public MTrkEvent(uint ticks)
        {
            this.ticks = ticks;
        }

        protected abstract Type ToString(List<string> list);

        public override string ToString()
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            var typeName = ToString(list).Name;
            sb.Append($"{typeName}: {ticks}, ");
            sb.Append(string.Join(", ", list));
            return sb.ToString();
        }
    }
}