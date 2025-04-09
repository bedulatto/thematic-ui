using TMPro;
using UnityEngine;

namespace ThematicUI
{
    public class ThemedFontTMP : ThemedElementBase
    {
        [ThemeKey("Fonts")]
        [SerializeField] private string fontKey;

        [SerializeField] private TextMeshProUGUI target;

        protected override void ApplyTheme(Theme newTheme)
        {
            target.font = newTheme.GetFont(fontKey).Font;
        }
    }
}