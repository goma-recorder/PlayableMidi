using System;
using UnityEngine;

namespace Midity
{
    public enum NoteName{C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B}
    public enum NoteOctave{Minus2, Minus1, Zero, Plus1, Plus2, Plus3, Plus4, Plus5, Plus6, Plus7, Plus8}
    [System.Serializable]
    public class NoteEvent : MTrkEvent
    {
        public bool isNoteOn;
        [SerializeField]
        private byte channel;
        public byte Channel
        {
            get => channel;
            set => channel = (byte)Mathf.Clamp(value, 0, 16);
        }

        public byte Status => (byte)((isNoteOn ? 0x90 : 0x80) | Channel);
        public NoteName noteName;
        public NoteOctave noteOctave;
        public byte NoteNumber => (byte)((int)noteOctave * 12 + (int)noteName);
        public byte velocity;
        
        public NoteEvent(){}

        public NoteEvent(uint ticks, bool isNoteOn, byte channel, NoteName noteName, NoteOctave noteOctave,
            byte velocity):base(ticks)
        {
            this.isNoteOn = isNoteOn;
            Channel = channel;
            this.noteName = noteName;
            this.noteOctave = noteOctave;
            this.velocity = velocity;
        }
    }   
}
