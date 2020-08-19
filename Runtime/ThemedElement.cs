using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThematicUI
{
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

        private void Awake()
        {
            colorTarget = GetComponent<Graphic>();
            fontTarget = GetComponent<TextMeshProUGUI>();
            spriteTarget = GetComponent<Image>();
        }

        private void OnEnable()
        {
            ThemeManager.OnThemeChanged += OnThemeChanged;
        }
        private void OnDisable()
        {
            ThemeManager.OnThemeChanged -= OnThemeChanged;
        }
        void OnThemeChanged(Theme newTheme)
        {
            if (changeColor && colorTarget)
                colorTarget.color = newTheme.GetColor(colorKey).Color;
            if (changeFont && fontTarget)
                fontTarget.font = newTheme.GetFont(fontKey).Font;
            if (changeSprite && spriteTarget)
                spriteTarget.sprite = newTheme.GetSprite(spriteKey).Sprite;
        }
    }
}
