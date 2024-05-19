using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace HierarchyDecorator
{
    public enum PrefabInfo { None, Root, Part }

    public static class HierarchyManager
    {
        // --- Scene Data

        private static Dictionary<int, HierarchyItem> lookup = new Dictionary<int, HierarchyItem>();
        public static int Count => lookup.Count;
        public static IReadOnlyDictionary<int, HierarchyItem> Items => lookup;

        public static HierarchyItem Current { get; private set; }
        public static HierarchyItem Previous { get; private set; }

        // --- Methods

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            EditorApplication.delayCall -= SetupCallbacks;
            EditorApplication.delayCall += SetupCallbacks;
        }

        private static void SetupCallbacks()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;

            EditorSceneManager.sceneOpened += OnSceneOpen;
            EditorSceneManager.sceneClosed += OnSceneClose;

            EditorApplication.hierarchyChanged += OnHierarchyChange;
        }

        private static void OnHierarchyChange()
        {
            if (lookup.Count < 100)
            {
                return;
            }

            ResetLookup();
        }

        private static void OnSceneOpen(Scene scene, OpenSceneMode mode)
        {
            ResetLookup();
        }

        private static void OnSceneClose(Scene scene)
        {
            ResetLookup();
        }

        private static void ResetLookup()
        {
            lookup.Clear();
            Current = null;
        }

        // - GUI

        public static void OnGUI(int id, Rect rect)
        {
            if (EditorApplication.isUpdating)
            {
                return;
            }

            if (!TryGetValidInstance(id, out HierarchyItem item))
            {
                return;
            }

            // Need to check IsValid() as the last GO in hierarchy doees not call OnHierarchyChange() in time
            
            if (Current != null && Current.IsValid())
            {
                Current.BeforeNextGUI(item);
                Previous = Current;
            }

            Current = item;

            item.OnGUI(rect);
            HierarchyInfo.ResetIndent();
        }

        private static bool TryGetValidInstance(int id, out HierarchyItem item)
        {
            GameObject instance = EditorUtility.InstanceIDToObject(id) as GameObject;

            if (instance == null)
            {
                item = null;
                return false;
            }

            item = GetNext(id, instance);
            return true;
        }

        private static HierarchyItem GetNext(int id, GameObject instance)
        {
            if (!lookup.TryGetValue(id, out HierarchyItem item))
            {
                item = new HierarchyItem(id, instance);
                lookup.Add(id, item);
            }

            return item;
        }

        public static bool IsPreviousParent()
        {
            if (Previous == null)
            {
                return false;
            }

            return Current.Transform.parent == Previous.Transform;
        }
    }

    public class ComponentList
    {
        private List<ComponentItem> items = new List<ComponentItem>();

        public ComponentList(GameObject instance)
        {
            UpdateCache(GetComponents(instance));
        }

        public void Validate(GameObject instance)
        {
            UpdateCache(GetComponents(instance));
        }

        private void UpdateCache(Component[] components)
        {
            List<ComponentItem> nextItems = new List<ComponentItem>();

            for (int i = 0; i < components.Length; i++)
            {
                ComponentItem item = Get(components[i]);
                if (item == null)
                {
                    item = new ComponentItem(components[i]);
                }
                
                nextItems.Add(item);
            }
            items = nextItems;
        }

        private Component[] GetComponents(GameObject instance)
        {
            return instance.GetComponents<Component>();
        }

        private ComponentItem Get(Component component)
        {
            return items.Find(c => c.Component == component);
        }

        public IEnumerable<ComponentItem> GetItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                yield return items[i];
            }
        }
    }
}