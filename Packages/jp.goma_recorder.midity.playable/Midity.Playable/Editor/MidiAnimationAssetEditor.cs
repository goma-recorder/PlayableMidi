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

            var note = new HashSet<int>();
            var cc = new HashSet<int>();

            foreach (var holder in asset.midiEvents)
            {
                var midiEvent = holder.Event;
                switch (midiEvent.status & 0xf0u)
                {
                    case 0x80u:
                    case 0x81u: note.Add(midiEvent.data1); break;
                    case 0xb0u: cc.Add(midiEvent.data1); break;
                }
            }

            _noteText = note.Count == 0 ? "-" : string.Join(", ", note.OrderBy(x => x));
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
