using UnityEngine;
using UnityEditor;

namespace Midity.Playable.Editor
{
    // Custom inspector for MidiSignalReceiver
    [CustomEditor(typeof(MidiSignalReceiver))]
    sealed class MidiSignalReceiverEditor : UnityEditor.Editor
    {
        SerializedProperty _noteFilter;
        SerializedProperty _noteOnEvent;
        SerializedProperty _noteOffEvent;

        static readonly GUIContent _labelNoteOctave = new GUIContent("Note/Octave");

        void OnEnable()
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
