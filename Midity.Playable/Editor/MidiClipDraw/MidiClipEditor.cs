using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace Midity.Playable.Editor
{
    [CustomTimelineEditor(typeof(MidiAnimationAsset))]
    public class MidiClipEditor : ClipEditor
    {
        private readonly Dictionary<TimelineClip, Texture2D> _textures = new Dictionary<TimelineClip, Texture2D>();
        private readonly Dictionary<TimelineClip, Material> _materials = new Dictionary<TimelineClip, Material>();
        private Texture2D _texture;
        private Material _material;

        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            if (!(clip.asset is MidiAnimationAsset midiAnimationAsset))
                return;
            var midiTrack = midiAnimationAsset.template.Track;
            var noteTable = ExtractNoteEvent(midiTrack);
            _texture = WriteNoteBarTexture2D(noteTable, midiTrack.AllTicks, 20, 5, 5);
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            if (!(clip.asset is MidiAnimationAsset midiAnimationAsset))
                return;
            var midiTrack = midiAnimationAsset.template.Track;
            if (_texture == null)
            {
                var noteTable = ExtractNoteEvent(midiTrack);
                const int noteWidthRate = 1;
                const int topMargin = 2;
                const int bottomMargin = 1;
                _texture = WriteNoteBarTexture2D(noteTable, midiTrack.AllTicks, noteWidthRate, topMargin, bottomMargin);
            }

            var rect = region.position;
            var quantizedRect = new Rect(Mathf.Ceil(rect.x), Mathf.Ceil(rect.y), Mathf.Ceil(rect.width),
                Mathf.Ceil(rect.height));
            if (_material == null)
            {
                var shader = Shader.Find("jp.goma_recorder.Midity.Playable/ClipBackground");
                _material = new Material(shader) {mainTexture = _texture};
            }

            if (Event.current.type != EventType.Repaint) return;
            var trackSeconds = midiTrack.ConvertTicksToSecond(midiTrack.AllTicks);
            var loopCount = (region.endTime - region.startTime) / trackSeconds;
            _material.SetFloat("_RepeatX", (float) loopCount);
            _material.SetFloat("_OffsetX", (float) (region.startTime / trackSeconds));
            Graphics.DrawTexture(quantizedRect, _texture, _material);
        }

        SortedDictionary<int, List<(uint onTick, uint length)>> ExtractNoteEvent(MidiTrack midiTrack)
        {
            var noteTable = new SortedDictionary<int, List<(uint onTick, uint length)>>();
            var tickCash = new Dictionary<int, uint>();
            var currentTick = 0u;
            foreach (var mTrkEvent in midiTrack.events)
            {
                currentTick += mTrkEvent.ticks;
                if (!(mTrkEvent is NoteEvent noteEvent)) continue;

                if (noteEvent.isNoteOn && noteEvent.velocity != 0)
                {
                    tickCash.Add(noteEvent.NoteNumber, currentTick);
                }
                else
                {
                    if (!tickCash.TryGetValue(noteEvent.NoteNumber, out var onTick))
                        continue;

                    if (!noteTable.ContainsKey(noteEvent.NoteNumber))
                        noteTable.Add(noteEvent.NoteNumber, new List<(uint onTick, uint offTick)>());
                    noteTable[noteEvent.NoteNumber].Add((onTick, currentTick - onTick));
                    tickCash.Remove(noteEvent.NoteNumber);
                }
            }

            return noteTable;
        }

        Texture2D WriteNoteBarTexture2D(SortedDictionary<int, List<(uint onTick, uint length)>> noteTable,
            uint trackTicks, int noteWidthRate, int topMargin, int bottomMargin)
        {
            var minNoteNumber = noteTable.First().Key;
            var noteNumberRange = noteTable.Last().Key - minNoteNumber;
            var texture = new Texture2D((int) trackTicks / noteWidthRate, bottomMargin + noteNumberRange + topMargin)
            {
                filterMode = FilterMode.Point
            };
            foreach (var pair in noteTable)
            {
                var noteNumber = pair.Key;
                foreach (var (onTick, length) in pair.Value)
                    for (var i = (int) onTick / noteWidthRate; i < (onTick + length) / noteWidthRate; i++)
                    {
                        texture.SetPixel(i, noteNumber - minNoteNumber + bottomMargin,
                            Color.HSVToRGB((noteNumber % 12) / 12f, 0.85f, 0.87f));
                    }
            }

            texture.Apply();

            return texture;
        }
    }
}