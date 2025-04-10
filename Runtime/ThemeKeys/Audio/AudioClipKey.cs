using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThematicUI
{
    [System.Serializable]
    public class AudioClipKey : ThemeKey
    {
        public AudioClip Value;

#if UNITY_EDITOR
        public override void DrawField()
        {
            Value = EditorGUILayout.ObjectField(Name, Value, typeof(AudioClip), false) as AudioClip;
        }
#endif
    }
}
