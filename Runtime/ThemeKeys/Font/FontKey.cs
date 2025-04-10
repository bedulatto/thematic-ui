using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThematicUI
{
    [System.Serializable]
    public class FontKey : ThemeKey
    {
        public TMP_FontAsset Value;

#if UNITY_EDITOR
        public override void DrawField()
        {
            Value = (TMP_FontAsset)EditorGUILayout.ObjectField(Name, Value, typeof(TMP_FontAsset), false);
        }
#endif
    }
}
