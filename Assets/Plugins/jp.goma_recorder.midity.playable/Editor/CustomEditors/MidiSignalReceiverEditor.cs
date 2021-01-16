using UnityEditor;
using UnityEngine;

namespace Midity.Playable.Editor
{
    // Custom inspector for MidiSignalReceiver
    [CustomEditor(typeof(MidiSignalReceiver))]
    internal sealed class MidiSignalReceiverEditor : UnityEditor.Editor
    {
        private static readonly GUIContent _labelNoteOctave = new GUIContent("Note/Octave");
        private SerializedProperty _noteFilter;
        private SerializedProperty _noteOffEvent;
        private SerializedProperty _noteOnEvent;

        private void OnEnable()
        {
            _noteFilter = serializedObject.FindProperty("noteFilter");
            _noteOnEvent = serializedObject.FindProperty("noteOnEvent");
            _noteOffEvent = serializedObject.FindProperty("noteOffEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_noteFilter, _labelNoteOctave);
            EditorGUILayout.PropertyField(_noteOnEvent);
            EditorGUILayout.PropertyField(_noteOffEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}