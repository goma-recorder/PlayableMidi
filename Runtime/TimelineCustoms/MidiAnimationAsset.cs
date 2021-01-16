using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Midity.Playable
{
    // Playable asset class that contains a MIDI animation clip
    [Serializable]
    public sealed class MidiAnimationAsset : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private MidiTrackAsset midiTrackAsset;

        private MidiAnimation _template;
        public MidiTrack MidiTrack => midiTrackAsset?.MidiTrack;

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

        private uint TickDuration
        {
            get
            {
                var bars = MidiTrack.Bars;
                var tpq = MidiTrack.DeltaTime;
                return bars * tpq * 4;
            }
        }

        public override double duration => MidiTrack?.ConvertTicksToSecond(TickDuration) ?? 50;

        public ClipCaps clipCaps =>
            ClipCaps.Blending |
            ClipCaps.Extrapolation |
            ClipCaps.Looping |
            ClipCaps.SpeedMultiplier;


        public override UnityEngine.Playables.Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return ScriptPlayable<MidiAnimation>.Create(graph, Template);
        }
    }
}