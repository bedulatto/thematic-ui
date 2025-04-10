using UnityEngine;

namespace ThematicUI
{
    public class ThemedRendererMaterial : ThemedElementBase
    {
        [ThemeKey(typeof(MaterialKey))]
        [SerializeField] private string materialKey;
        [SerializeField] private Renderer target;

        protected override void ApplyTheme(Theme theme)
        {
            var mat = theme.Get<MaterialKey>(materialKey);
            if (mat != null && target != null)
                target.material = mat.Value;
        }
    }
}