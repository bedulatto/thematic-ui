using TMPro;
using UnityEngine;

namespace ThematicUI
{
    public class ThemedFontTMP : ThemedElementBase
    {
        [ThemeKey(typeof(FontKey))]
        [SerializeField] private string fontKey;

        [SerializeField] private TextMeshProUGUI target;

        protected override void ApplyTheme(Theme newTheme)
        {
            target.font = newTheme.Get<FontKey>(fontKey).Value;
        }
    }
}