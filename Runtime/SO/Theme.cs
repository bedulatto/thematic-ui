using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
            int colorIndex = ThemeAsset.Colors.Select((k, i) => new { key = k, index = i }).First(ctx => ctx.key == colorKey).index;
            var color = Colors[colorIndex];
            return color;
        }
        public FontKey GetFont(string fontKey)
        {
            int fontIndex = ThemeAsset.Fonts.Select((k, i) => new { key = k, index = i }).First(ctx => ctx.key == fontKey).index;
            var font = Fonts[fontIndex];
            return font;
        }
        public SpriteKey GetSprite(string spriteKey)
        {
            int spriteIndex = ThemeAsset.Sprites.Select((k, i) => new { key = k, index = i }).First(ctx => ctx.key == spriteKey).index;
            var sprite = Sprites[spriteIndex];
            return sprite;
        }
    }
}
