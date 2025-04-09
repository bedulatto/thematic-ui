using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        int selectedCurrentThemeIndex = -1;
        Dictionary<ThemeFieldType, HashSet<int>> editingKeyIndices = new(); 
        Dictionary<ThemeFieldType, Dictionary<int, string>> editingValues = new();
        Dictionary<ThemeFieldType, Dictionary<int, string>> originalKeyValues = new();

        private void OnEnable()
        {
            asset = (ThemeAsset)target;
            if (asset.Themes == null)
                asset.Themes = new Theme[0];

            editingKeyIndices[ThemeFieldType.Color] = new HashSet<int>();
            editingKeyIndices[ThemeFieldType.Font] = new HashSet<int>();
            editingKeyIndices[ThemeFieldType.Sprite] = new HashSet<int>();

            editingValues[ThemeFieldType.Color] = new Dictionary<int, string>();
            editingValues[ThemeFieldType.Font] = new Dictionary<int, string>();
            editingValues[ThemeFieldType.Sprite] = new Dictionary<int, string>();

            originalKeyValues[ThemeFieldType.Color] = new Dictionary<int, string>();
            originalKeyValues[ThemeFieldType.Font] = new Dictionary<int, string>();
            originalKeyValues[ThemeFieldType.Sprite] = new Dictionary<int, string>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            DrawHeader("Theme Configuration");
            DrawThemeSelectors();

            EditorGUILayout.Space(10);
            DrawHeader("Theme Keys");
            DrawKeySection("Color Keys", ThemeFieldType.Color, ref showColorKeys);
            DrawKeySection("Font Keys", ThemeFieldType.Font, ref showFontKeys);
            DrawKeySection("Sprite Keys", ThemeFieldType.Sprite, ref showSpriteKeys);

            EditorGUILayout.Space(10);
            DrawHeader("Theme List");
            showThemes = EditorGUILayout.Foldout(showThemes, "Registered Themes", true, EditorStyles.foldoutHeader);
            if (showThemes)
            {
                EditorGUILayout.BeginVertical("box");
                DrawThemes();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            DrawAddThemeField();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        void DrawThemeSelectors()
        {
            EditorGUILayout.BeginVertical("box");

            if (asset.Themes == null || asset.Themes.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "No themes registered. Use the field below to add your first theme.",
                    MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            if (asset.CurrentTheme == null)
            {
                EditorGUILayout.HelpBox(
                    "No current theme is set. Add and apply a theme to begin using the system.",
                    MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            string[] themeNames = asset.Themes.Select(t => t.name).ToArray();

            int currentIndex = ArrayUtility.IndexOf(asset.Themes, asset.CurrentTheme);
            if (selectedCurrentThemeIndex < 0)
                selectedCurrentThemeIndex = currentIndex < 0 ? 0 : currentIndex;

            selectedCurrentThemeIndex = EditorGUILayout.Popup("Current Theme", selectedCurrentThemeIndex, themeNames);

            if (GUILayout.Button("Apply Current Theme"))
            {
                var selected = asset.Themes[selectedCurrentThemeIndex];
                asset.ChangeTheme(selected);
                EditorUtility.SetDirty(asset);
            }

            EditorGUILayout.EndVertical();
        }


        void DrawKeySection(string label, ThemeFieldType fieldType, ref bool foldout)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, label, true, EditorStyles.foldoutHeader);
            if (GUILayout.Button("Add", GUILayout.Width(100)))
            {
                AddField(fieldType);
                foldout = true;
            }
            EditorGUILayout.EndHorizontal();

            if (foldout)
            {
                string[] keys = GetFieldArray(fieldType);

                if (keys == null || keys.Length == 0)
                {
                    string keyLabel = fieldType.ToString() + " Keys";
                    EditorGUILayout.HelpBox(
                        $"No {keyLabel} found. Click 'Add' to register a new key.",
                        MessageType.Info);
                }
                else
                {
                    DrawFieldList(keys, fieldType);
                }
            }

            EditorGUILayout.EndVertical();
        }

        void DrawFieldList(string[] fields, ThemeFieldType fieldType)
        {
            string removeKey = null;

            for (int i = 0; i < fields.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();

                bool isEditing = editingKeyIndices[fieldType].Contains(i);

                if (isEditing && !editingValues[fieldType].ContainsKey(i))
                {
                    editingValues[fieldType][i] = fields[i];
                    originalKeyValues[fieldType][i] = fields[i];
                }

                string displayValue = isEditing ? editingValues[fieldType][i] : fields[i];

                if (isEditing)
                {
                    string edited = EditorGUILayout.TextField(displayValue);
                    editingValues[fieldType][i] = edited;

                    if (GUILayout.Button("Save", GUILayout.Width(60)))
                    {
                        UpdateKeyValue(fieldType, i, edited);
                        editingKeyIndices[fieldType].Remove(i);
                        editingValues[fieldType].Remove(i);
                        originalKeyValues[fieldType].Remove(i);
                    }

                    if (GUILayout.Button("Discard", GUILayout.Width(80)))
                    {
                        RevertKeyValue(fieldType, i);
                        editingKeyIndices[fieldType].Remove(i);
                        editingValues[fieldType].Remove(i);
                        originalKeyValues[fieldType].Remove(i);
                        GUI.FocusControl(null); 
                    }
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField(displayValue);
                    EditorGUI.EndDisabledGroup();

                    if (GUILayout.Button("Edit", GUILayout.Width(60)))
                    {
                        editingKeyIndices[fieldType].Add(i);
                    }

                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        removeKey = fields[i];
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeKey != null)
                RemoveField(removeKey, fieldType);
        }

        void RevertKeyValue(ThemeFieldType fieldType, int index)
        {
            if (originalKeyValues[fieldType].TryGetValue(index, out var original))
            {
                UpdateKeyValue(fieldType, index, original);

                if (editingValues[fieldType].ContainsKey(index))
                {
                    editingValues[fieldType][index] = original;
                }
            }
        }

        void UpdateKeyValue(ThemeFieldType fieldType, int index, string newValue)
        {
            switch (fieldType)
            {
                case ThemeFieldType.Color:
                    asset.Colors[index] = newValue;
                    break;
                case ThemeFieldType.Font:
                    asset.Fonts[index] = newValue;
                    break;
                case ThemeFieldType.Sprite:
                    asset.Sprites[index] = newValue;
                    break;
            }
            EditorUtility.SetDirty(asset);
        }


        string[] GetFieldArray(ThemeFieldType type)
        {
            return type switch
            {
                ThemeFieldType.Color => asset.Colors,
                ThemeFieldType.Font => asset.Fonts,
                ThemeFieldType.Sprite => asset.Sprites,
                _ => null
            };
        }

        void AddField(ThemeFieldType fieldType)
        {
            switch (fieldType)
            {
                case ThemeFieldType.Color:
                    if (asset.Colors == null)
                        asset.Colors = new string[0];
                    ArrayUtility.Add(ref asset.Colors, "NewColorKey");
                    break;
                case ThemeFieldType.Font:
                    if (asset.Fonts == null)
                        asset.Fonts = new string[0];
                    ArrayUtility.Add(ref asset.Fonts, "NewFontKey");
                    break;
                case ThemeFieldType.Sprite:
                    if (asset.Sprites == null)
                        asset.Sprites = new string[0];
                    ArrayUtility.Add(ref asset.Sprites, "NewSpriteKey");
                    break;
            }
        }

        void RemoveField(string field, ThemeFieldType fieldType)
        {
            string typeName = fieldType.ToString().ToLower();

            bool confirmed = EditorUtility.DisplayDialog(
                "Confirm Removal",
                $"Are you sure you want to remove the {typeName} key \"{field}\"?",
                "Remove", "Cancel");

            if (!confirmed) return;

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

            EditorUtility.SetDirty(asset);
        }


        void DrawThemes()
        {
            if (asset.Themes == null || asset.Themes.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "There are no registered themes.\nUse the field below to create and add new themes to this asset.",
                    MessageType.Info);
                return;
            }

            for (int i = 0; i < asset.Themes.Length; i++)
            {
                asset.Themes[i].ThemeAsset = asset;
                EditorUtility.SetDirty(asset.Themes[i]);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(asset.Themes[i], typeof(Theme), false);
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Duplicate", GUILayout.Width(80)))
                {
                    Duplicate(asset.Themes[i]);
                }

                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    RemoveTheme(asset.Themes[i]);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        void DrawAddThemeField()
        {
            EditorGUILayout.BeginHorizontal();
            newThemeName = EditorGUILayout.TextField("New Theme Name", newThemeName);
            if (GUILayout.Button("Add Theme", GUILayout.Width(100)))
                CreateTheme();
            EditorGUILayout.EndHorizontal();
        }

        public void CreateTheme()
        {
            if (string.IsNullOrEmpty(newThemeName)) return;

            var newTheme = CreateInstance<Theme>();
            newTheme.name = newThemeName;
            newThemeName = "";
            newTheme.ThemeAsset = asset;
            AddThemeToAsset(newTheme);
        }

        public void Duplicate(Theme dup)
        {
            var newTheme = Instantiate(dup);
            newTheme.name = dup.name + "_Copy";
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
            bool confirmed = EditorUtility.DisplayDialog(
                "Confirm Theme Removal",
                $"Are you sure you want to remove the theme \"{theme.name}\"?",
                "Remove", "Cancel");

            if (!confirmed) return;

            ArrayUtility.Remove(ref asset.Themes, theme);
            AssetDatabase.RemoveObjectFromAsset(theme);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(asset);
        }

        void DrawHeader(string text)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
        }
    }
}
