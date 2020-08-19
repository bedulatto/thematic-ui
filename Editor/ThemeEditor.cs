using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(Theme))]
    public class ThemeEditor : Editor
    {
        Theme theme;
        bool initialized;
        private void OnEnable()
        {
            theme = (Theme)target;
            if (!initialized && Application.isEditor)
                UpdateFields();
        }

        void UpdateFields()
        {
            initialized = false;
            if (ThemeSettings.Instance == null) return;
            theme.Colors = MountFields<ThemeColor>(ThemeSettings.Instance.Colors, theme.Colors);
            theme.Fonts = MountFields<ThemeFont>(ThemeSettings.Instance.Fonts, theme.Fonts);
            theme.Sprites = MountFields<ThemeSprite>(ThemeSettings.Instance.Sprites, theme.Sprites);
            initialized = true;
        }
        T[] MountFields<T>(string[] keys, ThemeField[] fields) where T : ThemeField, new()
        {
            var newFields = new T[0];
            foreach (var key in keys)
            {
                if (fields != null)
                {
                    var field = fields.FirstOrDefault(ctx => ctx.Name == key);
                    if (field != null)
                    {
                        ArrayUtility.Add(ref newFields, (T)field);
                        continue;
                    }
                }
                var newField = new T();
                newField.Name = key;
                ArrayUtility.Add(ref newFields, newField);
            }
            return newFields;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (!initialized)
                EditorGUILayout.LabelField("You need to create a Theme Setting to set up a Theme");
            else
            {
                DrawLists();
            }

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
        void DrawLists()
        {
            DrawList(theme.Colors, "Colors");
            EditorGUILayout.Space();
            DrawList(theme.Fonts, "Fonts");
            EditorGUILayout.Space();
            DrawList(theme.Sprites, "Sprites");
        }
        void DrawList(ThemeField[] themeFields, string header)
        {
            EditorGUILayout.LabelField(header);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = 0; i < themeFields.Length; i++)
            {
                if (i > 0)
                    EditorGUILayout.Space();
                themeFields[i].DrawField();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
