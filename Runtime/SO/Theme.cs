using UnityEngine;
namespace ThematicUI
{
    [System.Serializable]
    public class Theme : ScriptableObject
    {
        public ThemeAsset ThemeAsset;

        public ColorKey[] Colors;
        public FontKey[] Fonts;
        public SpriteKey[] Sprites;

        public ColorKey GetColor(string colorKey)
        {
            int colorIndex = ThemeAsset.GetColorFieldIndex(colorKey);
            if (colorIndex >= 0 && colorIndex < Colors.Length)
                return Colors[colorIndex];

            Debug.LogWarning($"Color key '{colorKey}' not found.");
            return default;
        }
        public FontKey GetFont(string fontKey)
        {
            int fontIndex = ThemeAsset.GetFontFieldIndex(fontKey);
            if (fontIndex >= 0 && fontIndex < Fonts.Length)
                return Fonts[fontIndex];

            Debug.LogWarning($"Font key '{fontKey}' not found.");
            return default;
        }
        public SpriteKey GetSprite(string spriteKey)
        {
            int spriteIndex = ThemeAsset.GetSpriteFieldIndex(spriteKey);
            if (spriteIndex >= 0 && spriteIndex < Sprites.Length)
                return Sprites[spriteIndex];

            Debug.LogWarning($"Sprite key '{spriteKey}' not found.");
            return default;
        }
    }
}
