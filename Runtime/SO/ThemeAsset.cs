using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace ThematicUI
{
    [CreateAssetMenu(fileName = "Theme Asset", menuName = "ThematicUI/Theme Asset")]
    public class ThemeAsset : ScriptableObject
    {
        public List<ThemeKeyReference> KeyReferences = new();
        public Theme[] Themes;

        private Theme currentTheme;
        public Theme CurrentTheme
        {
            get
            {
                if (currentTheme == null && Themes != null && Themes.Length > 0)
                    currentTheme = Themes[0];
                return currentTheme;
            }
        }

        public event Action<Theme> OnThemeChanged;

        public void ChangeTheme(Theme newTheme)
        {
            if (newTheme == null || currentTheme == newTheme) return;
            currentTheme = newTheme;
            OnThemeChanged?.Invoke(currentTheme);
        }

        public void ChangeTheme(string newThemeName)
        {
            Theme found = Themes.FirstOrDefault(t => t.name == newThemeName);
            if (found != null)
                ChangeTheme(found);
            else
                Debug.LogWarning($"Theme '{newThemeName}' not found.");
        }

        public void ChangeToNextTheme()
        {
            if (Themes == null || Themes.Length == 0) return;

            bool foundNext = false;
            Theme firstTheme = Themes[0];

            foreach (var item in Themes)
            {
                if (foundNext)
                {
                    ChangeTheme(item);
                    return;
                }

                if (CurrentTheme == item)
                    foundNext = true;
            }

            ChangeTheme(firstTheme);
        }
    }
}
