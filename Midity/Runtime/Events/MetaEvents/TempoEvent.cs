using UnityEngine;
using System;

namespace Midity
{
    [System.Serializable]
    public sealed class TempoEvent : MTrkEvent
    {
        public const byte EventNumber = 0x51;
        [SerializeField] 
        private uint _tickTempo;

        public uint tickTempo
        {
            get => _tickTempo;
            set => _tickTempo = Math.Min(Math.Max(0u, value), 0xff_ff_ff);
        }
        public float tempo => 60000000f / tickTempo;
        
        public TempoEvent(){}

        public TempoEvent(uint ticks, uint tickTempo):base(ticks)
        {
            this.tickTempo = tickTempo;
        }
    }
}
