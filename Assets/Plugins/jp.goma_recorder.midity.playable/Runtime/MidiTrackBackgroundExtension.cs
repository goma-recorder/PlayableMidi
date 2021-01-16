using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Midity.Playable
{
    public static class MidiTrackBackgroundExtension
    {
        private static SortedDictionary<int, List<(uint onTick, uint length)>> ExtractNoteEvent(
            this MidiTrack midiTrack)
        {
            var noteTable = new SortedDictionary<int, List<(uint onTick, uint length)>>();
            var tickCash = new Dictionary<int, uint>();
            var currentTick = 0u;
            foreach (var mTrkEvent in midiTrack.Events)
            {
                currentTick += mTrkEvent.Ticks;
                if (!(mTrkEvent is NoteEvent noteEvent)) continue;

                if (noteEvent.isNoteOn)
                {
                    if (tickCash.ContainsKey(noteEvent.NoteNumber))
                        tickCash[noteEvent.NoteNumber] = currentTick;
                    else
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

        public static Texture2D WriteNoteBarTexture2D(this MidiTrack midiTrack, uint trackTicks, int noteWidthRate,
            int topMargin, int bottomMargin)
        {
            var noteTable = midiTrack.ExtractNoteEvent();
            if (noteTable.Count == 0)
                return new Texture2D(1, 1);

            var minNoteNumber = noteTable.First().Key;
            var noteNumberRange = noteTable.Last().Key - minNoteNumber;

            var texture = new Texture2D((int) trackTicks / noteWidthRate,
                bottomMargin + noteNumberRange + topMargin, TextureFormat.RGBA32, false, true)
            {
                filterMode = FilterMode.Point
            };

            var data = texture.GetRawTextureData<Color32>();
            foreach (var pair in noteTable)
            {
                var noteNumber = pair.Key;
                foreach (var (onTick, length) in pair.Value)
                    for (var x = (int) onTick / noteWidthRate; x < (onTick + length) / noteWidthRate; x++)
                    {
                        var y = noteNumber - minNoteNumber + bottomMargin;
                        data[x + y * texture.width] = Color.HSVToRGB(noteNumber % 12 / 12f, 0.85f, 0.87f);
                    }
            }

            texture.Apply();

            return texture;
        }
    }
}