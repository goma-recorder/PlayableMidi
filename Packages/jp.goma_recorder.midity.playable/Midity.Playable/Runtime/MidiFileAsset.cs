using UnityEngine;

namespace Midity.Playable
{
    // ScriptableObject class used for storing a MIDI file asset
    public sealed class MidiFileAsset : ScriptableObject
    {
        [SerializeField] private byte format;
        [SerializeField] private uint deltaTime;
        [SerializeField] internal int codePage;
        [SerializeField] internal MidiAnimationAsset[] animationAssets;

        private MidiFile _midiFile;

        public MidiFile MidiFile
        {
            get
            {
                if (_midiFile != null) return _midiFile;

                _midiFile = new MidiFile(format, deltaTime, codePage);
                foreach (var animationAsset in animationAssets)
                    _midiFile.AddTrack(animationAsset.trackNumber, animationAsset.Translate());

                return _midiFile;
            }
        }

        public static MidiFileAsset Instantiate(MidiFile midiFile)
        {
            var fileAsset = CreateInstance<MidiFileAsset>();
            fileAsset.format = midiFile.format;
            fileAsset.deltaTime = midiFile.DeltaTime;
            fileAsset.codePage = midiFile.codePage;
            fileAsset._midiFile = midiFile;
            return fileAsset;
        }
    }
}