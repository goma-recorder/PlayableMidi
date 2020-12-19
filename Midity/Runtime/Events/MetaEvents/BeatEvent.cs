using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class BeatEvent : MTrkEvent
    {
        public const byte EventNumber = 0x58;
        public byte[] data = new byte[4];
        public byte nn;
        public byte dd;
        public byte cc;
        public byte bb;
        
        public BeatEvent(){}

        public BeatEvent(uint ticks, byte nn, byte dd, byte cc, byte bb):base(ticks)
        {
            this.nn = nn;
            this.dd = dd;
            this.cc = cc;
            this.bb = bb;
        }

        protected override Type ToString(List<string> list)
        {
            return typeof(BeatEvent);
        }
    }
}
