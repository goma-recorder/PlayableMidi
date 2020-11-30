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
        string _tempoText;
        string _durationText;
        string _noteText;
        string _ccText;

        void OnEnable()
        {
            var asset = ((MidiAnimationAsset)target).template;

            _tempoText = asset.tempo.ToString();

            var bars = (float)asset.duration / (asset.ticksPerQuarterNote * 4);
            _durationText = bars.ToString() + (bars > 1 ? " bars" : " bar");

            var note = new HashSet<(byte number,NoteOctave octave,NoteName name)>();
            var cc = new HashSet<int>();

            foreach (var holder in asset.noteEvents)
            {
                var noteEvent = holder.Event;
                if(!noteEvent.isNoteOn)
                    note.Add((noteEvent.NoteNumber,noteEvent.noteOctave,noteEvent.noteName));
            }
            foreach (var holder in asset.controlChangeEvents)
            {
                var controlChangeEvent = holder.Event;
                cc.Add(controlChangeEvent.controlChangeNumber);
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
            EditorGUILayout.LabelField("Tempo", _tempoText);
            EditorGUILayout.LabelField("Duration", _durationText);
            EditorGUILayout.LabelField("Contained Events");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Note", _noteText);
            EditorGUILayout.LabelField("CC", _ccText);
            EditorGUI.indentLevel--;
        }
    }
}
