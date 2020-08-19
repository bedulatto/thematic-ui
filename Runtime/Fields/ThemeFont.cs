using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
namespace ThematicUI
{
    [System.Serializable]
    public class ThemeFont : ThemeField
    {
        public override ThemeFieldType FieldType => ThemeFieldType.Font;
        public TMP_FontAsset Font;

        protected override void DrawContent()
        {
            Font = (TMP_FontAsset)EditorGUILayout.ObjectField(Font, typeof(TMP_FontAsset), true);
        }
    }
}
