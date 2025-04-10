using UnityEngine;

namespace ThematicUI
{
    [ExecuteInEditMode]
    public abstract class ThemedElementBase : MonoBehaviour
    {
        [SerializeField] private ThemeAsset themeAsset;

        private void OnEnable()
        {
            if (themeAsset == null) return;

            themeAsset.OnThemeChanged += ApplyTheme;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this == null || themeAsset == null) return;
                    ApplyTheme(themeAsset.CurrentTheme);
                };
            }
            else
#endif
            {
                ApplyTheme(themeAsset.CurrentTheme);
            }
        }

        private void OnDisable()
        {
            if (themeAsset == null) return;
            themeAsset.OnThemeChanged -= ApplyTheme;
        }

        protected abstract void ApplyTheme(Theme newTheme);
    }
}
