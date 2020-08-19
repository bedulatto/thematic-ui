using UnityEditor;
using UnityEngine;
namespace ThematicUI
{
    public enum ThemeFieldType { Color, Font, Sprite }
    [System.Serializable]
    public abstract class ThemeField
    {
        public string Name;
        public abstract ThemeFieldType FieldType { get; }
        public virtual void DrawField()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(Name);
            DrawContent();
            EditorGUILayout.EndVertical();
        }
        protected abstract void DrawContent();
    }
}
