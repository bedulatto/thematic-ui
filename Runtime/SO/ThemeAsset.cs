using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
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

#if UNITY_EDITOR
        public void SyncAllThemesWithReferences()
        {
            if (Themes == null) return;

            string assetPath = AssetDatabase.GetAssetPath(this);
            string directory = Path.GetDirectoryName(assetPath);

            foreach (var theme in Themes)
            {
                if (theme == null) continue;

                // Remove chaves obsoletas
                theme.Keys.RemoveAll(k => !KeyReferences.Any(r => r.Name == k.Name && r.Type == k.FieldType));

                // Adiciona chaves novas
                foreach (var refKey in KeyReferences)
                {
                    if (!theme.Keys.Any(k => k.Name == refKey.Name && k.FieldType == refKey.Type))
                    {
                        ThemeKey newKey = refKey.Type switch
                        {
                            ThemeFieldType.Color => new ColorKey(),
                            ThemeFieldType.Font => new FontKey(),
                            ThemeFieldType.Sprite => new SpriteKey(),
                            _ => null
                        };

                        if (newKey != null)
                        {
                            newKey.Name = refKey.Name;
                            newKey.FieldType = refKey.Type;
                            theme.Keys.Add(newKey);
                        }
                    }
                }

                EditorUtility.SetDirty(theme);
            }
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
