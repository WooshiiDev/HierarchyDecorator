using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace HierarchyDecorator
{
    [Serializable]
    /// <summary>
    /// Collection of components for icons
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentGroup : IGroup<ComponentType>
    {
        // Fields

        [SerializeField] protected string name;
        [SerializeField] protected List<ComponentType> components = new List<ComponentType>();
       
        private Dictionary<Type, ComponentType> lookup;
        private bool hasCached = false;

        // Properties

        /// <summary>
        /// The name of the group
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
        /// The components stored in this group. 
        /// </summary>
        public ComponentType[] Components
        {
            get
            {
                return components.ToArray();
            }
        }

        /// <summary>
        /// The number of components in this group.
        /// </summary>
        public int Count
        {
            get
            {
                return components.Count;
            }
        }

        // Constructor

        public ComponentGroup(string name)
        {
            this.name = name;
            ValidateCache();
        }

        // Methods

        /// <summary>
        /// Add an empty component type. Useful when needing a dummy or creating custom types.
        /// </summary>
        /// <param name="builtIn">Is the component built in? Most likely will not be touched.</param>
        /// <returns>Returns the created component for any use thereafter.</returns>
        public ComponentType AddEmpty(bool builtIn = false)
        {
            ComponentType component = new ComponentType(typeof(MonoScript), builtIn);
            Add(component);

            return component;
        }

        /// <summary>
        /// Add an element to the group.
        /// </summary>
        /// <param name="component">The component to add to the group.</param>
        /// <returns>Returns a bool based on if the instance was added to the group successfully.</returns>
        public bool Add(ComponentType component)
        {
            if (component == null)
            {
                Debug.LogError("Attempted to add null component to a group.");
                return false;
            }

            components.Add(component);

            if (component.Type != null)
            {
                ValidateCache();
                lookup.Add(component.Type, component);
            }

            return true;
        }

        /// <summary>
        /// Remove an element from the group.
        /// </summary>
        /// <param name="component">The component to remove.</param>
        /// <returns>Returns true if a component was removed, otherwise will return false.</returns>
        public bool Remove(ComponentType component)
        {
            // Cannot remove a null component, even if null components exist, it's not explicit enough.

            if (component == null)
            {
                Debug.LogError("Attempted to remove null component from a group.");
                return false;
            }

            if (!components.Remove(component))
            {
                Debug.LogError("Cannot remove a component that does not belong to the group.");
                return false;
            }

            if (component.Type != null)
            {
                lookup.Remove(component.Type);
            }

            return true;
        }

        /// <summary>
        /// Remove an element from the group.
        /// </summary>
        /// <param name="index">The index of the component.</param>
        /// <returns>Returns true if a component was removed, otherwise will return false.</returns>
        public bool Remove(int index)
        {
            if (index < 0 || index >= Count)
            {
                Debug.LogError("Given index to remove component is out of range.");
                return false;
            }

            return Remove(components[index]);
        }

        public void Clear()
        {
            components.Clear();
            lookup.Clear();

            hasCached = false;
        }

        public void Update(ComponentType component, MonoScript script)
        {
            if (component == null || script == null)
            {
                return;
            }

            // Remove old component cached

            if (Contains(component))
            {
                lookup.Remove(component.Type);
            }

            // Update type and recache 

            component.UpdateType(script);

            if (!Contains(component.Type))
            {
                lookup.Add(component.Type, component);
            }
        }

        /// <summary>
        /// Get a component element in the group.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>Returns the found element in the group. WIll return null if out of range.</returns>
        public ComponentType Get(int index)
        {
            if (index < 0 || index >= components.Count)
            {
                Debug.LogError("Index is out of range of group size.");
                return null;
            }

            return components[index];
        }

        /// <summary>
        /// Sort the group.
        /// </summary>
        public void Sort()
        {
            components.Sort();
        }

        /// <summary>
        /// Set show on all components.
        /// </summary>
        /// <param name="shown">Show value on all components.</param>
        public void SetAllShown(bool shown)
        {
            for (int i = 0; i < Count; i++)
            {
                components[i].Shown = shown;
            }
        }

        private void ValidateCache()
        {
            if (lookup == null) // Required due to lack of serialization
            {
                lookup = new Dictionary<Type, ComponentType>();
            }
            else
            {
                lookup.Clear();
            }
        }

        public void UpdateCache(bool updateContent = true)
        {
            ValidateCache();

            for (int i = 0; i < components.Count; i++)
            {
                ComponentType component = components[i];

                if (component == null) // Remove invalid 
                {
                    Remove(i);
                    i--;
                    continue;
                }
                else if (component.IsValid()) // Ignore valid
                {
                    continue;
                }

                // Update components if valid, return false if invalid

                bool isValid = component.IsBuiltIn
                    ? UpdateBuiltIn(component, updateContent)
                    : UpdateCustom(component, updateContent);

                if (component == null || component.Type == null)
                {
                    Remove(i);
                    i--;
                    continue;
                }

                isValid &= !lookup.ContainsKey(component.Type);
                isValid &= component.IsValid();

                if (isValid)
                {
                    lookup.Add(component.Type, component);
                }
            }

            hasCached = true;
        }

        private bool UpdateCustom(ComponentType component, bool updateContent)
        {
            MonoScript script = component.Script;

            if (script == null)
            {
                return false;
            }

            component.UpdateType(script.GetClass(), updateContent);
            return true;
        }

        private bool UpdateBuiltIn(ComponentType component, bool updateContent)
        {
            Type type = Type.GetType(component.Name);

            if (type == null)
            {
                return false;
            }

            component.UpdateType(type, updateContent);
            return true;
        }

        // Queries

        /// <summary>
        /// Attempt to look for a component that's been added to this group.
        /// </summary>
        /// <param name="type">The type of component to look for.</param>
        /// <param name="component">The found component if any are valid. Will return null if no component is found.</param>
        /// <returns>Returns a bool based on if a valid component is found or not.</returns>
        public bool TryGetComponent(Type type, out ComponentType component)
        {
            if (hasCached && lookup.TryGetValue(type, out component))
            {
                return true;
            }

            for (int i = 0; i < Count; i++)
            {
                ComponentType componentType = components[i];

                if (componentType.Type == type && componentType.IsValid())
                {
                    component = componentType;
                    return true;
                }
            }

            component = null;
            return false;
        }

        /// <summary>
        /// Check if a component is in this group.
        /// </summary>
        /// <param name="component">The component to check.</param>
        /// <returns>Returns true if the given component is found in the group otherwise it will return false.</returns>
        public bool Contains(ComponentType component)
        {
            if (hasCached && lookup.ContainsKey(component.Type))
            {
                return true;
            }

            return components.Contains(component);
        }

        public bool Contains(Type type)
        {
            if (hasCached && lookup.ContainsKey(type))
            {
                return true;
            }

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Type == type)
                {
                    return true;
                }
            }

            return false;
        }

        // Indexer

        /// <summary>
        /// Access the elements of the group. Calls <see cref="Get(int)" to find the element./>
        /// </summary>
        /// <param name="i">The index of the component.</param>
        /// <returns>Returns the found element in the group. WIll return null if out of range.</returns>
        public ComponentType this[int i]
        {
            get
            {
                return Get(i);
            }
        }
    }
}