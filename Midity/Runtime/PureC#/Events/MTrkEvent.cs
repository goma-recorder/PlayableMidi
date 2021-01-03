using System;
using System.Collections.Generic;
using System.Text;

namespace Midity
{
    public abstract class MTrkEvent
    {
        protected MTrkEvent(uint ticks)
        {
            Ticks = ticks;
        }

        public uint Ticks { get; internal set; }

        protected abstract Type ToString(List<string> list);

        public override string ToString()
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            var typeName = ToString(list).Name;
            sb.Append($"{typeName}: {Ticks}, ");
            sb.Append(string.Join(", ", list));
            return sb.ToString();
        }
    }
}