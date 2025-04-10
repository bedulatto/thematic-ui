using UnityEngine;

namespace ThematicUI
{
    public class ThemedAudioSource : ThemedElementBase
    {
        [ThemeKey(typeof(AudioClipKey))]
        [SerializeField] private string audioKey;
        [SerializeField] private AudioSource target;

        protected override void ApplyTheme(Theme theme)
        {
            var audioValue = theme.Get<AudioClipKey>(audioKey);
            if (audioValue != null && target != null)
                target.clip = audioValue.Value;
        }
    }
}