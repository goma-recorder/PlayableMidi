using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class ChannelPrefixEvent : MTrkEvent
    {
        public const byte EventNumber = 0x20;
        public byte data;

        internal ChannelPrefixEvent(uint ticks, byte data) : base(ticks)
        {
            this.data = data;
        }

        public ChannelPrefixEvent(byte data) : this(0, data)
        {
        }

        protected override Type ToString(List<string> list)
        {
            return typeof(ChannelPrefixEvent);
        }
    }
}