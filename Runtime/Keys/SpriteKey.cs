using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ThematicUI
{
    [System.Serializable]
    public class SpriteKey : ThemeKey
    {
        public override ThemeFieldType FieldType => ThemeFieldType.Color;
        public Sprite Sprite;

        protected override void DrawContent()
        {
            Sprite = (Sprite)EditorGUILayout.ObjectField(Sprite, typeof(Sprite), true);
        }
    }
}
