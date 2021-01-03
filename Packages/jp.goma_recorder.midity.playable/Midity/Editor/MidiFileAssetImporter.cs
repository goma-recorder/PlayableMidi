using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Midity.Playable.Editor
{
    // Custom importer for .mid files
    [ScriptedImporter(1, "mid")]
    internal sealed class MidiFileAssetImporter : ScriptedImporter
    {
        [SerializeField] private string _codeName = "us-ascii";
        [SerializeField] internal MidiFileAsset _midiFileAsset;

        public override void OnImportAsset(AssetImportContext context)
        {
            var assetName = Path.GetFileNameWithoutExtension(assetPath);

            // Main MIDI file asset
            var buffer = File.ReadAllBytes(context.assetPath);
            var deserializer = new MidiDeserializer(buffer, _codeName);
            var (midiFile, trackBytes) = deserializer.LoadTrackBytes();

            var fileAsset = MidiFileAsset.Instantiate(midiFile, assetName, trackBytes);
            fileAsset.name = assetName;
            context.AddObjectToAsset("MidiFileAsset", fileAsset);
            context.SetMainObject(fileAsset);

            var trackCount = midiFile.Tracks.Count;
            var trackAssets = new MidiTrackAsset[trackCount];
            for (var i = 0; i < trackCount; i++)
            {
                trackAssets[i] = MidiTrackAsset.Instantiate(fileAsset, i);
                trackAssets[i].name = $"{i}:{midiFile.Tracks[i].Name}";
            }

            // Contained tracks
            foreach (var track in trackAssets)
                context.AddObjectToAsset(track.name, track);
            _midiFileAsset = fileAsset;
            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}