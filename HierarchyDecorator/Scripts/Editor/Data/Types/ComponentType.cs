using System;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [Serializable]
    public class ComponentType : IComparable<ComponentType>
    {
        [HideInInspector]
        public string name;

        public bool shown = false;
        public Type type;

        public ComponentType(Type type)
        {
            this.type = type;
            this.name = type.Name;
        }

        public void UpdateType(Type type)
        {
            this.type = type;
            this.name = type.Name;
        }

        public int CompareTo(ComponentType other)
        {
            if (other == null)
            {
                return 1;
            }

            return name.CompareTo (other.name);
        }
    }

    [Serializable]
    public class CustomComponentType : ComponentType
    {
        public MonoScript script = null;

        public CustomComponentType(Type type) : base (type)
        {
        }

        public void UpdateScriptType()
        {
            if (script == null)
            {
                return;
            }

            this.type = script.GetClass ();
            this.name = script.name;
        }
    }
}