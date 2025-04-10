using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(ThemeAsset))]
    public class ThemeAssetEditor : Editor
    {
        ThemeAsset asset;
        string newThemeName;
        int selectedCurrentThemeIndex = -1;
        ThemeFieldType newKeyType = ThemeFieldType.Color;
        string newKeyName = "NewKey";

        Dictionary<int, string> editingOriginalNames = new();
        HashSet<int> editingIndices = new();

        private void OnEnable()
        {
            asset = (ThemeAsset)target;
            if (asset.Themes == null)
                asset.Themes = new Theme[0];
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeader("Theme Configuration");
            DrawThemeSelectors();

            EditorGUILayout.Space(10);
            DrawKeySection();

            EditorGUILayout.Space(10);
            DrawThemeSection();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        void DrawHeader(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        void DrawThemeSelectors()
        {
            EditorGUILayout.BeginVertical("box");

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

            selectedCurrentThemeIndex = EditorGUILayout.Popup("Current Theme", selectedCurrentThemeIndex, themeNames);

            if (GUILayout.Button("Apply Current Theme"))
            {
                var selected = asset.Themes[selectedCurrentThemeIndex];
                asset.ChangeTheme(selected);
                EditorUtility.SetDirty(asset);
            }

            EditorGUILayout.EndVertical();
        }

        void DrawKeySection()
        {
            EditorGUILayout.BeginVertical("box");
            DrawHeader("Key References");

            if (asset.KeyReferences == null || asset.KeyReferences.Count == 0)
            {
                EditorGUILayout.HelpBox("No key references defined. Use the form below to add one.", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < asset.KeyReferences.Count; i++)
                {
                    var keyRef = asset.KeyReferences[i];
                    EditorGUILayout.BeginHorizontal();
                    bool isEditing = editingIndices.Contains(i);

                    if (isEditing)
                    {
                        if (!editingOriginalNames.ContainsKey(i))
                            editingOriginalNames[i] = keyRef.Name;

                        keyRef.Name = EditorGUILayout.TextField(keyRef.Name);
                        keyRef.Type = (ThemeFieldType)EditorGUILayout.EnumPopup(keyRef.Type);

                        if (GUILayout.Button("Save", GUILayout.Width(60)))
                        {
                            editingIndices.Remove(i);
                            editingOriginalNames.Remove(i);
                            asset.SyncAllThemesWithReferences();
                        }

                        if (GUILayout.Button("Discard", GUILayout.Width(80)))
                        {
                            keyRef.Name = editingOriginalNames[i];
                            editingIndices.Remove(i);
                            editingOriginalNames.Remove(i);
                            GUI.FocusControl(null);
                        }
                    }
                    else
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField(keyRef.Name);
                        EditorGUILayout.EnumPopup(keyRef.Type);
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("Edit", GUILayout.Width(60)))
                        {
                            editingIndices.Add(i);
                        }

                        if (GUILayout.Button("Remove", GUILayout.Width(80)))
                        {
                            if (EditorUtility.DisplayDialog("Confirm Removal", $"Are you sure you want to remove the key '{keyRef.Name}'?", "Remove", "Cancel"))
                            {
                                asset.KeyReferences.RemoveAt(i);
                                asset.SyncAllThemesWithReferences();
                                break;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space(5);
            DrawAddKeyField();
            EditorGUILayout.EndVertical();
        }

        void DrawAddKeyField()
        {
            EditorGUILayout.LabelField("Add New Key Reference", EditorStyles.miniBoldLabel);
            newKeyName = EditorGUILayout.TextField("Key Name", newKeyName);
            newKeyType = (ThemeFieldType)EditorGUILayout.EnumPopup("Key Type", newKeyType);

            if (GUILayout.Button("Add Key Reference"))
            {
                if (!string.IsNullOrEmpty(newKeyName))
                {
                    asset.KeyReferences.Add(new ThemeKeyReference { Name = newKeyName, Type = newKeyType });
                    newKeyName = "NewKey";
                    EditorUtility.SetDirty(asset);
                    asset.SyncAllThemesWithReferences();
                }
            }
        }

        void DrawThemeSection()
        {
            EditorGUILayout.BeginVertical("box");
            DrawHeader("Registered Themes");

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
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space(5);
            DrawAddThemeField();
            EditorGUILayout.EndVertical();
        }

        void DrawAddThemeField()
        {
            EditorGUILayout.LabelField("Add New Theme", EditorStyles.miniBoldLabel);
            newThemeName = EditorGUILayout.TextField("Theme Name", newThemeName);
            if (GUILayout.Button("Add Theme"))
                CreateTheme();
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

            asset.SyncAllThemesWithReferences();
        }
    }
}
