using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ThematicUI
{
    [CreateAssetMenu(fileName = "New Theme Settings", menuName = "ThematicUI/Theme Settings")]
    public class ThemeSettings : ScriptableObject
    {
        static ThemeSettings _instance = null;
        public static ThemeSettings Instance
        {
            get
            {
                if (!_instance)
                    _instance = Resources.FindObjectsOfTypeAll<ThemeSettings>().FirstOrDefault();
                return _instance;
            }
        }
        public string[] Colors;
        public string[] Fonts;
        public string[] Sprites;

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
