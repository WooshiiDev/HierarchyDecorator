using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HierarchyDecorator
{
    public static class HierarchyManager
    {
        // --- Scene Data

        private static Dictionary<int, HierarchyItem> lookup = new Dictionary<int, HierarchyItem>();

        // --- Methods

        public static void OnGUI(int id, Rect rect)
        {
            if (!TryGetValidInstance(id, out HierarchyItem item))
            {
                return;
            }

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
    }

    public class HierarchyItem
    {
        // --- Fields

        private GameObject instance;

        // --- Properties

        public Scene Scene => instance.scene;

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
        }
    }
}