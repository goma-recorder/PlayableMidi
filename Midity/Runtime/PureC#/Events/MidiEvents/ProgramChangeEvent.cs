using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class ProgramChangeEvent : MidiEvent
    {
        public const byte StatusHead = 0xc0;
        public ushort programNumber;

        internal ProgramChangeEvent(uint ticks, byte channel, ushort programNumber) : base(ticks, channel)
        {
            this.programNumber = programNumber;
        }

        public ProgramChangeEvent(byte channel, ushort programNumber) : this(0, channel, programNumber)
        {
        }

        public byte Status => (byte) (StatusHead | Channel);

        protected override Type ToString(List<string> list)
        {
            return typeof(ProgramChangeEvent);
        }
    }
}