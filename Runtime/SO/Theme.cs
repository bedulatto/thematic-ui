using System.Collections.Generic;
using UnityEngine;

namespace ThematicUI
{
    [System.Serializable]
    public class Theme : ScriptableObject
    {
        public ThemeAsset ThemeAsset;

        [SerializeReference]
        public List<ThemeKey> Keys = new();

        public T Get<T>(string name) where T : ThemeKey
        {
            return Keys.Find(k => k.Name == name && k is T) as T;
        }
    }
}
