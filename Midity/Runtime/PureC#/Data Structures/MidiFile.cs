using System.Collections.Generic;
using System.Text;

namespace Midity
{
    public class MidiFile
    {
        public readonly byte format;
        private readonly List<MidiTrack> _tracks = new List<MidiTrack>();
        public readonly Encoding encoding;

        public MidiFile(uint deltaTime, Encoding encoding, byte format = 0)
        {
            DeltaTime = deltaTime;
            this.encoding = encoding;
            this.format = format;
        }

        public MidiFile(uint deltaTime, int codePage, byte format = 0)
            : this(deltaTime, Encoding.GetEncoding(codePage), format)
        {
        }

        public MidiFile(uint deltaTime, string codeName, byte format = 0)
            : this(deltaTime, Encoding.GetEncoding(codeName), format)
        {
        }

        public uint DeltaTime { get; }
        public IReadOnlyList<MidiTrack> Tracks => _tracks;

        public MidiTrack AddNewTrack(string name)
        {
            var track = new MidiTrack(name, DeltaTime);

            if (_tracks.Contains(track)) return null;

            _tracks.Add(track);
            return track;
        }

        public MidiTrack AddTrack(int trackNumber, List<MTrkEvent> events)
        {
            if (Tracks.Count < trackNumber) return null;
            var track = new MidiTrack(DeltaTime, events);
            _tracks.Add(track);
            return track;
        }
    }
}