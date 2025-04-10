#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomPropertyDrawer(typeof(ThemeKeyAttribute))]
    public class ThemeKeyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, ThemeFieldType> keyTypeToFieldType = new()
        {
            { typeof(ColorKey), ThemeFieldType.Color },
            { typeof(FontKey), ThemeFieldType.Font },
            { typeof(SpriteKey), ThemeFieldType.Sprite },
            { typeof(AudioClipKey), ThemeFieldType.Audio },
            { typeof(MaterialKey), ThemeFieldType.Material },
            { typeof(TextureKey), ThemeFieldType.Texture },
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Only string fields are supported.");
                return;
            }

            var themeKeyAttribute = attribute as ThemeKeyAttribute;
            var keyType = themeKeyAttribute.KeyType;

            if (!keyTypeToFieldType.TryGetValue(keyType, out ThemeFieldType fieldType))
            {
                EditorGUI.HelpBox(position, $"Unsupported key type: {keyType.Name}", MessageType.Warning);
                return;
            }

            var target = property.serializedObject.targetObject as MonoBehaviour;
            if (target == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            ThemeAsset themeAsset = GetThemeAssetFromObject(target);
            if (themeAsset == null)
            {
                EditorGUI.HelpBox(position, "ThemeAsset not found or not assigned.", MessageType.Warning);
                return;
            }

            string[] keys = themeAsset.KeyReferences
                .Where(k => k.Type == fieldType)
                .Select(k => k.Name)
                .ToArray();

            if (keys == null || keys.Length == 0)
            {
                EditorGUI.HelpBox(position, $"No keys found for '{keyType.Name}' in ThemeAsset.", MessageType.Info);
                return;
            }

            int index = Array.IndexOf(keys, property.stringValue);
            if (index < 0) index = 0;

            int selected = EditorGUI.Popup(position, label.text, index, keys);
            property.stringValue = keys[selected];
        }

        private ThemeAsset GetThemeAssetFromObject(MonoBehaviour target)
        {
            Type type = target.GetType();
            while (type != null)
            {
                var field = type
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(f => f.FieldType == typeof(ThemeAsset));

                if (field != null)
                    return field.GetValue(target) as ThemeAsset;

                type = type.BaseType;
            }

            return null;
        }
    }
}
#endif
