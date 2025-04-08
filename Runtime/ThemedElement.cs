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
        [SerializeField] bool changeColor;
        [SerializeField] string colorKey;
        [SerializeField] Graphic colorTarget;

        [SerializeField] bool changeFont;
        [SerializeField] string fontKey;
        [SerializeField] TextMeshProUGUI fontTarget;

        [SerializeField] bool changeSprite;
        [SerializeField] string spriteKey;
        [SerializeField] Image spriteTarget;

        [SerializeField] bool forceLayoutRebuild;

        [SerializeField] ThemeEvent beforeThemeChanged;
        [SerializeField] ThemeEvent afterThemeChanged;

        [SerializeField] RectTransform rect;

        bool initialized;
        Theme currentTheme;

        private void Awake()
        {
            if (!colorTarget && changeColor)
                colorTarget = GetComponent<Graphic>();

            if (!fontTarget && changeFont)
                fontTarget = GetComponent<TextMeshProUGUI>();

            if (!spriteTarget && changeSprite)
                spriteTarget = GetComponent<Image>();

            if (!rect)
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
            if (beforeThemeChanged != null)
                beforeThemeChanged.Invoke(newTheme);

            currentTheme = newTheme;
            UpdateUI();

            initialized = true;

            if (afterThemeChanged != null)
                afterThemeChanged.Invoke(currentTheme);
        }

        public void UpdateUI()
        {
            if (changeColor && colorTarget)
                colorTarget.color = currentTheme.GetColor(colorKey).Color;

            if (changeFont && fontTarget)
                fontTarget.font = currentTheme.GetFont(fontKey).Font;

            if (changeSprite && spriteTarget)
                spriteTarget.sprite = currentTheme.GetSprite(spriteKey).Sprite;

            if (forceLayoutRebuild && rect)
                ThemeManager.Instance.RegisterToRebuild(rect);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || !enabled)
                return;

            if (!colorTarget && changeColor)
                colorTarget = GetComponent<Graphic>();

            if (!fontTarget && changeFont)
                fontTarget = GetComponent<TextMeshProUGUI>();

            if (!spriteTarget && changeSprite)
                spriteTarget = GetComponent<Image>();

            if (!rect)
                rect = GetComponent<RectTransform>();

            if (ThemeManager.Instance?.CurrentTheme != null)
                ChangeTheme(ThemeManager.Instance.CurrentTheme);
        }
#endif
    }
}
