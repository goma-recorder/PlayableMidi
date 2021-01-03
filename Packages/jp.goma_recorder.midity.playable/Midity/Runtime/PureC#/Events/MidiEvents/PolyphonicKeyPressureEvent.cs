using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class PolyphonicKeyPressureEvent : MidiEvent
    {
        public const byte StatusHead = 0xa0;
        public byte noteNumber;
        public byte pressure;

        internal PolyphonicKeyPressureEvent(uint ticks, byte channel, byte noteNumber, byte pressure) : base(ticks,
            channel)
        {
            this.noteNumber = noteNumber;
            this.pressure = pressure;
        }

        public PolyphonicKeyPressureEvent(byte channel, byte noteNumber, byte pressure) : this(0, channel, noteNumber,
            pressure)
        {
        }

        public byte Status => (byte) (StatusHead | Channel);

        protected override Type ToString(List<string> list)
        {
            return typeof(PolyphonicKeyPressureEvent);
        }
    }
}