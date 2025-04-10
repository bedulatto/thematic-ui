using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThematicUI
{
    [System.Serializable]
    public class TextureKey : ThemeKey
    {
        public Texture Value;

#if UNITY_EDITOR
        public override void DrawField()
        {
            Value = EditorGUILayout.ObjectField(Name, Value, typeof(Texture), false) as Texture;
        }
#endif
    }
}
