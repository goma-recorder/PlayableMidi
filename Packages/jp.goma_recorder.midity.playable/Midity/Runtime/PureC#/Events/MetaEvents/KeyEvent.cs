using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class KeyEvent : MTrkEvent
    {
        public const byte EventNumber = 0x59;
        public NoteKey noteKey;

        internal KeyEvent(uint ticks, NoteKey noteKey) : base(ticks)
        {
            this.noteKey = noteKey;
        }

        public KeyEvent(NoteKey noteKey) : this(0, noteKey)
        {
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(noteKey.ToString());
            return typeof(KeyEvent);
        }
    }
}