using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midity
{
    [System.Serializable]
    public class NoteEvent : MTrkEvent
    {
        public bool isNoteOn;
        [SerializeField] private byte channel;

        public byte Channel
        {
            get => channel;
            set => channel = (byte) Mathf.Clamp(value, 0, 16);
        }

        public byte Status => (byte) ((isNoteOn ? 0x90 : 0x80) | Channel);

        public byte noteNumber;

        public NoteName noteName
        {
            get => (NoteName) (noteNumber % 12);
            set => noteNumber = (byte) (value + noteNumber / 12);
        }

        public NoteOctave noteOctave
        {
            get => (NoteOctave) (noteNumber / 12);
            set => noteNumber = (byte) ((byte) value * 12 + noteNumber % 12);
        }

        public byte velocity;

        public NoteEvent()
        {
        }

        public NoteEvent(uint ticks, bool isNoteOn, byte channel, byte noteNumber, byte velocity) : base(ticks)
        {
            this.isNoteOn = isNoteOn;
            Channel = channel;
            this.noteNumber = noteNumber;
            this.velocity = velocity;
        }

        public NoteEvent(uint ticks, bool isNoteOn, byte channel, NoteName noteName, NoteOctave noteOctave,
            byte velocity) : this(ticks, isNoteOn, channel, (byte) ((int) noteName + (int) noteOctave * 12), velocity)
        {
        }

        protected override Type ToString(List<string> list)
        {
            list.Add(isNoteOn.ToString());
            list.Add(noteNumber.ToString());
            list.Add(velocity.ToString());
            return typeof(NoteEvent);
        }
    }
}