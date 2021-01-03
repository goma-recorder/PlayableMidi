using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class TempoEvent : MTrkEvent
    {
        public const byte EventNumber = 0x51;
        private uint _tickTempo;

        internal TempoEvent(uint ticks, uint tickTempo) : base(ticks)
        {
            TickTempo = tickTempo;
        }

        public TempoEvent(uint tickTempo) : this(0, tickTempo)
        {
        }

        public uint TickTempo
        {
            get => _tickTempo;
            set => _tickTempo = Math.Min(Math.Max(0u, value), 0xff_ff_ff);
        }

        public float Tempo => 60000000f / TickTempo;

        protected override Type ToString(List<string> list)
        {
            list.Add(Tempo.ToString());
            return typeof(TempoEvent);
        }
    }
}