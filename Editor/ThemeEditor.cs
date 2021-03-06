﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(Theme))]
    public class ThemeEditor : Editor
    {
        Theme theme;
        bool showColorValues;
        bool showFontValues;
        bool showSpriteValues;
        string rename;
        private void OnEnable()
        {
            theme = (Theme)target;
            GetKeys();
            rename = target.name;
            showColorValues = showFontValues = showSpriteValues = true;
        }
        void GetKeys()
        {
            theme.Colors = GetKeys<ColorKey>(theme.ThemeAsset.Colors, theme.Colors);
            theme.Fonts = GetKeys<FontKey>(theme.ThemeAsset.Fonts, theme.Fonts);
            theme.Sprites = GetKeys<SpriteKey>(theme.ThemeAsset.Sprites, theme.Sprites);
        }
        T[] GetKeys<T>(string[] keys, ThemeKey[] fields) where T : ThemeKey, new()
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

            EditorGUILayout.BeginHorizontal();
            rename = EditorGUILayout.TextField("Theme Name", rename);
            if (GUILayout.Button("Apply"))
            {
                Rename();
            }
            EditorGUILayout.EndHorizontal();

            DrawLists();

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
        void Rename()
        {
            string path = AssetDatabase.GetAssetPath(theme.ThemeAsset);
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                if (AssetDatabase.IsSubAsset(assets[i]))
                {
                    if (assets[i].name == target.name)
                    {
                        assets[i].name = rename;
                        EditorUtility.SetDirty(assets[i]);
                    }
                }
            }
            EditorUtility.SetDirty(theme);
            AssetDatabase.ImportAsset(path);
        }
        void DrawLists()
        {
            showColorValues = EditorGUILayout.BeginFoldoutHeaderGroup(showColorValues, "Color Values") && theme.Colors.Length > 0;
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (showColorValues)
                DrawList(theme.Colors);

            EditorGUILayout.Space();

            showFontValues = EditorGUILayout.BeginFoldoutHeaderGroup(showFontValues, "Font Values") && theme.Fonts.Length > 0;
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (showFontValues)
                DrawList(theme.Fonts);

            EditorGUILayout.Space();

            showSpriteValues = EditorGUILayout.BeginFoldoutHeaderGroup(showSpriteValues, "Sprite Values") && theme.Sprites.Length > 0;
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (showSpriteValues)
                DrawList(theme.Sprites);
        }
        void DrawList(ThemeKey[] themeFields)
        {
            for (int i = 0; i < themeFields.Length; i++)
            {
                if (i > 0)
                    EditorGUILayout.Space();
                themeFields[i].DrawField();
            }
        }
    }
}
