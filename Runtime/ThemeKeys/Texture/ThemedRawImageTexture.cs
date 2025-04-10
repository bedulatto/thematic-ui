using UnityEngine;
using UnityEngine.UI;

namespace ThematicUI
{
    public class ThemedRawImageTexture : ThemedElementBase
    {
        [ThemeKey(typeof(TextureKey))]
        [SerializeField] private string textureKey;
        [SerializeField] private RawImage target;

        protected override void ApplyTheme(Theme theme)
        {
            var tex = theme.Get<TextureKey>(textureKey);
            if (tex != null && target != null)
                target.texture = tex.Value;
        }
    }
}