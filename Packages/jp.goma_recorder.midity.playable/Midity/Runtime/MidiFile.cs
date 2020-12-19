using System.Collections.Generic;

namespace Midity
{
    public class MidiFile
    {
        public readonly byte format;
        public readonly int codePage;
        public uint DeltaTime { get; private set; }
        private readonly List<MidiTrack> _tracks;
        public IReadOnlyList<MidiTrack> Tracks => _tracks;

        public MidiFile(byte format, uint deltaTime = 96, int codePage = 20127, List<MidiTrack> tracks = null)
        {
            this.format = format;
            DeltaTime = deltaTime;
            this.codePage = codePage;
            _tracks = tracks ?? new List<MidiTrack>();
        }

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