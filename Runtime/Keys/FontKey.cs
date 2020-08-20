using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ThematicUI
{
    [System.Serializable]
    public class FontKey : ThemeKey
    {
        public override ThemeFieldType FieldType => ThemeFieldType.Font;
        public TMP_FontAsset Font;
#if UNITY_EDITOR
        protected override void DrawContent()
        {
            Font = (TMP_FontAsset)EditorGUILayout.ObjectField(Name,Font, typeof(TMP_FontAsset), true);
        }
#endif
    }
}
