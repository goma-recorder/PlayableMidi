using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Midity.Playable
{
    // Playable asset class that contains a MIDI animation clip
    [System.Serializable]
    public sealed class MidiAnimationAsset : PlayableAsset, ITimelineClipAsset
    {
        public MidiTrack MidiTrack => midiFileAsset.MidiFile.Tracks[trackNumber];

        private MidiAnimation _template;

        public MidiAnimation Template
        {
            get
            {
                if (_template != null)
                    return _template;
                _template = new MidiAnimation(this);
                return _template;
            }
        }

        internal uint TickDuration
        {
            get
            {
                var bars = MidiTrack.Bars;
                var tpq = MidiTrack.TicksPerQuarterNote;
                return bars * tpq * 4;
            }
        }

        public override double duration => MidiTrack.ConvertTicksToSecond(TickDuration);

        public ClipCaps clipCaps =>
            ClipCaps.Blending |
            ClipCaps.Extrapolation |
            ClipCaps.Looping |
            ClipCaps.SpeedMultiplier;


        public override UnityEngine.Playables.Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return ScriptPlayable<MidiAnimation>.Create(graph, Template);
        }

        [SerializeField] internal int trackNumber;
        [SerializeField] internal MidiFileAsset midiFileAsset;

        [SerializeField] internal int eventCount;
        [SerializeField] internal MTrkEventHolder<NoteEvent>[] noteEvents;
        [SerializeField] internal MTrkEventHolder<ControlChangeEvent>[] controlChangeEvents;
        [SerializeField] internal MTrkEventHolder<SequenceNumberEvent>[] sequenceNumberEvents;
        [SerializeField] internal MTrkEventHolder<TextEvent>[] textEvents;
        [SerializeField] internal MTrkEventHolder<CopyrightEvent>[] copyrightEvents;
        [SerializeField] internal MTrkEventHolder<TrackNameEvent>[] trackNameEvents;
        [SerializeField] internal MTrkEventHolder<InstrumentNameEvent>[] instrumentNameEvents;
        [SerializeField] internal MTrkEventHolder<LyricEvent>[] lyricEvents;
        [SerializeField] internal MTrkEventHolder<MarkerEvent>[] markerEvents;
        [SerializeField] internal MTrkEventHolder<QueueEvent>[] queueEvents;
        [SerializeField] internal MTrkEventHolder<ChannelPrefixEvent>[] channelPrefixEvents;
        [SerializeField] internal MTrkEventHolder<EndPointEvent>[] endPointEvents;
        [SerializeField] internal MTrkEventHolder<TempoEvent>[] tempoEvents;
        [SerializeField] internal MTrkEventHolder<SmpteOffsetEvent>[] smpteOffsetEvents;
        [SerializeField] internal MTrkEventHolder<BeatEvent>[] beatEvents;
        [SerializeField] internal MTrkEventHolder<KeyEvent>[] keyEvents;
        [SerializeField] internal MTrkEventHolder<SequencerUniqueEvent>[] sequencerUniqueEvents;

        internal List<MTrkEvent> Translate()
        {
            var eventList = new List<MTrkEvent>();
            var indexTable = new Dictionary<Type, int>();
            for (var listIndex = 0; listIndex < eventCount; listIndex++)
            {
                Search(noteEvents);
                Search(controlChangeEvents);
                Search(sequenceNumberEvents);
                Search(textEvents);
                Search(copyrightEvents);
                Search(trackNameEvents);
                Search(instrumentNameEvents);
                Search(lyricEvents);
                Search(markerEvents);
                Search(queueEvents);
                Search(channelPrefixEvents);
                Search(endPointEvents);
                Search(tempoEvents);
                Search(smpteOffsetEvents);
                Search(beatEvents);
                Search(keyEvents);
                Search(sequencerUniqueEvents);


                void Search<T>(MTrkEventHolder<T>[] events) where T : MTrkEvent
                {
                    var type = typeof(T);
                    if (!indexTable.ContainsKey(type))
                        indexTable.Add(type, 0);
                    var index = indexTable[type];

                    if (events.Length == index) return;

                    if (events[index].index == listIndex)
                    {
                        eventList.Add(events[index].Event);
                        indexTable[type]++;
                    }
                }
            }

            return eventList;
        }
    }
}