using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ThematicUI
{
    [System.Serializable]
    public class SpriteKey : ThemeKey
    {
        public override ThemeFieldType FieldType => ThemeFieldType.Color;
        public Sprite Sprite;
#if UNITY_EDITOR
        protected override void DrawContent()
        {
            Sprite = (Sprite)EditorGUILayout.ObjectField(Sprite, typeof(Sprite), true);
        }
#endif

    }
}
