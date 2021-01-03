using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class ChannelPressureEvent : MidiEvent
    {
        public const byte StatusHead = 0xd0;
        public byte pressure;

        internal ChannelPressureEvent(uint ticks, byte channel, byte pressure) : base(ticks, channel)
        {
            this.pressure = pressure;
        }

        public ChannelPressureEvent(byte channel, byte pushData) : this(0, channel, pushData)
        {
        }

        public byte Status => (byte) (StatusHead | Channel);

        protected override Type ToString(List<string> list)
        {
            return typeof(ChannelPressureEvent);
        }
    }
}