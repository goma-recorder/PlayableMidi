using System;
using UnityEditor;
using UnityEngine;

namespace Midity.Playable.Editor
{
    //
    // These CoreEditor classes are simply copy-pasted from the core render
    // pipeline package (com.unity.render-pipelines.core). They are useful to
    // build a multi segment inspector, but it's not preferable to have a
    // dependency to the render pipelines package just for this purpose.
    //

    internal static class CoreEditorStyles
    {
        public static readonly GUIStyle smallTickbox;
        public static readonly GUIStyle miniLabelButton;

        private static readonly Texture2D paneOptionsIconDark;
        private static readonly Texture2D paneOptionsIconLight;

        static CoreEditorStyles()
        {
            smallTickbox = new GUIStyle("ShurikenToggle");

            var transparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            transparentTexture.SetPixel(0, 0, Color.clear);
            transparentTexture.Apply();

            miniLabelButton = new GUIStyle(EditorStyles.miniLabel);
            miniLabelButton.normal = new GUIStyleState
            {
                background = transparentTexture,
                scaledBackgrounds = null,
                textColor = Color.grey
            };
            var activeState = new GUIStyleState
            {
                background = transparentTexture,
                scaledBackgrounds = null,
                textColor = Color.white
            };
            miniLabelButton.active = activeState;
            miniLabelButton.onNormal = activeState;
            miniLabelButton.onActive = activeState;

            paneOptionsIconDark = (Texture2D) EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
            paneOptionsIconLight = (Texture2D) EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
        }

        public static Texture2D paneOptionsIcon =>
            EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight;
    }

    internal static class CoreEditorUtils
    {
        public static void DrawSplitter(bool isBoxed = false)
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f);

            // Splitter rect should be full-width
            rect.xMin = 0f;
            rect.width += 4f;

            if (isBoxed)
            {
                rect.xMin = EditorGUIUtility.singleLineHeight - 2;
                rect.width -= 1;
            }

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }

        public static bool DrawHeaderToggle(string title, SerializedProperty group, SerializedProperty activeField,
            Action<Vector2> contextAction = null)
        {
            return DrawHeaderToggle(EditorGUIUtility.TrTextContent(title), group, activeField, contextAction);
        }

        public static bool DrawHeaderToggle(GUIContent title, SerializedProperty group, SerializedProperty activeField,
            Action<Vector2> contextAction = null)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            var backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            using (new EditorGUI.DisabledScope(!activeField.boolValue))
            {
                EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);
            }

            // Foldout
            group.serializedObject.Update();
            group.isExpanded = GUI.Toggle(foldoutRect, group.isExpanded, GUIContent.none, EditorStyles.foldout);
            group.serializedObject.ApplyModifiedProperties();

            // Active checkbox
            activeField.serializedObject.Update();
            activeField.boolValue = GUI.Toggle(toggleRect, activeField.boolValue, GUIContent.none,
                CoreEditorStyles.smallTickbox);
            activeField.serializedObject.ApplyModifiedProperties();

            // Context menu
            var menuIcon = CoreEditorStyles.paneOptionsIcon;
            var menuRect = new Rect(labelRect.xMax + 4f, labelRect.y + 4f, menuIcon.width, menuIcon.height);

            if (contextAction != null)
                GUI.DrawTexture(menuRect, menuIcon);

            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (contextAction != null && menuRect.Contains(e.mousePosition))
                {
                    contextAction(new Vector2(menuRect.x, menuRect.yMax));
                    e.Use();
                }
                else if (labelRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                        group.isExpanded = !group.isExpanded;
                    else if (contextAction != null)
                        contextAction(e.mousePosition);

                    e.Use();
                }
            }

            return group.isExpanded;
        }
    }
}