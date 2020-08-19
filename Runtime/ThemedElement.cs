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
            if (!initialized && ThemeManager.Instance != null && ThemeManager.Instance.CurrentTheme != null)
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

            if (changeColor && colorTarget)
                colorTarget.color = newTheme.GetColor(colorKey).Color;

            if (changeFont && fontTarget)
                fontTarget.font = newTheme.GetFont(fontKey).Font;

            if (changeSprite && spriteTarget)
            {
                spriteTarget.sprite = newTheme.GetSprite(spriteKey).Sprite;
                if (!changeColor)
                    spriteTarget.color = Color.white;
            }

            if (ForceLayoutRebuild)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            initialized = true;

            if (AfterThemeChanged != null)
                AfterThemeChanged.Invoke(newTheme);
        }
    }
}
