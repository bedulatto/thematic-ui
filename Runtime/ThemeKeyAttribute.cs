using UnityEngine;

namespace ThematicUI
{
    public class ThemeKeyAttribute : PropertyAttribute
    {
        public string KeyType { get; }

        public ThemeKeyAttribute(string keyType)
        {
            KeyType = keyType;
        }
    }
}
