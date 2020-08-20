using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace ThematicUI
{
    [CustomEditor(typeof(ThemeAsset))]
    public class ThemeAssetEditor : Editor
    {
        ThemeAsset asset;
        bool showColorKeys;
        bool showFontKeys;
        bool showSpriteKeys;
        bool showThemes;
        string newThemeName;
        private void OnEnable()
        {
            asset = (ThemeAsset)target;
            if (asset.Themes == null)
                asset.Themes = new Theme[0];
        }
        public void DrawFieldList(string[] fields, ThemeFieldType fieldType)
        {
            string remove = null;
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (fields != null)
                for (int i = 0; i < fields.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    fields[i] = EditorGUILayout.TextField(fields[i]);
                    if (GUILayout.Button("Remove"))
                    {
                        remove = fields[i];
                    }
                    EditorGUILayout.EndHorizontal();
                }
            EditorGUILayout.EndVertical();
            if (remove != null)
                RemoveField(remove, fieldType);
        }
        void AddField(ThemeFieldType fieldType)
        {
            switch (fieldType)
            {
                case ThemeFieldType.Color:
                    if (asset.Colors == null)
                        asset.Colors = new string[0];
                    ArrayUtility.Add(ref asset.Colors, "ColorKey");
                    showColorKeys = true;
                    break;
                case ThemeFieldType.Font:
                    if (asset.Fonts == null)
                        asset.Fonts = new string[0];
                    ArrayUtility.Add(ref asset.Fonts, "FontKey");
                    showFontKeys = true;
                    break;
                case ThemeFieldType.Sprite:
                    if (asset.Sprites == null)
                        asset.Sprites = new string[0];
                    ArrayUtility.Add(ref asset.Sprites, "SpriteKey");
                    showSpriteKeys = true;
                    break;
            }
        }
        void RemoveField(string field, ThemeFieldType fieldType)
        {
            switch (fieldType)
            {
                case ThemeFieldType.Color:
                    ArrayUtility.Remove(ref asset.Colors, field);
                    break;
                case ThemeFieldType.Font:
                    ArrayUtility.Remove(ref asset.Fonts, field);
                    break;
                case ThemeFieldType.Sprite:
                    ArrayUtility.Remove(ref asset.Sprites, field);
                    break;
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            showColorKeys = EditorGUILayout.BeginFoldoutHeaderGroup(showColorKeys, "Color Keys") && asset.Colors.Length > 0;
            if (GUILayout.Button("Add Color Key"))
            {
                AddField(ThemeFieldType.Color);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndHorizontal();
            if (showColorKeys)
                DrawFieldList(asset.Colors, ThemeFieldType.Color);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            showFontKeys = EditorGUILayout.BeginFoldoutHeaderGroup(showFontKeys, "Font Keys") && asset.Fonts.Length > 0;
            if (GUILayout.Button("Add Font Key"))
            {
                AddField(ThemeFieldType.Font);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndHorizontal();
            if (showFontKeys)
                DrawFieldList(asset.Fonts, ThemeFieldType.Font);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            showSpriteKeys = EditorGUILayout.BeginFoldoutHeaderGroup(showSpriteKeys, "Sprite Keys") && asset.Sprites.Length > 0;
            if (GUILayout.Button("Add Sprite Key"))
            {
                AddField(ThemeFieldType.Sprite);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndHorizontal();
            if (showSpriteKeys)
                DrawFieldList(asset.Sprites, ThemeFieldType.Sprite);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            showThemes = EditorGUILayout.BeginFoldoutHeaderGroup(showThemes, "Themes");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (showThemes)
                DrawThemes();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            newThemeName = EditorGUILayout.TextField(newThemeName);
            if (GUILayout.Button("Add Theme"))
                CreateTheme();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
        public void DrawThemes()
        {
            for (int i = 0; i < asset.Themes.Length; i++)
            {
                asset.Themes[i].ThemeAsset = asset;
                EditorUtility.SetDirty(asset.Themes[i]);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(asset.Themes[i], typeof(Theme), false);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Duplicate"))
                {
                    Duplicate(asset.Themes[i]);
                }
                bool remove = GUILayout.Button("Remove");
                EditorGUILayout.EndHorizontal();
                if (remove)
                    RemoveTheme(asset.Themes[i]);
            }
        }
        public void CreateTheme()
        {
            var newTheme = CreateInstance<Theme>();

            if (string.IsNullOrEmpty(newThemeName)) return;
            newTheme.name = newThemeName;
            newThemeName = "";

            newTheme.ThemeAsset = asset;
            AddThemeToAsset(newTheme);
        }
        public void Duplicate(Theme dup)
        {
            var newTheme = Instantiate(dup);
            AddThemeToAsset(newTheme);
        }
        public void AddThemeToAsset(Theme newTheme)
        {
            ArrayUtility.Add(ref asset.Themes, newTheme);

            AssetDatabase.AddObjectToAsset(newTheme, asset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newTheme));
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(asset);
            AssetDatabase.Refresh();
            showThemes = true;
        }
        public void RemoveTheme(Theme theme)
        {
            if (EditorUtility.DisplayDialog(theme.name, "Remove " + theme.name + "?", "Ok", "Cancel"))
            {
                ArrayUtility.Remove(ref asset.Themes, theme);

                AssetDatabase.RemoveObjectFromAsset(theme);
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(asset);
            }
        }
    }
}
