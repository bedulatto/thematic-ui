using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ThematicUI
{
    public class ThemeManager : MonoBehaviour
    {
        static ThemeManager instance;
        public static ThemeManager Instance { get => instance; }

        public static Action<Theme> OnThemeChanged;

        public ThemeAsset ThemeAsset;
        public string InitialTheme;
        Dictionary<string, Theme> themes;
        public Theme CurrentTheme { get; private set; }

        private void Awake()
        {
            if (!instance)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            LoadThemes();
        }
        void LoadThemes()
        {
            themes = new Dictionary<string, Theme>();
            var themesList = ThemeAsset.Themes;
            foreach (var theme in themesList)
            {
                themes.Add(theme.name, theme);
            }
        }
        private void Start()
        {
            ChangeTheme(InitialTheme);
        }
        public void ChangeTheme(Theme newTheme)
        {
            if (CurrentTheme == newTheme) return;
            CurrentTheme = newTheme;
            if (OnThemeChanged != null)
                OnThemeChanged(newTheme);
        }
        public void ChangeTheme(string newTheme)
        {
            var theme = themes[newTheme];
            if (theme)
                ChangeTheme(theme);
        }
        public void ChangeToNext()
        {
            bool foundNext = false;
            Theme firstTheme = null;
            foreach (var item in themes)
            {
                if (foundNext)
                {
                    ChangeTheme(item.Value);
                    return;
                }
                if (firstTheme == null)
                    firstTheme = item.Value;
                if (CurrentTheme.name == item.Key)
                    foundNext = true;
            }
            ChangeTheme(firstTheme);
        }
    }
}