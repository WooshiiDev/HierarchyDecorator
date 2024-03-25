using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace HierarchyDecorator
{
    public static class HierarchyManager
    {
        // --- Scene Data

        private static Dictionary<int, HierarchyItem> lookup = new Dictionary<int, HierarchyItem>();
        public static int Count => lookup.Count;
        public static IReadOnlyDictionary<int, HierarchyItem> Items => lookup;

        public static HierarchyItem Current { get; private set; }
        public static HierarchyItem Previous { get; private set; }

        // --- Methods

        public static void Initialize()
        {
            EditorSceneManager.sceneOpened += OnSceneOpen;
            EditorSceneManager.sceneClosed += OnSceneClose;

            EditorApplication.hierarchyChanged += OnHierarchyChange;
        }

        private static void OnHierarchyChange()
        {
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
            if (!TryGetValidInstance(id, out HierarchyItem item))
            {
                return;
            }

            if (Current != null)
            {
                // Need to check due to last game object in hierarchy not calling OnHierarchyChange() if deleted

                if (Current.IsValid())
                {
                    Current.BeforeNextGUI(item);
                    Previous = Current;
                }
                else
                {
                    lookup.Remove(Current.ID);
                }
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

    public enum PrefabInfo { None, Root, Part }

    public class HierarchyItem
    {
        // Drawers 

        private static HierarchyDrawer[] Drawers = new HierarchyDrawer[]
        {
            new StyleDrawer(),
        };

        private static HierarchyDrawer[] OverlayDrawers = new HierarchyDrawer[]
        {
            new StateDrawer(),
            new ToggleDrawer(),
            new BreadcrumbsDrawer()
        };

        private static HierarchyInfo[] Info = new HierarchyInfo[]
        {
            new TagLayerInfo(),
            new ComponentIconInfo()
        };

        private static Settings s_settings = HierarchyDecorator.GetOrCreateSettings();

        // --- Fields

        
        private GameObject instance;

        // --- Properties

        public int ID { get; private set; }
        public Components Components { get; private set; }

        /// <summary>
        /// This items transform.
        /// </summary>
        public Transform Transform => instance.transform;
        
        /// <summary>
        /// The scene this item belongs in.
        /// </summary>
        public Scene Scene => instance.scene;

        /// <summary>
        /// Does this item have any children?
        /// </summary>
        public bool HasChildren => instance.transform.childCount > 0;

        /// <summary>
        /// Is this showing child items in hierarchy?
        /// </summary>
        public bool Foldout { get; private set; }

        /// <summary>
        /// Does this item have a parent or is it a root?
        /// </summary>
        public bool HasParent => Transform.parent != null;

        public PrefabInfo PrefabInfo { get; private set; }
        public bool IsPrefab => PrefabInfo != PrefabInfo.None;
        
        public HierarchyItem(int id, GameObject instance)
        {
            this.instance = instance;
            this.Components = new Components(instance);

            ID = id;
            PrefabInfo = GetPrefabInfo();
        }

        // --- Methods

        private PrefabInfo GetPrefabInfo()
        {
            if (!PrefabUtility.IsPartOfAnyPrefab(instance))
            {
                return PrefabInfo.None;
            }

            return PrefabUtility.GetNearestPrefabInstanceRoot(instance) == instance
                ? PrefabInfo.Root
                : PrefabInfo.Part;
        }

        public void OnGUI(Rect rect)
        {
            if (Components.Validate(instance))
            {

            }

#if UNITY_2019_1_OR_NEWER
            rect.height = 16f;
#endif

            // Draw GUI

            foreach (HierarchyDrawer info in Drawers)
            {
                info.Draw(rect, instance, s_settings);
            }

            foreach (HierarchyInfo info in Info)
            {
                info.Draw(rect, instance, s_settings);
            }

            foreach (HierarchyDrawer info in OverlayDrawers)
            {
                info.Draw(rect, instance, s_settings);
            }
        }

        public void BeforeNextGUI(HierarchyItem next)
        {
            Foldout = next.Transform.parent == Transform;
        }

        public int CalculateDepth()
        {
            Transform parent = Transform.parent;
            int index = 0;
            while (parent != null)
            {
                parent = parent.parent;
                index++;
            }

            return index;
        }

        public bool IsLastSibling()
        {
            int index = Transform.GetSiblingIndex();
            if (!HasParent)
            {
                return instance.scene.rootCount - 1 == index;
            }

            return Transform.parent.childCount - 1 == index;
        }

        public bool IsValid()
        {
            return instance != null;
        }
    }

    public class Components
    {
        public List<ComponentItem> Items { get; private set; } = new List<ComponentItem>();

        public Components(GameObject instance)
        {
            UpdateCache(GetComponents(instance));
        }

        public bool Validate(GameObject instance)
        {
            Component[] list = GetComponents(instance);

            bool isValid = Items.Count == list.Length;
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (!Items[i].IsValid())
                {
                    isValid = false;
                    Items.RemoveAt(i);
                }
            }

            if (!isValid)
            {
                UpdateCache(list);
            }

            return isValid;
        }

        private void UpdateCache(Component[] components)
        {

            foreach (Component component in components)
            {
                if (Items.Any(c => c.Component == component))
                {
                    continue;
                }

                Items.Add(new ComponentItem(component));
            }
        }

        private Component[] GetComponents(GameObject instance)
        {
            return instance.GetComponents<Component>();
        }
    }

    public class ComponentItem
    {
        public Component Component { get; private set; }
        public GUIContent Content { get; private set; }

        public ComponentItem(Component component)
        {
            Component = component;

            if (HierarchyDecorator.GetOrCreateSettings().Components.TryGetComponent(component.GetType(), out ComponentType type))
            {
                Content = type.Content;
            }
        }

        public bool IsValid()
        {
            return Component != null;
        }
    }
}