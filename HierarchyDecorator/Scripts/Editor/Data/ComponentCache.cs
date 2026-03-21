using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [Serializable]
    public class ComponentCache
    {
        [SerializeField] private List<ComponentType> components = new();

        private readonly Dictionary<string, ComponentType> idLookup = new();
        private readonly Dictionary<Type, ComponentType> typeLookup = new();

        public ComponentCache() { }

        public ComponentCache(IEnumerable<ComponentType> components)
        {
            Register(components);
        }

        // - Cache Updating 

        public void Collect()
        {
            // Clear just in case this is called multiple times

            idLookup.Clear();
            typeLookup.Clear();

            foreach (ComponentType component in components)
            {
                UpdateCache(component);
            }
        }

        private void UpdateCache(ComponentType component)
        {
            // TODO: Error Handling

            if (component == null)
            {
                return;
            }

            if (Contains(component.GUID))
            {
                return;
            }

            component.UpdateType(true);

            if (!component.IsValid())
            {
                return;
            }

            idLookup.Add(component.GUID, component);
            typeLookup.Add(component.Type, component);
        }
      
        // - Registry

        public void Register(IEnumerable<ComponentType> components)
        {
            // TODO: Error Handling

            if (components == null)
            {
                return;
            }

            foreach (ComponentType component in components)
            {
                Register(component);
            }
        }

        public void Register(ComponentType component)
        {
            // TODO: Error Handling

            if (component == null)
            {
                return;
            }

            if (Contains(component.GUID))
            {
                return;
            }

            components.Add(component);
            idLookup.Add(component.GUID, component);
            typeLookup.Add(component.Type, component);
        }

        public void Deregister(IEnumerable<ComponentType> components)
        {
            // TODO: Error Handling

            if (components == null)
            {
                return;
            }

            foreach (ComponentType component in components)
            {
                Deregister(component);
            }
        }

        public void Deregister(ComponentType component)
        {
            // TODO: Error Handling

            if (component == null)
            {
                return;
            }

            if (!Contains(component.GUID))
            {
                return;
            }

            idLookup.Remove(component.GUID);
            typeLookup.Remove(component.Type);
            components.Remove(component);
        }

        // - Queries

        public bool TryGet(string id, out ComponentType component)
        {
            return idLookup.TryGetValue(id, out component);
        }

        public bool TryGet(Type type, out ComponentType component)
        {
            return typeLookup.TryGetValue(type, out component);
        }

        public int IndexOf(string id)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GUID.Equals(id))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Contains(string id)
        {
            return idLookup.ContainsKey(id);
        }

        public bool Contains(Type type)
        {
            return typeLookup.ContainsKey(type);
        }
    }

    public static class ComponentContentCache
    {
        private static readonly Dictionary<Type, GUIContent> s_cache = new Dictionary<Type, GUIContent>();
        private static readonly GUIContent s_none = new GUIContent(GUIContent.none); // Modification safety

        public static GUIContent GetIcon(Type type) 
        {
            if (type == null)
            {
                return s_none;
            }

            if (!s_cache.TryGetValue(type, out GUIContent content))
            {
                GUIContent objectContent = EditorGUIUtility.ObjectContent(null, type);
                content = new GUIContent(type.Name, objectContent.image, type.Name);
                s_cache[type] = content;
            }

            return content;
        }
    }
}