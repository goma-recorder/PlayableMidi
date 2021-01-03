using System;

namespace Midity
{
    public abstract class MidiEvent : MTrkEvent
    {
        private byte _channel;

        internal MidiEvent(uint ticks, byte channel) : base(ticks)
        {
            Channel = channel;
        }

        public byte Channel
        {
            get => _channel;
            internal set
            {
                if (value > 16)
                    throw new Exception("Numeric value out of range.(0-16)");
                _channel = value;
            }
        }
    }
}