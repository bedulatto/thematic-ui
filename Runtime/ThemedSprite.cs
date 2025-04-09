using UnityEngine;
using UnityEngine.UI;

namespace ThematicUI
{
    public class ThemedSprite : ThemedElementBase
    {
        [ThemeKey("Sprites")][SerializeField] string spriteKey;
        [SerializeField] Image spriteTarget;

        protected override void ApplyTheme(Theme newTheme)
        {
            spriteTarget.sprite = newTheme.GetSprite(spriteKey).Sprite;
        }
    }
}