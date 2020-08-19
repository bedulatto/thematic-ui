using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ThematicUI
{
    [System.Serializable]
    public class ColorKey : ThemeKey
    {
        public override ThemeFieldType FieldType => ThemeFieldType.Color;
        public Color Color;

        protected override void DrawContent()
        {
           Color = EditorGUILayout.ColorField(Color);
        }
    }
}