using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThematicUI
{
    [System.Serializable]
    public class ColorKey : ThemeKey
    {
        public Color Value;

#if UNITY_EDITOR
        public override void DrawField()
        {
            Value = EditorGUILayout.ColorField(Name, Value);
        }
#endif
    }
}
