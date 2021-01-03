using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class BeatEvent : MTrkEvent
    {
        public const byte EventNumber = 0x58;
        public byte nn;
        public byte dd;
        public byte cc;
        public byte bb;

        internal BeatEvent(uint ticks, byte nn, byte dd, byte cc, byte bb) : base(ticks)
        {
            this.nn = nn;
            this.dd = dd;
            this.cc = cc;
            this.bb = bb;
        }

        public BeatEvent(byte nn, byte dd, byte cc, byte bb) : this(0, nn, dd, cc, bb)
        {
        }

        public byte TopNumber => nn;
        public byte BottomNumber => (byte) Math.Pow(2, dd);

        protected override Type ToString(List<string> list)
        {
            list.Add(nn.ToString());
            list.Add(dd.ToString());
            list.Add(cc.ToString());
            list.Add(bb.ToString());
            return typeof(BeatEvent);
        }
    }
}