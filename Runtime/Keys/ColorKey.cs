using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ThematicUI
{
    [System.Serializable]
    public class ColorKey : ThemeKey
    {
        public override ThemeFieldType FieldType => ThemeFieldType.Color;
        public Color Color;
#if UNITY_EDITOR
        protected override void DrawContent()
        {
           Color = EditorGUILayout.ColorField(Color);
        }
#endif
    }
}