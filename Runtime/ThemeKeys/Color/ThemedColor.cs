using UnityEngine;
using UnityEngine.UI;

namespace ThematicUI
{
    public class ThemedColor : ThemedElementBase
    {
        [ThemeKey(typeof(ColorKey))]
        [SerializeField] private string colorKey;

        [SerializeField] private Graphic target;

        protected override void ApplyTheme(Theme newTheme)
        {
            target.color = newTheme.Get<ColorKey>(colorKey).Value;
        }
    }
}