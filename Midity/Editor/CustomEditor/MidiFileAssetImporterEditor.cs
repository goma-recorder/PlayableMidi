using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace Midity.Playable.Editor
{
    [CustomEditor(typeof(MidiFileAssetImporter))]
    internal sealed class MidiFileAssetImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty _codeName;

        public override bool showImportedObject => false;

        public override void OnEnable()
        {
            base.OnEnable();
            _codeName = serializedObject.FindProperty("_codeName");
        }

        public override void OnInspectorGUI()
        {
            var assetImporter = (MidiFileAssetImporter) target;
            EditorGUILayout.LabelField("Format", assetImporter._midiFileAsset.MidiFile?.format.ToString());
            EditorGUILayout.LabelField("Delta Time", assetImporter._midiFileAsset.MidiFile?.DeltaTime.ToString());

            serializedObject.Update();
            EditorGUILayout.PropertyField(_codeName);
            serializedObject.ApplyModifiedProperties();

            ApplyRevertGUI();
        }
    }
}