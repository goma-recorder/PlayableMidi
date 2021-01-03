using System;
using System.Collections.Generic;

namespace Midity
{
    public sealed class NoteEvent : MidiEvent
    {
        public readonly bool isNoteOn;

        private byte _noteNumber;
        private byte _velocity;

        internal NoteEvent(uint ticks, bool isNoteOn, byte channel, byte noteNumber, byte velocity) : base(ticks,
            channel)
        {
            this.isNoteOn = isNoteOn;
            NoteNumber = noteNumber;
            Velocity = velocity;
        }

        internal NoteEvent(uint ticks, bool isNoteOn, byte channel, NoteName noteName, NoteOctave noteOctave,
            byte velocity) : this(ticks, isNoteOn, channel, NoteEnumUtil.ToNoteNumber(noteName, noteOctave), velocity)
        {
        }

        public byte Status => (byte) ((isNoteOn ? 0x90 : 0x80) | Channel);

        public byte NoteNumber
        {
            get => _noteNumber;
            internal set
            {
                if (value > 131)
                    throw new Exception("Numeric value out of range.(0-131)");
                _noteNumber = value;
            }
        }

        public NoteName NoteName
        {
            get => NoteEnumUtil.ToNoteName(NoteNumber);
            internal set => NoteNumber = NoteEnumUtil.ToNoteNumber(value, NoteOctave);
        }

        public NoteOctave NoteOctave
        {
            get => NoteEnumUtil.ToNoteOctave(NoteNumber);
            internal set => NoteNumber = NoteEnumUtil.ToNoteNumber(NoteName, value);
        }

        public byte Velocity
        {
            get => _velocity;
            internal set
            {
                if (isNoteOn)
                    _velocity = value;
            }
        }


        protected override Type ToString(List<string> list)
        {
            list.Add(isNoteOn.ToString());
            list.Add(NoteNumber.ToString());
            list.Add(Velocity.ToString());
            return typeof(NoteEvent);
        }
    }
}