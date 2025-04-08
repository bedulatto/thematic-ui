using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ThematicUI
{
    [System.Serializable]
    public class ThemeEvent : UnityEvent<Theme> { }
    public class ThemedElement : MonoBehaviour
    {
        public bool changeColor;
        public string colorKey;
        Graphic colorTarget;

        public bool changeFont;
        public string fontKey;
        TextMeshProUGUI fontTarget;

        public bool changeSprite;
        public string spriteKey;
        Image spriteTarget;

        public bool ForceLayoutRebuild;

        public ThemeEvent BeforeThemeChanged;
        public ThemeEvent AfterThemeChanged;

        Theme currentTheme;

        RectTransform rect;
        bool initialized;

        private void Awake()
        {
            colorTarget = GetComponent<Graphic>();
            fontTarget = GetComponent<TextMeshProUGUI>();
            spriteTarget = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ThemeManager.OnThemeChanged += ChangeTheme;

            if (!initialized && ThemeManager.Instance?.CurrentTheme != null)
                ChangeTheme(ThemeManager.Instance.CurrentTheme);
        }

        private void OnDisable()
        {
            ThemeManager.OnThemeChanged -= ChangeTheme; 
            initialized = false;
        }

        void ChangeTheme(Theme newTheme)
        {
            if (BeforeThemeChanged != null)
                BeforeThemeChanged.Invoke(newTheme);

            currentTheme = newTheme;
            UpdateUI();

            initialized = true;

            if (AfterThemeChanged != null)
                AfterThemeChanged.Invoke(currentTheme);
        }

        public void UpdateUI()
        {
            if (changeColor && colorTarget)
                colorTarget.color = currentTheme.GetColor(colorKey).Color;

            if (changeFont && fontTarget)
                fontTarget.font = currentTheme.GetFont(fontKey).Font;

            if (changeSprite && spriteTarget)
                spriteTarget.sprite = currentTheme.GetSprite(spriteKey).Sprite;
            if (ForceLayoutRebuild)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}
