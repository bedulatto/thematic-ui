using UnityEngine;
using UnityEngine.UI;

namespace ThematicUI
{
    public class ThemedImage : ThemedElementBase
    {
        [ThemeKey(typeof(SpriteKey))]
        [SerializeField] string spriteKey;
        [SerializeField] Image spriteTarget;

        protected override void ApplyTheme(Theme newTheme)
        {
            spriteTarget.sprite = newTheme.Get<SpriteKey>(spriteKey).Value;
        }
    }
}