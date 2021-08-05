using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace HierarchyDecorator
{
    [InitializeOnLoad]
    internal static class HierarchyDecorator
    {
        private static Settings Settings;

        private static List<HierarchyDrawer> Drawers = new List<HierarchyDrawer> ()
        {
            new StyleDrawer(),
            new ToggleDrawer(),
        };

        private static HierarchyInfo[] Info = new HierarchyInfo[]    
        {
            new LayerInfo(),
            new ComponentIconInfo()
        };


        static HierarchyDecorator()
        {
            Settings = Settings.GetOrCreateSettings ();
            Settings.UpdateSettings ();
            Settings.UpdateCustomComponentData ();

            if (Settings == null)
            {
                Debug.LogError ("Cannot initialize HierarchyDecorator because settings do not exist!");
                return;
            }

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItem;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItem;
        }

        private static void OnHierarchyItem(int instanceID, Rect selectionRect)
        {
            if (Settings == null)
            {
                Settings = Settings.GetOrCreateSettings ();
                return;
            }

            GameObject instance = EditorUtility.InstanceIDToObject (instanceID) as GameObject;

            // Skip over the instance 
            // - normally if it's a Scene instance rather than a GameObject
            if (instance == null)
            {
                return;
            }

            // Draw GUI

            for (int i = 0; i < Drawers.Count; i++)
            {
                Drawers[i].Draw (selectionRect, instance, Settings);
            }

            for (int i = 0; i < Info.Length; i++)
            {
                Info[i].Draw (selectionRect, instance, Settings);
            }

            HierarchyInfo.ResetIndent ();
        }

        public static void RegisterDrawer(HierarchyDrawer drawer)
        {
            if (Drawers.Contains(drawer))
            {
                Debug.LogError (string.Format("Drawer of {0} already exists!", drawer.GetType().Name));
                return;
            }

            Drawers.Add (drawer);
        }
    }
}
