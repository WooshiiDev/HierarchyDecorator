using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [Serializable]
    /// <summary>
    /// Collection of components for icons
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentGroup : IGroup<string>, IGuid<string>
    {
        // Fields

        [SerializeField] protected string name;
        [SerializeField] protected string guid;
        [SerializeField] protected List<string> components = new List<string>();

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
        public string[] Components => components.ToArray();

        /// <summary>
        /// The number of components in this group.
        /// </summary>
        public int Count => components.Count;

        public string GUID => guid;

        // Constructor

        public ComponentGroup(string name)
        {
            this.guid = CreateGUID();
            this.name = name;
        }

        private string CreateGUID()
        {
            return Guid.NewGuid().ToString();
        }

        // Methods

        public bool Add(string id)
        {
            if (Contains(id))
            {
                return false;
            }

            components.Add(id);
            return true;
        }

        public bool Remove(string id)
        {
            int index = IndexOf(id);
            if (index == -1)
            {
                return false;
            }

            components.RemoveAt(index);
            return true;
        }

        public string Get(int index)
        {
            if (index < 0 || index >= components.Count)
            {
                // TODO: Error Handling
                return string.Empty;
            }

            return components[index];
        }

        public bool Contains(string id)
        {
            return IndexOf(id) != -1;
        }

        public int IndexOf(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return -1;
            }

            return components.IndexOf(id);
        }
    }
}