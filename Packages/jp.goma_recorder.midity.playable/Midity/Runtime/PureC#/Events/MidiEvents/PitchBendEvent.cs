using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class PitchBendEvent : MidiEvent
    {
        public const byte StatusHead = 0xe0;
        public byte byte1;
        public byte byte2;

        internal PitchBendEvent(uint ticks, byte channel, byte byte1, byte byte2) : base(ticks, channel)
        {
            this.byte1 = byte1;
            this.byte2 = byte2;
        }

        public PitchBendEvent(byte channel, byte byte1, byte byte2) : this(0, channel, byte1, byte2)
        {
        }

        public byte Status => (byte) (StatusHead | Channel);

        protected override Type ToString(List<string> list)
        {
            return typeof(PitchBendEvent);
        }
    }
}