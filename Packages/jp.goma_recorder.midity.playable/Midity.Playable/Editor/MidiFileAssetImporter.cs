using System.IO;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace Midity.Playable.Editor
{
    // Custom importer for .mid files
    [ScriptedImporter(1, "mid")]
    sealed class MidiFileAssetImporter : ScriptedImporter
    {
        // [SerializeField] float _tempo = 120;

        public override void OnImportAsset(AssetImportContext context)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            // Main MIDI file asset
            var buffer = File.ReadAllBytes(context.assetPath);
            var tracks = MidiFileDeserializer.Load(buffer);
            var asset = MidiPlayableTranslator.Translate(tracks);
            asset.name = name;
            context.AddObjectToAsset("MidiFileAsset", asset);
            context.SetMainObject(asset);

            // Contained tracks
            for (var i = 0; i < asset.tracks.Length; i++)
            {
                var track = asset.tracks[i];
                // track.template.tempo = _tempo;
                context.AddObjectToAsset(track.name, track);
            }
        }
    }
}
