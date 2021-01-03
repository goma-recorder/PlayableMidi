using UnityEngine;

namespace Midity
{
    public class MidiTrackAsset : ScriptableObject
    {
        [SerializeField] private MidiFileAsset midiFileAsset;
        [SerializeField] private int trackNumber;

        public MidiTrack MidiTrack => midiFileAsset.MidiFile.Tracks[trackNumber];

        public static MidiTrackAsset Instantiate(MidiFileAsset midiFileAsset, int trackNumber)
        {
            var asset = CreateInstance<MidiTrackAsset>();
            asset.midiFileAsset = midiFileAsset;
            asset.trackNumber = trackNumber;
            return asset;
        }
    }
}