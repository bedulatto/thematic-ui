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
        string[] colorKeys;
        int selectedColorKey;
        string[] fontKeys;
        int selectedFontKey;
        string[] spriteKeys;
        int selectedSpriteKey;
        private void OnEnable()
        {
            element = (ThemedElement)target;
            if (!initialized && Application.isEditor)
                FetchKeys();
        }
        public void FetchKeys()
        {
            initialized = false;
            if (ThemeSettings.Instance == null) return;

            selectedColorKey = ThemeSettings.Instance.GetColorFieldIndex(element.colorKey);
            colorKeys = ThemeSettings.Instance.Colors;

            selectedFontKey = ThemeSettings.Instance.GetFontFieldIndex(element.fontKey);
            fontKeys = ThemeSettings.Instance.Fonts;

            selectedSpriteKey = ThemeSettings.Instance.GetSpriteFieldIndex(element.spriteKey);
            spriteKeys = ThemeSettings.Instance.Sprites;

            initialized = true;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (!initialized)
                EditorGUILayout.LabelField("You need to create a Theme Setting to set up a Theme");
            else
                DrawFields();

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        public void DrawFields()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Change Color", GUILayout.Width(100f));
            element.changeColor = EditorGUILayout.Toggle(element.changeColor, GUILayout.Width(EditorGUIUtility.singleLineHeight));
            if (element.changeColor)
            {
                selectedColorKey = EditorGUILayout.Popup(selectedColorKey, colorKeys);
                if (selectedColorKey >= 0)
                    element.colorKey = colorKeys[selectedColorKey];
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Change Font", GUILayout.Width(100f));
            element.changeFont = EditorGUILayout.Toggle(element.changeFont, GUILayout.Width(EditorGUIUtility.singleLineHeight));
            if (element.changeFont)
            {
                selectedFontKey = EditorGUILayout.Popup(selectedFontKey, fontKeys);
                if (selectedFontKey >= 0)
                    element.fontKey = fontKeys[selectedFontKey];
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Change Sprite", GUILayout.Width(100f));
            element.changeSprite = EditorGUILayout.Toggle(element.changeSprite, GUILayout.Width(EditorGUIUtility.singleLineHeight));
            if (element.changeSprite)
            {
                selectedSpriteKey = EditorGUILayout.Popup(selectedSpriteKey, spriteKeys);
                if (selectedSpriteKey >= 0)
                    element.spriteKey = spriteKeys[selectedSpriteKey];
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
