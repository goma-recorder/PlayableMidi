namespace Midity
{
    // MIDI note event raw data struct
    [System.Serializable]
    public sealed class MidiEvent : MTrkEvent
    {
        public byte status;
        public byte data1;
        public byte data2;

        public bool IsCC { get { return (status & 0xb0) == 0xb0; } }
        public bool IsNote { get { return (status & 0xe0) == 0x80; } }
        public bool IsNoteOn { get { return (status & 0xf0) == 0x90; } }
        public bool IsNoteOff { get { return (status & 0xf0) == 0x80; } }
    }
}
