using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThematicUI
{
    [CreateAssetMenu(fileName = "Theme Asset", menuName = "ThematicUI/Theme Asset")]
    public class ThemeAsset : ScriptableObject
    {
        public string[] Colors;
        public string[] Fonts;
        public string[] Sprites;

        public Theme[] Themes;

        public int GetColorFieldIndex(string name)
        {
            for (int i = 0; i < Colors.Length; i++)
            {
                if (name == Colors[i])
                    return i;
            }
            return -1;
        }
        public int GetFontFieldIndex(string name)
        {
            for (int i = 0; i < Fonts.Length; i++)
            {
                if (name == Fonts[i])
                    return i;
            }
            return -1;
        }
        public int GetSpriteFieldIndex(string name)
        {
            for (int i = 0; i < Sprites.Length; i++)
            {
                if (name == Sprites[i])
                    return i;
            }
            return -1;
        }
    }
}