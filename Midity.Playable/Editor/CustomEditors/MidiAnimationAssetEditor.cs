using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Midity.Playable.Editor
{
    // Custom inspector for MIDI assets
    // There is no editable property; It just shows some infomation.
    [CustomEditor(typeof(MidiAnimationAsset))]
    class MidiAnimationAssetEditor : UnityEditor.Editor
    {
        private string _durationText;
        private string _noteText;
        private string _ccText;
        private readonly List<string> _eventTexts = new List<string>();

        void OnEnable()
        {
            var asset = ((MidiAnimationAsset) target);

            var track = asset.MidiTrack;
            var bars = track.Bars;
            _durationText = bars.ToString() + (bars > 1 ? " bars" : " bar");

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
                            note.Add((noteEvent.noteNumber, noteEvent.noteOctave, noteEvent.noteName));
                        break;
                    case ControlChangeEvent controlChangeEvent:
                        cc.Add(controlChangeEvent.controlChangeNumber);
                        break;
                }
            }

            if (note.Count == 0)
                _noteText = "-";
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
            foreach (var s in _eventTexts)
            {
                EditorGUILayout.LabelField(s);
            }
        }
    }
}