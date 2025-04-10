using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThematicUI
{
    [System.Serializable]
    public class MaterialKey : ThemeKey
    {
        public Material Value;

#if UNITY_EDITOR
        public override void DrawField()
        {
            Value = EditorGUILayout.ObjectField("Material", Value, typeof(Material), false) as Material;
        }
#endif
    }
}

