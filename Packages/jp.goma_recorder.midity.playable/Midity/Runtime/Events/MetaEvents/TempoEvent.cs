using UnityEngine;
using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class TempoEvent : MTrkEvent
    {
        public const byte EventNumber = 0x51;
        [SerializeField] 
        private uint _tickTempo;

        public uint TickTempo
        {
            get => _tickTempo;
            set => _tickTempo = Math.Min(Math.Max(0u, value), 0xff_ff_ff);
        }
        public float Tempo => 60000000f / TickTempo;
        
        public TempoEvent(){}

        public TempoEvent(uint ticks, uint tickTempo):base(ticks)
        {
            TickTempo = tickTempo;
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(Tempo.ToString());
            return typeof(TempoEvent);
        }
    }
}
