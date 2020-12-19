using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace Midity.Playable.Editor
{
    // Custom importer for .mid files
    [ScriptedImporter(1, "mid")]
    sealed class MidiFileAssetImporter : ScriptedImporter
    {
        [SerializeField] private string _codeName = "us-ascii";

        public override void OnImportAsset(AssetImportContext context)
        {
            var assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            // Main MIDI file asset
            var buffer = File.ReadAllBytes(context.assetPath);
            var tracks = MidiFileDeserializer.Load(buffer, _codeName);
            var (asset, animations) = MidiPlayableTranslator.Translate(tracks);
            asset.name = assetName;
            context.AddObjectToAsset("MidiFileAsset", asset);
            context.SetMainObject(asset);

            // Contained tracks
            foreach (var track in animations)
                context.AddObjectToAsset(track.name, track);
        }
    }
}