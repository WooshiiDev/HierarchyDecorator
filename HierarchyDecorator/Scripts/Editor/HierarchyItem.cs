using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HierarchyDecorator
{
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
        public ComponentList Components { get; private set; }

        public GameObject GameObject => instance;

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

        public string DisplayName => instance.name;

        public HierarchyItem(int id, GameObject instance)
        {
            this.instance = instance;
            this.Components = new ComponentList(instance);

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
            Components.Validate(instance);

#if UNITY_2019_1_OR_NEWER
            rect.height = 16f;
#endif

            // Draw GUI

            foreach (HierarchyDrawer info in Drawers)
            {
                info.Draw(rect, this, s_settings);
            }

            foreach (HierarchyInfo info in Info)
            {
                info.Draw(rect, this, s_settings);
            }

            foreach (HierarchyDrawer info in OverlayDrawers)
            {
                info.Draw(rect, this, s_settings);
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
}