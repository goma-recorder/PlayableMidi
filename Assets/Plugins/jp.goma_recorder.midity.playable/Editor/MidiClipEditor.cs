using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Midity.Playable.Editor
{
    [CustomTimelineEditor(typeof(MidiAnimationAsset))]
    public class MidiClipEditor : ClipEditor
    {
        private readonly Dictionary<TimelineClip, Material> _materials = new Dictionary<TimelineClip, Material>();
        private readonly Dictionary<TimelineClip, Texture> _textures = new Dictionary<TimelineClip, Texture>();

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            if (!(clip.asset is MidiAnimationAsset midiAnimationAsset))
                return;

            var midiTrack = midiAnimationAsset.MidiTrack;

            if (midiTrack == null)
                return;

            Texture texture;
            if (_textures.ContainsKey(clip) && _textures[clip] is null)
                texture = _textures[clip];
            else
            {
                const int topMargin = 2;
                const int bottomMargin = 1;
                texture = midiTrack.WriteNoteBarTexture2D((int) midiTrack.DeltaTime / 2, topMargin, bottomMargin);
                if (_textures.ContainsKey(clip))
                    _textures[clip] = texture;
                else
                    _textures.Add(clip, texture);
            }

            Material material;
            if (_materials.ContainsKey(clip) && _materials[clip] is null)
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

            var loopCount = (region.endTime - region.startTime) / midiTrack.TotalSeconds;
            material.SetFloat("_RepeatX", (float) loopCount);
            material.SetFloat("_OffsetX", (float) (region.startTime / midiTrack.TotalSeconds));
            var rect = region.position;
            var quantizedRect = new Rect(Mathf.Ceil(rect.x), Mathf.Ceil(rect.y), Mathf.Ceil(rect.width),
                Mathf.Ceil(rect.height));

            Graphics.DrawTexture(quantizedRect, texture, material);
        }
    }
}