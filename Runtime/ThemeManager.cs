using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThematicUI
{
    public class ThemeManager : MonoBehaviour
    {
        static ThemeManager instance;
        public static ThemeManager Instance { get => instance; }

        public static Action<Theme> OnThemeChanged;

        [SerializeField] ThemeAsset themeAsset;
        [SerializeField] string initialTheme;

        Dictionary<string, Theme> themes;
        HashSet<RectTransform> elementsToRebuild = new HashSet<RectTransform>();

        public Theme CurrentTheme { get; private set; }

        private void Awake()
        {
            if (!themeAsset)
            {
                Debug.LogError("ThemeAsset is missing in the ThemeManager.");
                enabled = false;
                return;
            }

            if (!instance)
                instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            LoadThemes();
        }

        void LateUpdate()
        {
            foreach (var rect in elementsToRebuild)
            {
                if (rect)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }

            elementsToRebuild.Clear();
        }

        public  void RegisterToRebuild(RectTransform rect)
        {
            if (rect && !elementsToRebuild.Contains(rect))
                elementsToRebuild.Add(rect);
        }

        void LoadThemes()
        {
            themes = new Dictionary<string, Theme>();
            var themesList = themeAsset.Themes;
            foreach (var theme in themesList)
            {
                themes.Add(theme.name, theme);
            }
        }

        private void Start()
        {
            ChangeTheme(initialTheme);
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
            if (themes.TryGetValue(newTheme, out var theme))
                ChangeTheme(theme);
            else
                Debug.LogWarning($"Theme '{newTheme}' não encontrado.");
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