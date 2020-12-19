using System;
using System.Collections.Generic;

namespace Midity
{
    [System.Serializable]
    public sealed class KeyEvent : MTrkEvent
    {
        public const byte EventNumber = 0x59;
        public NoteKey key;
        
        public KeyEvent(){}

        public KeyEvent(uint ticks, NoteKey key):base(ticks)
        {
            this.key = key;
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(key.ToString());
            return typeof(KeyEvent);
        }
    }
}
