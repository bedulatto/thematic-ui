using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(ThemeManager))]
    public class ThemeManagerEditor : Editor
    {
        SerializedProperty themeAssetProp;
        SerializedProperty initialThemeProp;

        string[] themes;
        int selectedInitialTheme;

        private void OnEnable()
        {
            themeAssetProp = serializedObject.FindProperty("themeAsset");
            initialThemeProp = serializedObject.FindProperty("initialTheme");
            FetchThemes();
        }

        void FetchThemes()
        {
            themes = new string[0];
            selectedInitialTheme = 0;

            if (themeAssetProp.objectReferenceValue is ThemeAsset asset && asset.Themes != null)
            {
                themes = new string[asset.Themes.Length];
                for (int i = 0; i < asset.Themes.Length; i++)
                {
                    themes[i] = asset.Themes[i].name;
                    if (initialThemeProp.stringValue == themes[i])
                        selectedInitialTheme = i;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(themeAssetProp);

            if (themeAssetProp.objectReferenceValue is ThemeAsset asset && asset.Themes != null && asset.Themes.Length > 0)
            {
                FetchThemes();
                selectedInitialTheme = EditorGUILayout.Popup("Initial Theme", selectedInitialTheme, themes);
                if (selectedInitialTheme >= 0 && selectedInitialTheme < themes.Length)
                    initialThemeProp.stringValue = themes[selectedInitialTheme];
            }
            else
            {
                EditorGUILayout.HelpBox("No themes defined in the ThemeAsset.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
