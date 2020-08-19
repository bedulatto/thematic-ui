using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace ThematicUI
{
    [CreateAssetMenu(fileName = "New Theme", menuName = "ThematicUI/Theme")]
    [System.Serializable]
    public class Theme : ScriptableObject
    {
        public ThemeColor[] Colors;
        public ThemeFont[] Fonts;
        public ThemeSprite[] Sprites;

        public ThemeColor GetColor(string colorKey)
        {
            int colorIndex = ThemeSettings.Instance.Colors.Select((k, i) => new { key = k, index = i }).First(ctx => ctx.key == colorKey).index;
            var color = Colors[colorIndex];
            return color;
        }
        public ThemeFont GetFont(string fontKey)
        {
            int fontIndex = ThemeSettings.Instance.Fonts.Select((k, i) => new { key = k, index = i }).First(ctx => ctx.key == fontKey).index;
            var font = Fonts[fontIndex];
            return font;
        }
        public ThemeSprite GetSprite(string spriteKey)
        {
            int spriteIndex = ThemeSettings.Instance.Sprites.Select((k, i) => new { key = k, index = i }).First(ctx => ctx.key == spriteKey).index;
            var sprite = Sprites[spriteIndex];
            return sprite;
        }
    }
}
