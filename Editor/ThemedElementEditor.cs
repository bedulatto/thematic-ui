using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(ThemedElement))]
    public class ThemedElementEditor : Editor
    {
        SerializedProperty changeColorProp;
        SerializedProperty colorKeyProp;
        SerializedProperty changeFontProp;
        SerializedProperty fontKeyProp;
        SerializedProperty changeSpriteProp;
        SerializedProperty spriteKeyProp;
        SerializedProperty forceLayoutRebuildProp;
        SerializedProperty beforeThemeChangedProp;
        SerializedProperty afterThemeChangedProp;

        string[] colorKeys;
        int selectedColorKey;
        string[] fontKeys;
        int selectedFontKey;
        string[] spriteKeys;
        int selectedSpriteKey;

        bool initialized;
        bool allToggle;
        bool showKeys;
        bool showSettings;
        bool showEvents;
        string errorLabel;

        private void OnEnable()
        {
            changeColorProp = serializedObject.FindProperty("changeColor");
            colorKeyProp = serializedObject.FindProperty("colorKey");
            changeFontProp = serializedObject.FindProperty("changeFont");
            fontKeyProp = serializedObject.FindProperty("fontKey");
            changeSpriteProp = serializedObject.FindProperty("changeSprite");
            spriteKeyProp = serializedObject.FindProperty("spriteKey");
            forceLayoutRebuildProp = serializedObject.FindProperty("forceLayoutRebuild");
            beforeThemeChangedProp = serializedObject.FindProperty("beforeThemeChanged");
            afterThemeChangedProp = serializedObject.FindProperty("afterThemeChanged");

            FetchKeys();
        }

        void FetchKeys()
        {
            initialized = false;

            var themeManager = FindObjectOfType<ThemeManager>();
            if (!themeManager)
            {
                errorLabel = "Theme Manager instance is required in the scene.";
                return;
            }

            SerializedObject managerSerialized = new SerializedObject(themeManager);
            SerializedProperty themeAssetProp = managerSerialized.FindProperty("themeAsset");

            if (themeAssetProp == null || themeAssetProp.objectReferenceValue == null)
            {
                errorLabel = "ThemeAsset reference is missing in ThemeManager.";
                return;
            }

            ThemeAsset asset = themeAssetProp.objectReferenceValue as ThemeAsset;

            colorKeys = asset.Colors;
            selectedColorKey = asset.GetColorFieldIndex(colorKeyProp.stringValue);

            fontKeys = asset.Fonts;
            selectedFontKey = asset.GetFontFieldIndex(fontKeyProp.stringValue);

            spriteKeys = asset.Sprites;
            selectedSpriteKey = asset.GetSpriteFieldIndex(spriteKeyProp.stringValue);

            initialized = true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!initialized)
            {
                EditorGUILayout.HelpBox(errorLabel, MessageType.Error);
                return;
            }

            showKeys = EditorGUILayout.BeginFoldoutHeaderGroup(showKeys, "Keys");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (showKeys)
                DrawThemeKeys();

            EditorGUILayout.Space();

            showSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showSettings, "Settings");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (showSettings)
                EditorGUILayout.PropertyField(forceLayoutRebuildProp);

            EditorGUILayout.Space();

            showEvents = EditorGUILayout.BeginFoldoutHeaderGroup(showEvents, "Events");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (showEvents)
            {
                EditorGUILayout.PropertyField(beforeThemeChangedProp);
                EditorGUILayout.PropertyField(afterThemeChangedProp);
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        void DrawThemeKeys()
        {
            bool tempAllToggle = allToggle;
            tempAllToggle = EditorGUILayout.Toggle("Select All", tempAllToggle);

            if (allToggle != tempAllToggle)
            {
                allToggle = tempAllToggle;
                changeColorProp.boolValue = allToggle;
                changeFontProp.boolValue = allToggle;
                changeSpriteProp.boolValue = allToggle;
            }

            EditorGUILayout.BeginHorizontal();
            changeColorProp.boolValue = EditorGUILayout.Toggle("Set Color", changeColorProp.boolValue);
            if (changeColorProp.boolValue)
            {
                selectedColorKey = EditorGUILayout.Popup(selectedColorKey, colorKeys);
                if (selectedColorKey >= 0)
                    colorKeyProp.stringValue = colorKeys[selectedColorKey];
            }
            else
            {
                allToggle = false;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            changeFontProp.boolValue = EditorGUILayout.Toggle("Set Font", changeFontProp.boolValue);
            if (changeFontProp.boolValue)
            {
                selectedFontKey = EditorGUILayout.Popup(selectedFontKey, fontKeys);
                if (selectedFontKey >= 0)
                    fontKeyProp.stringValue = fontKeys[selectedFontKey];
            }
            else
            {
                allToggle = false;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            changeSpriteProp.boolValue = EditorGUILayout.Toggle("Set Sprite", changeSpriteProp.boolValue);
            if (changeSpriteProp.boolValue)
            {
                selectedSpriteKey = EditorGUILayout.Popup(selectedSpriteKey, spriteKeys);
                if (selectedSpriteKey >= 0)
                    spriteKeyProp.stringValue = spriteKeys[selectedSpriteKey];
            }
            else
            {
                allToggle = false;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
