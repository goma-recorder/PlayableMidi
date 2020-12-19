// using System.Linq;
// using System.Text;
// using UnityEditor;
// using UnityEditor.Experimental.AssetImporters;
//
// namespace Midity.Playable.Editor
// {
//     [CustomEditor(typeof(MidiFileAssetImporter))]
//     sealed class MidiFileAssetImporterEditor : ScriptedImporterEditor
//     {
//         private string[] _codeNames;
//         private SerializedProperty _codePage;
//         private int _lastIndex;
//         public override bool showImportedObject => false;
//
//         public override void OnEnable()
//         {
//             base.OnEnable();
//             _codePage = serializedObject.FindProperty("_codeName");
//             _codeNames = Encoding.GetEncodings().Select(x => x.Name).ToArray();
//             for (var i = 0; i < _codeNames.Length; i++)
//                 if (_codeNames[i] == _codePage.stringValue)
//                     _lastIndex = i;
//         }
//
//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();
//             var currentIndex = EditorGUILayout.Popup("Code page", _lastIndex, _codeNames);
//             if (currentIndex != _lastIndex)
//             {
//                 _codePage.stringValue = _codeNames[currentIndex];
//                 _lastIndex = currentIndex;
//             }
//
//             serializedObject.ApplyModifiedProperties();
//             var assetImporter = (MidiFileAssetImporter) target;
//             EditorGUILayout.LabelField("Format", assetImporter._midiFileAsset.MidiFile.format.ToString());
//             EditorGUILayout.LabelField("Code Page", assetImporter._midiFileAsset.MidiFile.codePage.ToString());
//
//             // base.OnInspectorGUI();
//             ApplyRevertGUI();
//
//
//             // これ以降の要素に関してエディタによる変更を記録
//             // EditorGUI.BeginChangeCheck();
//
//             // ラベルの作成
//             // var label = "List";
//             // 初期値として表示する項目のインデックス番号
//             // var selectedIndex = 5;
//             // プルダウンメニューに登録する文字列配列
//             // var displayOptions = Extend.list;
//             // // プルダウンメニューの作成
//             // var index = Extend.list.Length > 0
//             //     ? 
//             //     : -1;
//
//             // if (index != extend.index)
//             // {
//             //     // インデックス番号が変わったら番号をログ出力
//             //     Debug.Log(index);
//             // }
//
//             // if (EditorGUI.EndChangeCheck())
//             // {
//             //     // 操作を Undo に登録
//             //     // Extend クラスの変更を記録
//             //     var objectToUndo = extend;
//             //     // Undo メニューに表示する項目名
//             //     var name = "Extend";
//             //     // 記録準備
//             //     Undo.RecordObject(objectToUndo, name);
//             //     // インデックス番号を登録
//             //     extend.index = index;
//             // }
//         }
//     }
// }