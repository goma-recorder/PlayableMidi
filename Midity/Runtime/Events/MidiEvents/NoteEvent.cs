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
        public NoteName noteName;
        public NoteOctave noteOctave;
        public byte NoteNumber => (byte)((int)noteOctave * 12 + (int)noteName);
        public byte velocity;
        
        public NoteEvent(){}
        public NoteEvent(uint ticks, byte status, byte noteNumber, byte velocity)
        {
            this.ticks = ticks;
            isNoteOn = (status & 0xf0) == 0x90;
            Channel = (byte)(status & 0x0f);
            noteName = (NoteName)(noteNumber % 12);
            noteOctave = (NoteOctave)(noteNumber / 12);
            this.velocity = velocity;
        }
    }   
}
