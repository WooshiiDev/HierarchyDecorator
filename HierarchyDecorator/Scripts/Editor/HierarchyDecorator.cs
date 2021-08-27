using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [InitializeOnLoad]
    internal static class HierarchyDecorator
    {
        public const string SETTINGS_TYPE_STRING = "Settings";

        private static Settings Settings;

        // Drawers 

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
            Settings = GetOrCreateSettings ();

            if (Settings == null)
            {
                Debug.LogError ("Cannot initialize HierarchyDecorator because settings do not exist!");
                return;
            }

            Settings.componentData.UpdateData (true);

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItem;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItem;
        }

        private static void OnHierarchyItem(int instanceID, Rect selectionRect)
        {
            if (Settings == null)
            {
                Settings = GetOrCreateSettings ();
                return;
            }

            // Skip over the instance 
            // - normally if it's a Scene instance rather than a GameObject
            GameObject instance = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
            
            if (instance == null)
            {
                return;
            }

#if UNITY_2019_1_OR_NEWER
            selectionRect.height = 16f;
#endif

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

        // Factory Methods

        /// <summary>
        /// Load the asset for settings, or create one if it doesn't already exist
        /// </summary>
        /// <returns>The loaded settings</returns>
        public static Settings GetOrCreateSettings()
        {
            string path = null;

            // Make sure the key is still valid - no assuming that settings just 'exist'
            if (EditorPrefs.HasKey (Constants.PREF_GUID))
            {
                path = AssetDatabase.GUIDToAssetPath (EditorPrefs.GetString (Constants.PREF_GUID));

                if (AssetDatabase.GetMainAssetTypeAtPath (path) != null)
                {
                    return AssetDatabase.LoadAssetAtPath<Settings> (path);
                }
            }

            Settings settings = AssetUtility.FindOrCreateScriptable<Settings> (SETTINGS_TYPE_STRING, Constants.SETTINGS_ASSET_FOLDER);
            settings.SetDefaults (EditorGUIUtility.isProSkin);

            path = AssetDatabase.GetAssetPath (settings);
            EditorPrefs.SetString (Constants.PREF_GUID, AssetDatabase.AssetPathToGUID (path));

            EditorUtility.SetDirty (settings);
            AssetDatabase.SaveAssets ();

            return settings;
        }

        /// <summary>
        /// Convert into serialized object for handling GUI
        /// </summary>
        /// <returns>Serialized version of the settings</returns>
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject (GetOrCreateSettings ());
        }

        // Drawers

        public static void RegisterDrawer(HierarchyDrawer drawer)
        {
            if (Drawers.Contains (drawer))
            {
                Debug.LogError (string.Format ("Drawer of {0} already exists!", drawer.GetType ().Name));
                return;
            }

            Drawers.Add (drawer);
        }
    }
}
