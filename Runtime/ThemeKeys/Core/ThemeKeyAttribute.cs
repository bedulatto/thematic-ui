using System;
using UnityEngine;

namespace ThematicUI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ThemeKeyAttribute : PropertyAttribute
    {
        public Type KeyType { get; }

        public ThemeKeyAttribute(Type keyType)
        {
            if (!typeof(ThemeKey).IsAssignableFrom(keyType))
                throw new ArgumentException("Provided type must inherit from ThemeKey.");

            KeyType = keyType;
        }
    }
}
