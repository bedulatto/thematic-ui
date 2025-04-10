using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThematicUI
{
    [System.Serializable]
    public class SpriteKey : ThemeKey
    {
        public Sprite Value;

#if UNITY_EDITOR
        public override void DrawField()
        {
            Value = (Sprite)EditorGUILayout.ObjectField(Name, Value, typeof(Sprite), false);
        }
#endif
    }
}
