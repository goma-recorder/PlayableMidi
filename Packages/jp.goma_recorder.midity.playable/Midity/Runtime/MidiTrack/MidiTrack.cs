using System.Collections.Generic;

namespace Midity
{
    public class MidiTrack
    {
        #region Serialized variables

        public string Name { get; private set; } = "";

        public uint TicksPerQuarterNote { get; private set; }
        private readonly List<MTrkEvent> _events;
        public IReadOnlyList<MTrkEvent> Events => _events;
        SortedList<uint, TempoEvent> _tempoEvents = new SortedList<uint, TempoEvent>();

        public uint AllTicks { get; private set; }
        public float AllSeconds { get; private set; }
        public uint Bars => (AllTicks + TicksPerQuarterNote * 4 - 1) / (TicksPerQuarterNote * 4);

        internal MidiTrack(string name, uint ticksPerQuarterNote)
        {
            
        }
        internal MidiTrack(uint ticksPerQuarterNote, List<MTrkEvent> events)
        {
            TicksPerQuarterNote = ticksPerQuarterNote;
            _events = events ?? new List<MTrkEvent>();

            foreach (var x in Events)
            {
                AllTicks += x.ticks;
                switch (x)
                {
                    case TrackNameEvent trackNameEvent:
                        Name = trackNameEvent.name;
                        break;
                    case TempoEvent tempoEvent:
                        _tempoEvents.Add(AllTicks, tempoEvent);
                        break;
                }
            }

            AllSeconds = ConvertTicksToSecond(AllTicks);
        }

        public uint ConvertSecondToTicks(float time)
        {
            var tempo = 120f;
            var ticks = 0u;
            var offsetTicks = 0u;
            foreach (var pair in _tempoEvents)
            {
                var length = (pair.Key - offsetTicks)* 60 / (tempo * TicksPerQuarterNote);
                if (time > length)
                {
                    ticks += pair.Key - offsetTicks;
                    time -= length;
                    tempo = pair.Value.Tempo;
                    offsetTicks = pair.Key;
                }
                else
                    break;
            }

            ticks += (uint) (time * tempo / 60 * TicksPerQuarterNote);
            return ticks;
        }

        public float ConvertTicksToSecond(uint tick)
        {
            var tempo = 120f;
            var time = 0f;
            var offsetTicks = 0u;
            foreach (var pair in _tempoEvents)
            {
                var length = pair.Key - offsetTicks;
                if (tick > length)
                {
                    time += length * 60 / (tempo * TicksPerQuarterNote);
                    tick -= length;
                    tempo = pair.Value.Tempo;
                    offsetTicks = pair.Key;
                }
                else
                    break;
            }

            time += tick * 60 / (tempo * TicksPerQuarterNote);
            return time;
        }

        public bool GetAbstractTick(MTrkEvent mTrkEvent, out uint ticks)
        {
            ticks = 0u;
            foreach (var e in Events)
            {
                ticks += e.ticks;
                if (e == mTrkEvent)
                    return true;
            }

            ticks = 0u;
            return false;
        }

        public bool GetAbstractTime(MTrkEvent mTrkEvent, out float time)
        {
            if (GetAbstractTick(mTrkEvent, out var tick))
            {
                time = ConvertTicksToSecond(tick);
                return true;
            }
            else
            {
                time = 0f;
                return false;
            }
        }

        #endregion
    }
}