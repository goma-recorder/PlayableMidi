// using UnityEngine;
// using UnityEditor;
//
// namespace Midity.Playable.Editor
// {
//     [CustomEditor(typeof(MidiFileAsset))]
//     public class MidiFileAssetEditor : UnityEditor.Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             EditorApplication.QueuePlayerLoopUpdate();
//             serializedObject.Update();
//             var asset = (MidiFileAsset) target;
//             EditorGUILayout.LabelField("Format", asset.MidiFile.format.ToString());
//             EditorGUILayout.LabelField("Delta Time", asset.MidiFile.DeltaTime.ToString());
//             EditorGUILayout.LabelField("Code Page", asset.MidiFile.codePage.ToString());
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }