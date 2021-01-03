using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class ControlChangeEvent : MidiEvent
    {
        public const byte StatusHead = 0xb0;
        public byte controlChangeNumber;
        public byte data;

        internal ControlChangeEvent(uint ticks, byte channel, byte controlChangeNumber, byte data) : base(ticks,
            channel)
        {
            this.controlChangeNumber = controlChangeNumber;
            this.data = data;
        }

        public ControlChangeEvent(byte channel, byte controlChangeNumber, byte data) : this(0, channel,
            controlChangeNumber, data)
        {
        }

        public byte Status => (byte) (StatusHead | Channel);

        protected override Type ToString(List<string> list)
        {
            list.Add(controlChangeNumber.ToString());
            list.Add(data.ToString());
            return typeof(ControlChangeEvent);
        }
    }
}