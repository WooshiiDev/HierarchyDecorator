using System;
using UnityEditor;

namespace HierarchyDecorator
    {
    [System.Serializable]
    internal class ComponentType : IComparable<ComponentType>
        {
        //Off by default to stop a huge spam of every component
        public bool shown = false;
        public string name;
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
                return 1;

            return name.CompareTo (other.name);
            }
        }

    [System.Serializable]
    internal class CustomComponentType : ComponentType
        {
        public MonoScript script;

        public CustomComponentType(Type type) : base (type)
            {

            }

        public void UpdateScriptType()
            {
            this.type = script.GetType ();
            this.name = script.name;
            }
        }
    }
