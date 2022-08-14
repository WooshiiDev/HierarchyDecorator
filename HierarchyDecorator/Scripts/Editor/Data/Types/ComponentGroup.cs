using UnityEngine;
using System;
using System.Collections.Generic;

namespace HierarchyDecorator
{
    [Serializable]
    /// <summary>
    /// Collection of components for icons
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentGroup 
    {
        // Fields

        [SerializeField] protected string name;
        [SerializeField] protected List<ComponentType> components = new List<ComponentType>();

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
        }

        // Methods

        /// <summary>
        /// Add an element to the group.
        /// </summary>
        /// <param name="component">The component to add to the group.</param>
        public void Add(ComponentType component)
        {
            components.Add(component);
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
                //throw new IndexOutOfRangeException();
                return null;
            }

            return components[index];
        }

        public void Sort()
        {
            components.Sort();
        }

        // Queries

        public bool TryGetComponent(Type type, out ComponentType foundComponent)
        {
            for (int i = 0; i < Count; i++)
            {
                ComponentType component = components[i];

                if (component.Type == type)
                {
                    foundComponent = component;
                    return true;
                }
            }

            foundComponent = null;
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