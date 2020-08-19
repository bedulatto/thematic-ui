using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(ThemedElement))]
    public class ThemedElementEditor : Editor
    {
        ThemedElement element;
        bool initialized;
        string errorLabel;
        bool allToggle;
        string[] colorKeys;
        int selectedColorKey;
        string[] fontKeys;
        int selectedFontKey;
        string[] spriteKeys;
        int selectedSpriteKey;

        bool showKeys;
        bool showSetting;
        bool showEvents;

        private void OnEnable()
        {
            element = (ThemedElement)target;
            if (!initialized && Application.isEditor)
                FetchKeys();
        }
        public void FetchKeys()
        {
            initialized = false;
            var themeManager = FindObjectOfType<ThemeManager>();
            if (!themeManager)
            {
                errorLabel = "Theme Manager instance needed";
                return;
            }
            var asset = themeManager.ThemeAsset;
            if (!asset)
            {
                errorLabel = "Theme Asset reference in Theme Manager needed";
                return;
            }

            selectedColorKey = asset.GetColorFieldIndex(element.colorKey);
            colorKeys = asset.Colors;

            selectedFontKey = asset.GetFontFieldIndex(element.fontKey);
            fontKeys = asset.Fonts;

            selectedSpriteKey = asset.GetSpriteFieldIndex(element.spriteKey);
            spriteKeys = asset.Sprites;

            initialized = true;
            showKeys = true;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (!initialized)
            {
                EditorGUILayout.LabelField(errorLabel, EditorStyles.boldLabel);
                return;
            }
            showKeys = EditorGUILayout.BeginFoldoutHeaderGroup(showKeys, "Keys");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (showKeys)
                DrawFields();

            EditorGUILayout.Space();

            showSetting = EditorGUILayout.BeginFoldoutHeaderGroup(showSetting, "Settings");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (showSetting)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ForceLayoutRebuild"));
            }
            EditorGUILayout.Space();
            showEvents = EditorGUILayout.BeginFoldoutHeaderGroup(showEvents, "Events");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (showEvents)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BeforeThemeChanged"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AfterThemeChanged"));
            }

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        public void DrawFields()
        {

            bool tempAllToggle = allToggle;
            tempAllToggle = EditorGUILayout.Toggle("Select All", tempAllToggle);
            if (allToggle != tempAllToggle)
            {
                allToggle = tempAllToggle;
                element.changeColor = tempAllToggle;
                element.changeFont = tempAllToggle;
                element.changeSprite = tempAllToggle;
            }
            EditorGUILayout.BeginHorizontal();
            element.changeColor = EditorGUILayout.Toggle("Set Color", element.changeColor);
            if (element.changeColor)
            {
                selectedColorKey = EditorGUILayout.Popup(selectedColorKey, colorKeys);
                if (selectedColorKey >= 0)
                    element.colorKey = colorKeys[selectedColorKey];
            }
            else { allToggle = false; }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            element.changeFont = EditorGUILayout.Toggle("Set Font", element.changeFont);
            if (element.changeFont)
            {
                selectedFontKey = EditorGUILayout.Popup(selectedFontKey, fontKeys);
                if (selectedFontKey >= 0)
                    element.fontKey = fontKeys[selectedFontKey];
            }
            else { allToggle = false; }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            element.changeSprite = EditorGUILayout.Toggle("Set Sprite", element.changeSprite);
            if (element.changeSprite)
            {
                selectedSpriteKey = EditorGUILayout.Popup(selectedSpriteKey, spriteKeys);
                if (selectedSpriteKey >= 0)
                    element.spriteKey = spriteKeys[selectedSpriteKey];
            }
            else { allToggle = false; }
            EditorGUILayout.EndHorizontal();

        }
    }
}
