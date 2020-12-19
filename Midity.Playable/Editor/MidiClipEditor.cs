using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using Debug = UnityEngine.Debug;

namespace Midity.Playable.Editor
{
    [CustomTimelineEditor(typeof(MidiAnimationAsset))]
    public class MidiClipEditor : ClipEditor
    {
        private readonly Dictionary<TimelineClip, Texture> _textures = new Dictionary<TimelineClip, Texture>();
        private readonly Dictionary<TimelineClip, Material> _materials = new Dictionary<TimelineClip, Material>();

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            if (!(clip.asset is MidiAnimationAsset midiAnimationAsset))
                return;

            var midiTrack = midiAnimationAsset.MidiTrack;

            Texture texture;
            if (_textures.ContainsKey(clip) && _textures[clip] == null)
                texture = _textures[clip];
            else
            {
                const int topMargin = 2;
                const int bottomMargin = 1;
                texture = midiTrack.WriteNoteBarTexture2D(midiTrack.AllTicks,
                    (int) midiTrack.TicksPerQuarterNote / 2, topMargin,
                    bottomMargin);
                if (_textures.ContainsKey(clip))
                    _textures[clip] = texture;
                else
                    _textures.Add(clip, texture);
            }

            Material material;
            if (_materials.ContainsKey(clip) && _materials[clip] == null)
                material = _materials[clip];
            else
            {
                var shader = Shader.Find("jp.goma_recorder.Midity.Playable/ClipBackground");
                material = new Material(shader) {mainTexture = texture};
                
                if (_materials.ContainsKey(clip))
                    _materials[clip] = material;
                else
                    _materials.Add(clip, material);
            }

            var loopCount = (region.endTime - region.startTime) / midiTrack.AllSeconds;
            material.SetFloat("_RepeatX", (float) loopCount);
            material.SetFloat("_OffsetX", (float) (region.startTime / midiTrack.AllSeconds));
            var rect = region.position;
            var quantizedRect = new Rect(Mathf.Ceil(rect.x), Mathf.Ceil(rect.y), Mathf.Ceil(rect.width),
                Mathf.Ceil(rect.height));

            Graphics.DrawTexture(quantizedRect, texture, material);
        }
    }
}