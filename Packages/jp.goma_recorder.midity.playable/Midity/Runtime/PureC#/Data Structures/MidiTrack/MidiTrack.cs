using System;
using System.Collections.Generic;
using System.Linq;

namespace Midity
{
    public class MidiTrack
    {
        private readonly List<MTrkEvent> _events = new List<MTrkEvent>();
        private readonly SortedList<uint, TempoEvent> _tempoEvents = new SortedList<uint, TempoEvent>();
        private readonly List<NoteEventPair> _noteEventPairs = new List<NoteEventPair>();
        private TrackNameEvent _trackNameEvent;

        internal MidiTrack(string name, uint deltaTime)
        {
            DeltaTime = deltaTime;
            _events.Add(new TrackNameEvent(0, name));
            _events.Add(new EndPointEvent(0));
        }

        internal MidiTrack(uint deltaTime, List<MTrkEvent> events)
        {
            DeltaTime = deltaTime;
            _events = events;

            foreach (var x in Events)
            {
                AllTicks += x.Ticks;
                switch (x)
                {
                    case NoteEvent noteEvent:
                        break;
                    case TrackNameEvent trackNameEvent:
                        _trackNameEvent = trackNameEvent;
                        break;
                    case TempoEvent tempoEvent:
                        _tempoEvents.Add(AllTicks, tempoEvent);
                        break;
                }
            }

            AllSeconds = ConvertTicksToSecond(AllTicks);
        }

        public uint DeltaTime { get; }
        public uint AllTicks { get; private set; }
        public float AllSeconds { get; private set; }

        public IReadOnlyList<MTrkEvent> Events => _events;
        public IReadOnlyList<NoteEventPair> NoteEventPairs => _noteEventPairs;
        public uint Bars => (AllTicks + DeltaTime * 4 - 1) / (DeltaTime * 4);

        public string Name
        {
            get => _trackNameEvent?.name;
            set
            {
                if (_trackNameEvent == null)
                {
                    _trackNameEvent = new TrackNameEvent(0u, value);
                    _events.Insert(0, _trackNameEvent);
                    return;
                }

                _trackNameEvent.name = value;
            }
        }

        public uint ConvertSecondToTicks(float time)
        {
            var tempo = 120f;
            var ticks = 0u;
            var offsetTicks = 0u;
            foreach (var pair in _tempoEvents)
            {
                var length = (pair.Key - offsetTicks) * 60 / (tempo * DeltaTime);
                if (time > length)
                {
                    ticks += pair.Key - offsetTicks;
                    time -= length;
                    tempo = pair.Value.Tempo;
                    offsetTicks = pair.Key;
                }
                else
                {
                    break;
                }
            }

            ticks += (uint) (time * tempo / 60 * DeltaTime);
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
                    time += length * 60 / (tempo * DeltaTime);
                    tick -= length;
                    tempo = pair.Value.Tempo;
                    offsetTicks = pair.Key;
                }
                else
                {
                    break;
                }
            }

            time += tick * 60 / (tempo * DeltaTime);
            return time;
        }

        public bool GetAbsoluteTick(MTrkEvent mTrkEvent, out uint absoluteTick)
        {
            absoluteTick = 0u;
            foreach (var e in Events)
            {
                absoluteTick += e.Ticks;
                if (e == mTrkEvent)
                    return true;
            }

            absoluteTick = 0u;
            return false;
        }

        public bool GetAbsoluteTime(MTrkEvent mTrkEvent, out float time)
        {
            if (GetAbsoluteTick(mTrkEvent, out var tick))
            {
                time = ConvertTicksToSecond(tick);
                return true;
            }

            time = 0f;
            return false;
        }

        private void Validation<T>(T mTrkEvent) where T : MTrkEvent
        {
            switch (mTrkEvent)
            {
                case TrackNameEvent trackNameEvent:
                    throw new AggregateException($"{nameof(TrackNameEvent)}");
                case CopyrightEvent copyrightEvent:
                    throw new AggregateException($"{nameof(CopyrightEvent)}");
                case EndPointEvent endPointEvent:
                    throw new AggregateException($"{nameof(EndPointEvent)}");
            }
        }

        private void RegistTempoEvent(MTrkEvent mTrkEvent)
        {
            if (!(mTrkEvent is TempoEvent tempoEvent)) return;
            GetAbsoluteTick(tempoEvent, out var abstractTick);
            _tempoEvents.Add(abstractTick, tempoEvent);
            AllSeconds = ConvertTicksToSecond(AllTicks);
        }

        private void AddFirst(MTrkEvent mTrkEvent, uint offsetTicks)
        {
            var index = 0;
            for (; index < Events.Count; index++)
            {
                var e = Events[index];
                if (e.Ticks == 0)
                    switch (e)
                    {
                        case TempoEvent tempoEvent:
                        case TrackNameEvent trackNameEvent:
                        case CopyrightEvent copyrightEvent:
                            _events.RemoveAt(index);
                            _events.Insert(0, e);
                            continue;
                    }

                break;
            }

            _events.Insert(index, mTrkEvent);
            Events[index + 1].Ticks += offsetTicks;
            AllTicks += offsetTicks;
            RegistTempoEvent(mTrkEvent);
        }

        private void SortEnd(MTrkEvent mTrkEvent)
        {
            if (Events.Last() != mTrkEvent)
                return;
            var endPointEvent = Events[Events.Count - 2];
            if (!(endPointEvent is EndPointEvent)) throw new Exception();
            _events.RemoveAt(Events.Count - 2);
            _events.Add(endPointEvent);
            AllTicks += mTrkEvent.Ticks;
            RegistTempoEvent(mTrkEvent);
        }

        private void AddEvent(MTrkEvent mTrkEvent, int ticks, int index)
        {
            if (ticks >= 0)
            {
                for (var i = index + 1; i < Events.Count; i++)
                {
                    if (ticks <= Events[i].Ticks)
                    {
                        mTrkEvent.Ticks = (uint) ticks;
                        Events[i].Ticks -= (uint) ticks;
                        _events.Insert(i + 1, mTrkEvent);
                        RegistTempoEvent(mTrkEvent);
                        return;
                    }

                    ticks -= (int) Events[i].Ticks;
                }

                mTrkEvent.Ticks = (uint) ticks;
                _events.Add(mTrkEvent);
                SortEnd(mTrkEvent);
            }
            else
            {
                for (var i = index; i >= 0; i--)
                {
                    if (ticks >= 0)
                    {
                        mTrkEvent.Ticks = (uint) ticks;
                        Events[i + 1].Ticks -= (uint) ticks;
                        _events.Insert(i, mTrkEvent);
                        RegistTempoEvent(mTrkEvent);
                        return;
                    }

                    ticks += (int) Events[i].Ticks;
                }

                AddFirst(mTrkEvent, (uint) -ticks);
            }
        }

        public void AddEvent(MTrkEvent mTrkEvent, uint absoluteTick)
        {
            Validation(mTrkEvent);
            var ticks = absoluteTick;
            for (var i = 0; i < Events.Count; i++)
            {
                if (ticks < Events[i].Ticks)
                {
                    mTrkEvent.Ticks = ticks;
                    _events.Insert(i + 1, mTrkEvent);
                    SortEnd(mTrkEvent);
                    return;
                }

                ticks -= Events[i].Ticks;
            }

            mTrkEvent.Ticks = ticks;
            _events.Add(mTrkEvent);
            SortEnd(mTrkEvent);
        }

        public void AddEvent(MTrkEvent mTrkEvent, float absoluteTime)
        {
            var absoluteTick = ConvertSecondToTicks(absoluteTime);
            AddEvent(mTrkEvent, absoluteTick);
        }

        public void AddEvent(MTrkEvent originalEvent, MTrkEvent newEvent, int ticks = 0)
        {
            Validation(newEvent);
            var originalIndex = _events.IndexOf(originalEvent);
            AddEvent(newEvent, ticks, originalIndex);
        }

        public void RemoveEvent(MTrkEvent mTrkEvent)
        {
            Validation(mTrkEvent);
            var index = _events.IndexOf(mTrkEvent);
            if (index > 0)
                Events[index - 1].Ticks += mTrkEvent.Ticks;
            _events.RemoveAt(index);

            if (mTrkEvent is TempoEvent tempoEvent)
            {
                GetAbsoluteTick(tempoEvent, out var absoluteTick);
                _tempoEvents.Remove(absoluteTick);
                AllSeconds = ConvertSecondToTicks(AllTicks);
            }
        }

        public void MoveEvent(MTrkEvent mTrkEvent, int ticks)
        {
            if (ticks == 0) return;
            Validation(mTrkEvent);
            var index = _events.IndexOf(mTrkEvent);
            AddEvent(mTrkEvent, ticks, index);
            RemoveEvent(mTrkEvent);
        }

        public NoteEventPair AddNoteEvent(MTrkEvent rootEvent, NoteEvent onNoteEvent, uint length,
            float absoluteTime)
        {
            var absoluteTick = ConvertSecondToTicks(absoluteTime);
            return AddNoteEvent(rootEvent, onNoteEvent, length, absoluteTick);
        }

        private NoteEventPair AddNoteEvent(NoteEvent onNoteEvent, uint length, float absoluteTime)
        {
            AddEvent(onNoteEvent, absoluteTime);
            var offNoteEvent = AddOffNoteEvent(onNoteEvent, length);
            return new NoteEventPair(onNoteEvent, offNoteEvent, length);
        }

        private NoteEventPair AddNoteEvent(NoteEvent onNoteEvent, uint length, uint absoluteTick)
        {
            AddEvent(onNoteEvent, absoluteTick);
            var offNoteEvent = AddOffNoteEvent(onNoteEvent, length);
            return new NoteEventPair(onNoteEvent, offNoteEvent, length);
        }

        private NoteEventPair AddNoteEvent(MTrkEvent rootEvent, NoteEvent onNoteEvent, uint length,
            int ticks = 0)
        {
            AddEvent(rootEvent, onNoteEvent, ticks);
            var offNoteEvent = AddOffNoteEvent(onNoteEvent, length);
            return new NoteEventPair(onNoteEvent, offNoteEvent, length);
        }

        private NoteEvent AddOffNoteEvent(NoteEvent onNoteEvent, uint length)
        {
            var offNoteEvent = new NoteEvent(0, false, onNoteEvent.Channel, onNoteEvent.NoteNumber, 0);
            AddEvent(onNoteEvent, offNoteEvent, (int) length);
            return offNoteEvent;
        }
    }
}