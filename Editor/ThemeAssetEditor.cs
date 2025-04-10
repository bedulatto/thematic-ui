using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThematicUI.Editor
{
    [CustomEditor(typeof(ThemeAsset))]
    public class ThemeAssetEditor : UnityEditor.Editor
    {
        ThemeAsset asset;
        string newThemeName = "NewTheme";
        int selectedCurrentThemeIndex = -1;
        ThemeFieldType newKeyType = ThemeFieldType.Color;
        string newKeyName = "NewKey";

        Dictionary<int, string> editingOriginalNames = new();
        HashSet<int> editingIndices = new();
        Dictionary<ThemeFieldType, bool> foldoutStates = new();

        GUIStyle titleStyle;
        GUIStyle separatorStyle;
        bool initializedStyles = false;

        private void OnEnable()
        {
            asset = (ThemeAsset)target;
            if (asset.Themes == null)
                asset.Themes = new Theme[0];
        }

        public override void OnInspectorGUI()
        {
            if (!initializedStyles)
            {
                titleStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 13,
                    normal = { textColor = new Color(0.8f, 0.9f, 1f) }
                };

                separatorStyle = new GUIStyle(GUI.skin.box)
                {
                    margin = new RectOffset(0, 0, 10, 10),
                    padding = new RectOffset(10, 10, 5, 5)
                };

                initializedStyles = true;
            }

            serializedObject.Update();

            DrawHeader("🎨 Theme Configuration", 18);
            DrawThemeSection();

            EditorGUILayout.Space(10);
            DrawKeySection();

            EditorGUILayout.Space(10);
            DrawThemeSelectors();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        void DrawHeader(string title, int fontSize = 14)
        {
            GUIStyle style = new(EditorStyles.boldLabel)
            {
                fontSize = fontSize,
                normal = { textColor = Color.white }
            };
            EditorGUILayout.LabelField(title, style);
            EditorGUILayout.Space(4);
        }

        void DrawThemeSelectors()
        {
            EditorGUILayout.BeginVertical(separatorStyle);

            DrawHeader("🌈 Current Theme", 12);

            if (asset.Themes == null || asset.Themes.Length == 0)
            {
                EditorGUILayout.HelpBox("No themes registered. Use the section below to add your first theme.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            if (asset.CurrentTheme == null)
            {
                EditorGUILayout.HelpBox("No current theme is set. Add and apply a theme to begin using the system.", MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            string[] themeNames = asset.Themes.Select(t => t.name).ToArray();

            int currentIndex = System.Array.IndexOf(asset.Themes, asset.CurrentTheme);
            if (selectedCurrentThemeIndex < 0)
                selectedCurrentThemeIndex = currentIndex < 0 ? 0 : currentIndex;

            selectedCurrentThemeIndex = EditorGUILayout.Popup("Select Theme", selectedCurrentThemeIndex, themeNames);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Apply Current Theme"))
            {
                var selected = asset.Themes[selectedCurrentThemeIndex];
                asset.ChangeTheme(selected);
                EditorUtility.SetDirty(asset);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndVertical();
        }

        void DrawKeySection()
        {
            EditorGUILayout.BeginVertical(separatorStyle);
            DrawHeader("🗝️ Key References", 12);
            DrawValidationSummary();

            if (asset.KeyReferences == null || asset.KeyReferences.Count == 0)
            {
                EditorGUILayout.HelpBox("No key references defined. Use the form below to add one.", MessageType.Info);
            }
            else
            {
                var groupedKeys = asset.KeyReferences
                    .Select((refKey, index) => new { refKey, index })
                    .GroupBy(k => k.refKey.Type);

                foreach (var group in groupedKeys)
                {
                    if (!foldoutStates.ContainsKey(group.Key))
                        foldoutStates[group.Key] = true;

                    foldoutStates[group.Key] = EditorGUILayout.Foldout(foldoutStates[group.Key], $"{group.Key}", true);

                    if (foldoutStates[group.Key])
                    {
                        EditorGUI.indentLevel++;
                        foreach (var item in group)
                        {
                            var keyRef = item.refKey;
                            int i = item.index;

                            EditorGUILayout.BeginHorizontal();
                            bool isEditing = editingIndices.Contains(i);

                            if (isEditing)
                            {
                                if (!editingOriginalNames.ContainsKey(i))
                                    editingOriginalNames[i] = keyRef.Name;

                                keyRef.Name = EditorGUILayout.TextField(keyRef.Name);
                                keyRef.Type = (ThemeFieldType)EditorGUILayout.EnumPopup(keyRef.Type);

                                GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
                                if (GUILayout.Button("Save", GUILayout.Width(60)))
                                {
                                    editingIndices.Remove(i);
                                    editingOriginalNames.Remove(i);
                                    SyncAllThemesWithReferences(asset);
                                }

                                GUI.backgroundColor = new Color(1f, 0.5f, 0.3f);
                                if (GUILayout.Button("Discard", GUILayout.Width(80)))
                                {
                                    keyRef.Name = editingOriginalNames[i];
                                    editingIndices.Remove(i);
                                    editingOriginalNames.Remove(i);
                                    GUI.FocusControl(null);
                                }

                                GUI.backgroundColor = Color.white;
                            }
                            else
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.TextField(keyRef.Name);
                                EditorGUILayout.EnumPopup(keyRef.Type);
                                EditorGUI.EndDisabledGroup();

                                GUI.backgroundColor = new Color(0.4f, 0.6f, 1f);
                                if (GUILayout.Button("Edit", GUILayout.Width(60)))
                                    editingIndices.Add(i);

                                GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                                {
                                    if (EditorUtility.DisplayDialog("Confirm Removal", $"Are you sure you want to remove the key '{keyRef.Name}'?", "Remove", "Cancel"))
                                    {
                                        asset.KeyReferences.RemoveAt(i);
                                        SyncAllThemesWithReferences(asset);
                                        break;
                                    }
                                }

                                GUI.backgroundColor = Color.white;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.Space(5);
                }
            }

            EditorGUILayout.Space(10);
            DrawAddKeyField();
            EditorGUILayout.EndVertical();
        }

        void DrawValidationSummary()
        {
            var duplicates = asset.KeyReferences
                .GroupBy(k => new { k.Name, k.Type })
                .Where(g => !string.IsNullOrWhiteSpace(g.Key.Name) && g.Count() > 1)
                .ToList();

            bool hasEmptyNames = asset.KeyReferences.Any(k => string.IsNullOrWhiteSpace(k.Name));
            bool hasMissingInThemes = false;

            if (asset.Themes != null)
            {
                foreach (var theme in asset.Themes)
                {
                    foreach (var keyRef in asset.KeyReferences)
                    {
                        bool exists = theme.Keys.Any(k => k.Name == keyRef.Name && k.FieldType == keyRef.Type);
                        if (!exists)
                        {
                            hasMissingInThemes = true;
                            break;
                        }
                    }
                }
            }

            if (!hasEmptyNames && duplicates.Count == 0 && !hasMissingInThemes)
                return;

            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("⚠️ Some validation issues were found with your key references:", MessageType.Warning);

            if (hasEmptyNames)
            {
                EditorGUILayout.LabelField("• Some keys have empty names.", EditorStyles.miniLabel);
            }

            if (duplicates.Count > 0)
            {
                EditorGUILayout.LabelField("• Duplicate keys with the same type detected:", EditorStyles.miniLabel);
                foreach (var dup in duplicates)
                {
                    EditorGUILayout.LabelField($"    - {dup.Key.Name} ({dup.Key.Type})", EditorStyles.miniLabel);
                }
            }

            if (hasMissingInThemes)
            {
                EditorGUILayout.LabelField("• Some themes are missing values for defined keys.", EditorStyles.miniLabel);
            }

            EditorGUILayout.Space(4);
        }


        void DrawAddKeyField()
        {
            EditorGUILayout.LabelField("➕ Add New Key Reference", EditorStyles.miniBoldLabel);
            newKeyName = EditorGUILayout.TextField("Key Name", newKeyName);
            newKeyType = (ThemeFieldType)EditorGUILayout.EnumPopup("Key Type", newKeyType);

            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("Add Key Reference"))
            {
                if (!string.IsNullOrEmpty(newKeyName))
                {
                    asset.KeyReferences.Add(new ThemeKeyReference { Name = newKeyName, Type = newKeyType });
                    newKeyName = "NewKey";
                    EditorUtility.SetDirty(asset);
                    SyncAllThemesWithReferences(asset);
                }
            }
            GUI.backgroundColor = Color.white;
        }

        void DrawThemeSection()
        {
            EditorGUILayout.BeginVertical(separatorStyle);
            DrawHeader("📁 Registered Themes", 12);

            if (asset.Themes == null || asset.Themes.Length == 0)
            {
                EditorGUILayout.HelpBox("There are no registered themes. Use the form below to create a new one.", MessageType.Info);
            }
            else
            {
                foreach (var theme in asset.Themes)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(theme, typeof(Theme), false);
                    EditorGUI.EndDisabledGroup();

                    GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm Theme Removal", $"Are you sure you want to remove the theme '{theme.name}'?", "Remove", "Cancel"))
                        {
                            ArrayUtility.Remove(ref asset.Themes, theme);
                            AssetDatabase.RemoveObjectFromAsset(theme);
                            AssetDatabase.SaveAssets();
                            EditorUtility.SetDirty(asset);
                            break;
                        }
                    }
                    GUI.backgroundColor = Color.white;

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space(5);
            DrawAddThemeField();
            EditorGUILayout.EndVertical();
        }

        void DrawAddThemeField()
        {
            EditorGUILayout.LabelField("➕ Add New Theme", EditorStyles.miniBoldLabel);
            newThemeName = EditorGUILayout.TextField("Theme Name", newThemeName);

            GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
            if (GUILayout.Button("Add Theme"))
            {
                CreateTheme(); newThemeName = "NewTheme";
            }
            GUI.backgroundColor = Color.white;
        }

        void CreateTheme()
        {
            if (string.IsNullOrEmpty(newThemeName)) return;

            var newTheme = ScriptableObject.CreateInstance<Theme>();
            newTheme.name = newThemeName;
            newTheme.ThemeAsset = asset;

            ArrayUtility.Add(ref asset.Themes, newTheme);
            AssetDatabase.AddObjectToAsset(newTheme, asset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SyncAllThemesWithReferences(asset);
        }

        void SyncAllThemesWithReferences(ThemeAsset asset)
        {
            if (asset.Themes == null) return;

            string assetPath = AssetDatabase.GetAssetPath(asset);
            string directory = Path.GetDirectoryName(assetPath);

            foreach (var theme in asset.Themes)
            {
                if (theme == null) continue;

                theme.Keys.RemoveAll(k =>
                    !asset.KeyReferences.Any(r => r.Name == k.Name && r.Type == k.FieldType));

                foreach (var refKey in asset.KeyReferences)
                {
                    if (!theme.Keys.Any(k => k.Name == refKey.Name && k.FieldType == refKey.Type))
                    {
                        ThemeKey newKey = refKey.Type switch
                        {
                            ThemeFieldType.Color => new ColorKey(),
                            ThemeFieldType.Font => new FontKey(),
                            ThemeFieldType.Sprite => new SpriteKey(),
                            _ => null
                        };

                        if (newKey != null)
                        {
                            newKey.Name = refKey.Name;
                            newKey.FieldType = refKey.Type;
                            theme.Keys.Add(newKey);
                        }
                    }
                }

                EditorUtility.SetDirty(theme);
            }

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
