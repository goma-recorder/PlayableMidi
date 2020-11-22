using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Midity.Playable
{
    // Playable asset class that contains a MIDI animation clip
    [System.Serializable]
    public sealed class MidiAnimationAsset : PlayableAsset, ITimelineClipAsset
    {
        #region Serialized variables

        [field: SerializeField]
        public MidiAnimation template { get; private set; } = new MidiAnimation();

        #endregion

        #region PlayableAsset implementation

        public override double duration
        {
            get { return template.DurationInSecond; }
        }

        #endregion

        #region ITimelineClipAsset implementation

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending |
                       ClipCaps.Extrapolation |
                       ClipCaps.Looping |
                       ClipCaps.SpeedMultiplier;
            }
        }

        #endregion

        #region PlayableAsset overrides

        public override UnityEngine.Playables.Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return ScriptPlayable<MidiAnimation>.Create(graph, template);
        }

        #endregion
    }
}
