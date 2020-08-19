using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ThematicUI
{
    public class ThemeManager : MonoBehaviour
    {
        [SerializeField] Theme _initialTheme;
        Dictionary<string, Theme> _themes;
        static ThemeManager _instance;
        public static ThemeManager Instance { get => _instance; }
        public Theme CurrentTheme { get; private set; }
        public static Action<Theme> OnThemeChanged;
        private void Awake()
        {
            if (!_instance)
                _instance = this;
            else if (_instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            LoadThemes();
        }
        void LoadThemes()
        {
            _themes = new Dictionary<string, Theme>();
            var themes = Resources.FindObjectsOfTypeAll<Theme>();
            foreach (var theme in themes)
            {
                _themes.Add(theme.name, theme);
            }
        }
        private void Start()
        {
            ChangeTheme(_initialTheme);
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
            var theme = _themes[newTheme];
            if (theme)
                ChangeTheme(theme);
        }
        public void ChangeToNext()
        {
            bool foundNext = false;
            Theme firstTheme = null;
            foreach (var item in _themes)
            {
                if (foundNext)
                {
                    Debug.Log("found");
                    ChangeTheme(item.Value);
                    return;
                }
                if (firstTheme == null)
                    firstTheme = item.Value;
                if (CurrentTheme.name == item.Key)
                    foundNext = true;
            }
            Debug.Log(firstTheme.name);
            ChangeTheme(firstTheme);
        }
    }
}