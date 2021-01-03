using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Midity.Playable.Editor
{
    // Custom inspector for MIDI assets
    // There is no editable property; It just shows some infomation.
    [CustomEditor(typeof(MidiTrackAsset))]
    internal class MidiTrackAssetEditor : UnityEditor.Editor
    {
        private readonly List<string> _eventTexts = new List<string>();
        private string _ccText;
        private string _durationText;
        private string _noteText;

        private void OnEnable()
        {
            var asset = (MidiTrackAsset) target;

            var track = asset.MidiTrack;
            var bars = track.Bars;
            _durationText = bars + (bars > 1 ? " bars" : " bar");

            var note = new HashSet<(byte number, NoteOctave octave, NoteName name)>();
            var cc = new HashSet<int>();

            _eventTexts.Clear();
            foreach (var e in track.Events)
            {
                _eventTexts.Add(e.ToString());
                switch (e)
                {
                    case NoteEvent noteEvent:
                        if (!noteEvent.isNoteOn)
                            note.Add((noteEvent.NoteNumber, noteEvent.NoteOctave, noteEvent.NoteName));
                        break;
                    case ControlChangeEvent controlChangeEvent:
                        cc.Add(controlChangeEvent.controlChangeNumber);
                        break;
                }
            }

            if (note.Count == 0)
            {
                _noteText = "-";
            }
            else
            {
                var sorted = note
                    .OrderBy(x => x.number)
                    .Select(x => $"{x.octave} {x.name}");
                _noteText = string.Join(",", sorted);
            }

            _ccText = cc.Count == 0 ? "-" : string.Join(", ", cc.OrderBy(x => x));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Duration", _durationText);
            EditorGUILayout.LabelField("Contained Events");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Note", _noteText);
            EditorGUILayout.LabelField("CC", _ccText);
            EditorGUI.indentLevel--;
            foreach (var s in _eventTexts) EditorGUILayout.LabelField(s);
        }
    }
}