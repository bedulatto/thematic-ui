using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ThematicUI
{
    [CustomEditor(typeof(ThemeManager))]
    public class ThemeManagerEditor : Editor
    {
        ThemeManager manager;
        string[] themes;
        ThemeAsset newAsset;
        int selectedInitialTheme;
        private void OnEnable()
        {
            manager = (ThemeManager)target;
            FetchThemes();
        }
        public void FetchThemes()
        {
            themes = new string[0];
            if (manager.ThemeAsset != null)
                for (int i = 0; i < manager.ThemeAsset.Themes.Length; i++)
                {
                    ArrayUtility.Add(ref themes, manager.ThemeAsset.Themes[i].name);
                    if (manager.InitialTheme == manager.ThemeAsset.Themes[i].name)
                        selectedInitialTheme = i;
                }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            newAsset = manager.ThemeAsset;
            newAsset = (ThemeAsset)EditorGUILayout.ObjectField(newAsset, typeof(ThemeAsset), false);
            if (newAsset != manager.ThemeAsset)
            {
                manager.ThemeAsset = newAsset;
                FetchThemes();
                GUI.changed = true;
            }
            if (themes != null && themes.Length > 0)
            {
                selectedInitialTheme = EditorGUILayout.Popup("Initial Theme",selectedInitialTheme, themes);
                if (selectedInitialTheme >= 0)
                    manager.InitialTheme = themes[selectedInitialTheme];
            }

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
