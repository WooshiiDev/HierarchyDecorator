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
                Previous = Current;
                Previous.BeforeNextGUI(item);
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
                item = new HierarchyItem(instance);
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

        public HierarchyItem(GameObject instance)
        {
            this.instance = instance;
        }

        // --- Methods

        public void OnGUI(Rect rect)
        {
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
            if (Transform.parent == null)
            {
                return instance.scene.rootCount - 1 == index;
            }

            return Transform.parent.childCount - 1 == index;
        }
    }
}