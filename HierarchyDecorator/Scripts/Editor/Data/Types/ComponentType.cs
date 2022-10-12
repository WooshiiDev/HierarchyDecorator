using System;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [Serializable]
    public class ComponentType : IComparable<ComponentType>
    {
        // Fields

        // --- Component information

        [SerializeField] protected string name;
        [SerializeField] protected GUIContent content;

        // --- Settings

        [SerializeField] protected bool shown = false;

        //  --- Type Data

        [SerializeField] private bool isBuiltIn;
        [SerializeField] private MonoScript script;

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
        /// The type of component.
        /// </summary>
        public Type Type { get; private set; } = typeof(DefaultAsset);

        /// <summary>
        /// 
        /// </summary>
        public MonoScript Script
        {
            get
            {
                return script;
            }

            set
            {
                script = value;
            }
        }

        /// <summary>
        /// Is the component activated or not
        /// </summary>
        public bool Shown
        {
            get
            {
                return shown;
            }

            set
            {
                shown = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBuiltIn
        {
            get
            {
                return isBuiltIn;
            }
        }

        /// <summary>
        /// The GUIContent displayed for this component.
        /// </summary>
        public GUIContent Content
        {
            get
            {
                return content;
            }
        }

        // Constructor 

        /// <summary>
        /// ComponentType constructor.
        /// </summary>
        /// <param name="type">The type of component.</param>
        public ComponentType(Type type, bool isBuiltIn)
        {
            Type = type;
            name = type.Name;

            this.isBuiltIn = isBuiltIn;
        }
         
        // Methods

        public bool IsValid()
        {
            // Need to check type and content to validate GUI
            return Type != null && !string.IsNullOrEmpty(Content.text);
        }

        /// <summary>
        /// Update the component
        /// </summary>
        /// <param name="type"></param>
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
            name = type.Name;

            if (updateContent)
            {
                UpdateContent();
            }
        }

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
        /// 
        /// </summary>
        public void UpdateContent()
        {
            content = GetTypeContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private GUIContent GetTypeContent()
        {
            if (Type == null)
            {
                return new GUIContent(GUIContent.none);
            }

            if (isBuiltIn)
            {
                GUIContent c = new GUIContent(EditorGUIUtility.ObjectContent(null, Type));

                c.text = name;
                c.tooltip = name;

                return c;
            }

            string path = AssetDatabase.GetAssetPath(script);
            Texture tex = AssetDatabase.GetCachedIcon(path);

            GUIContent content = new GUIContent(name, tex, name);
            content.text = name;
            content.tooltip = name;

            return content;
        }

        // --- Overrides

        /// <summary>
        /// Compare this component type to another.
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

        public override string ToString()
        {
            return $"Component Type: {name}, {Type}";
        }

    }
}