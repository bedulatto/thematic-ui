#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace ThematicUI
{
    public enum ThemeFieldType { Color, Font, Sprite }
    [System.Serializable]
    public abstract class ThemeKey
    {
        public string Name;
        public abstract ThemeFieldType FieldType { get; }
#if UNITY_EDITOR
        public virtual void DrawField()
        {
            EditorGUILayout.BeginVertical();
            DrawContent();
            EditorGUILayout.EndVertical();
        }
        protected abstract void DrawContent();
#endif
    }
}
