using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [Serializable]
    public class ComponentType : IComparable<ComponentType>
    {
        // Fields

        // --- Component information

        [SerializeField] protected string displayName;
        [SerializeField] protected string name;
        [SerializeField] protected GUIContent content;

        // --- Settings

        [SerializeField] protected bool shown = false;
        [SerializeField] protected bool excluded = false;

        //  --- Type Data

        [SerializeField] private bool isBuiltIn;
        [SerializeField] private MonoScript script;

        [SerializeField] private bool hasToggle;

        // Properties

        /// <summary>
        /// The full name of the component
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
        /// The GUIContent displayed for this component.
        /// </summary>
        public GUIContent Content => content;

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
            this.isBuiltIn = isBuiltIn;
            UpdateType(type, false);
        }
         
        // Methods

        /// <summary>
        /// Evaluate and check if this is a valid representation of a component.
        /// </summary>
        /// <returns>Returns true if valid, otherwise returns false.</returns>
        public bool IsValid()
        {
            // Need to check type and content to validate GUI

            if (!IsBuiltIn && script == null)
            {
                return false;
            }

            if (content == null)
            {
                return false;
            }

            return Type != null && content.image != null;
        }

        /// <summary>
        /// Update the component
        /// </summary>
        /// <param name="type">The type to assign.</param>
        /// <param name="updateContent">Update the GUI.</param>
        public void UpdateType(Type type, bool updateContent = false)
        {
            if (type == null)
            {
                Type = null;
                name = "Undefined";

                UpdateContent();
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

            if (updateContent)
            {
                UpdateContent();
            }
        }
        
        /// <summary>
         /// Update the component.
         /// </summary>
         /// <param name="monoScript">The script to assign.</param>
        public void UpdateType(MonoScript monoScript)
        {
            script = monoScript;
            
            if (monoScript == null)
            {
                UpdateType(null, true);
                return;
            }

            UpdateType(monoScript.GetClass(), true);
        }

        /// <summary>
        /// Update the GUIContent cached.
        /// </summary>
        public void UpdateContent()
        {
            content = GetTypeContent();
        }

        /// <summary>
        /// Get the GUIContent icon for for component.
        /// </summary>
        /// <returns>Returns the content object.</returns>
        private GUIContent GetTypeContent()
        {
            if (Type == null)
            {
                return new GUIContent(GUIContent.none);
            }

            GUIContent content = new GUIContent(displayName, displayName);
            Texture texture;
            if (isBuiltIn)
            {
                texture = EditorGUIUtility.ObjectContent(null, Type).image;
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(script);
                texture = AssetDatabase.GetCachedIcon(path);
            }

            content.image = texture;

            return content;
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