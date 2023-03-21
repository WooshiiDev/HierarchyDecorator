using System;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public struct IconInfo : IComparable<IconInfo>
    {
        public readonly string Name;
        public readonly Type Type;
        public readonly GUIContent Content;
        public readonly SerializedProperty Property;

        public IconInfo(ComponentType type, SerializedProperty property)
        {
            Name = type.Name;
            Type = type.Type;
            Content = type.Content;
            Property = property;
        }

        public int CompareTo(IconInfo other)
        {
            return Name.CompareTo(Name);
        }
    }

}
