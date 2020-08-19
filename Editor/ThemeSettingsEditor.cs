using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace ThematicUI
{
    [CustomEditor(typeof(ThemeSettings))]
    public class ThemeSettingsEditor : Editor
    {
        ThemeSettings settings;

        private void OnEnable()
        {
            settings = (ThemeSettings)target;
        }
        public void DrawFieldList(string[] fields, ThemeFieldType fieldType)
        {
            EditorGUILayout.LabelField(fieldType.ToString());
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (fields != null)
                for (int i = 0; i < fields.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    fields[i] = EditorGUILayout.TextField(fields[i]);
                    if (GUILayout.Button("Remove"))
                    {
                        RemoveField(fields[i], fieldType);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            if (GUILayout.Button("Add"))
            {
                AddField(fieldType);
            }
            EditorGUILayout.EndVertical();
        }
        void AddField(ThemeFieldType fieldType)
        {
            switch (fieldType)
            {
                case ThemeFieldType.Color:
                    if (settings.Colors == null)
                        settings.Colors = new string[0];
                    ArrayUtility.Add(ref settings.Colors, "New Color");
                    break;
                case ThemeFieldType.Font:
                    if (settings.Fonts == null)
                        settings.Fonts = new string[0];
                    ArrayUtility.Add(ref settings.Fonts,"New Font");
                    break;
                case ThemeFieldType.Sprite:
                    if (settings.Sprites == null)
                        settings.Sprites = new string[0];
                    ArrayUtility.Add(ref settings.Sprites, "New Sprite");
                    break;
            }
        }
        void RemoveField(string field, ThemeFieldType fieldType)
        {
            switch (fieldType)
            {
                case ThemeFieldType.Color:
                    ArrayUtility.Remove(ref settings.Colors, field);
                    break;
                case ThemeFieldType.Font:
                    ArrayUtility.Remove(ref settings.Fonts, field);
                    break;
                case ThemeFieldType.Sprite:
                    ArrayUtility.Remove(ref settings.Sprites, field);
                    break;
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawFieldList(settings.Colors, ThemeFieldType.Color);
            DrawFieldList(settings.Fonts, ThemeFieldType.Font);
            DrawFieldList(settings.Sprites, ThemeFieldType.Sprite);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
