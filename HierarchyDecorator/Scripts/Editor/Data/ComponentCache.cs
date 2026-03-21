using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    // TODO: Documentation

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
            idLookup.Clear();
            typeLookup.Clear();

            for (int i = components.Count - 1; i >= 0; i--)
            {
                if (!UpdateCache(components[i])) // Remove invalid components
                {
                    components.RemoveAt(i);
                }
            }

            ComponentContentCache.PreloadCache(typeLookup.Keys);
        }

        private bool UpdateCache(ComponentType component)
        {
            if (component == null)
            {
                return false;
            }

            if (idLookup.TryGetValue(component.GUID, out _))
            {
                return false;
            }

            component.UpdateType();

            if (!component.IsValid())
            {
                return false;
            }

            idLookup[component.GUID] = component;
            typeLookup[component.Type] = component;
            return true;
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

            if (idLookup.TryGetValue(component.GUID, out _))
            {
                return;
            }

            components.Add(component);
            idLookup[component.GUID] = component;
            typeLookup[component.Type] = component;
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

            if (!idLookup.TryGetValue(component.GUID, out _))
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
        private static readonly GUIContent s_none = new GUIContent(GUIContent.none); // Mutation safety

        public static GUIContent GetIcon(Type type)
        {
            if (type == null)
            {
                return s_none;
            }

            if (!s_cache.TryGetValue(type, out GUIContent content))
            {
                content = CreateComponentContent(type);
                s_cache[type] = content;
            }

            return content;
        }

        public static void PreloadCache(IEnumerable<Type> componentTypes)
        {
            if (componentTypes == null)
            {
                throw new NullReferenceException("");
            }

            foreach (Type type in componentTypes)
            {
                if (type == null)
                {
                    continue;
                }

                s_cache[type] = CreateComponentContent(type);
            }
        }

        private static GUIContent CreateComponentContent(Type type)
        {
            if (type == null)
            {
                return s_none;
            }

            return new GUIContent(type.Name, GetBuiltInContent(type).image, type.Name);
        }

        private static GUIContent GetBuiltInContent(Type type)
        {
            return EditorGUIUtility.ObjectContent(null, type);
        }
    }
}