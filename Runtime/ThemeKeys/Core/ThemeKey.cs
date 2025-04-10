using UnityEngine;

namespace ThematicUI
{
    public enum ThemeFieldType
    {
        Color,
        Font,
        Sprite,
        Audio,
        Material,
        Texture,
    }

    [System.Serializable]
    public abstract class ThemeKey
    {
        public string Name;
        public ThemeFieldType FieldType;

#if UNITY_EDITOR
        public abstract void DrawField();
#endif
    }
}
