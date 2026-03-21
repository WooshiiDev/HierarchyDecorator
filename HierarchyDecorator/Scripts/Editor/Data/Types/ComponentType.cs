using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [Serializable]
    public class ComponentType : IComparable<ComponentType>, IGuid<string>
    {
        // - Component information

        [SerializeField] protected string guid;
        [SerializeField] protected string displayName;
        [SerializeField] protected string name;

        // - Settings

        [SerializeField] protected bool shown = false;
        [SerializeField] protected bool excluded = false;

        //  - Type Data

        [SerializeField] private bool isBuiltIn;
        [SerializeField] private MonoScript script;
        [SerializeField] private bool hasToggle;

        // Properties

        /// <summary>
        /// The GUID for this component.
        /// </summary>
        public string GUID => guid;

        /// <summary>
        /// The full name of the component.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// The display name for the component.
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// The type of component.
        /// </summary>
        public Type Type { get; private set; } = typeof(DefaultAsset);

        /// <summary>
        /// The script that represents this component.
        /// </summary>
        public MonoScript Script
        {
            get => script;
            set => script = value;
        }

        /// <summary>
        /// The GUIContent representing this component.
        /// </summary>
        public GUIContent Content => ComponentContentCache.GetIcon(Type);

        /// <summary>
        /// Is the component activated or not.
        /// </summary>
        public bool Shown
        {
            get => shown;
            set => shown = value;
        }

        /// <summary>
        /// Is this component completely excluded.
        /// </summary>
        public bool Excluded 
        { 
            get => excluded; 
            set => excluded = value; 
        }

        /// <summary>
        /// Represents whether the component is a Unity component or not.
        /// </summary>
        public bool IsBuiltIn => isBuiltIn;

        /// <summary>
        /// Can this component be toggled on/off or not.
        /// </summary>
        public bool HasToggle => hasToggle;

        /// <summary>
        /// The reflected toggle property.
        /// </summary>
        public PropertyInfo ToggleProperty { get; private set; }

        // Constructor 

        /// <summary>
        /// ComponentType constructor.
        /// </summary>
        /// <param name="type">The type of component.</param>
        public ComponentType(Type type, bool isBuiltIn)
        {
            this.guid = CreateGUID();
            this.isBuiltIn = isBuiltIn;
            UpdateType(type);
        }

        private string CreateGUID()
        {
            return Guid.NewGuid().ToString();
        }

        // Methods

        /// <summary>
        /// Evaluate and check if this is a valid representation of a component.
        /// </summary>
        /// <returns>Returns true if valid, otherwise returns false.</returns>
        public bool IsValid()
        {
            if (!IsBuiltIn && script == null)
            {
                return false;
            }

            return Type != null;
        }

        /// <summary>
        /// Update the component
        /// </summary>
        /// <param name="type">The type to assign.</param>
        /// <param name="updateContent">Update the GUI.</param>
        public void UpdateType()
        {
            UpdateType(Type.GetType(Name));
        }

        private void UpdateType(Type type)
        {
            if (type == null)
            {
                Type = null;
                name = "Undefined";
                return;
            }
            
            if (!isBuiltIn && script == null)
            {
                return;
            }

            Type = type;

            name = type.AssemblyQualifiedName;
            displayName = type.Name;

            hasToggle = ReflectionUtility.HasProperty(type, "enabled");
            if (hasToggle)
            {
                ToggleProperty = ReflectionUtility.GetProperty(Type, "enabled");
            }
        }
        
        /// <summary>
         /// Update the component.
         /// </summary>
         /// <param name="monoScript">The script to assign.</param>
        public void UpdateScriptType(MonoScript monoScript)
        {
            script = monoScript;
            
            if (monoScript == null)
            {
                UpdateType(null);
                return;
            }

            UpdateType(monoScript.GetClass());
        }

        // --- Overrides

        /// <summary>
        /// Compare this component to another.
        /// </summary>
        /// <param name="other">The other component type.</param>
        /// <returns>Returns an integer based on their sort position.</returns>
        public int CompareTo(ComponentType other)
        {
            if (other == null)
            {
                return 1;
            }

            return name.CompareTo (other.name);
        }

        /// <summary>
        /// Compare this component to another type.
        /// </summary>
        /// <param name="type">The type to compare to.</param>
        /// <returns>Returns an integer based on their sort position.</returns>
        public int CompareTo(Type type)
        {
            return name.CompareTo(type.AssemblyQualifiedName);
        }

        public override string ToString()
        {
            return $"Component Type: {displayName}, {Type}";
        }

        public override bool Equals(object obj)
        {
            if (obj is ComponentType component)
            {
                return Type == component.Type;
            }

            if (obj is Type type)
            {
                return Type == type;
            }

            return false;
        }


        public override int GetHashCode()
        {
            int hashCode = 1280150957;
            hashCode *= -1521134295 + name.GetHashCode();
            hashCode *= -1521134295 + displayName.GetHashCode();
            hashCode *= -1521134295 + shown.GetHashCode();
            hashCode *= -1521134295 + isBuiltIn.GetHashCode();
            hashCode *= -1521134295 + script.GetHashCode();
            return hashCode;
        }
    }
}