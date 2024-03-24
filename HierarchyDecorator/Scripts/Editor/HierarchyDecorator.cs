using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.PackageManager;

namespace HierarchyDecorator
{
    [InitializeOnLoad]
    internal static class HierarchyDecorator
    {
        public const string SETTINGS_TYPE_STRING = "Settings";
        public const string SETTINGS_NAME_STRING = "Settings";

        private static Settings Settings;
        private static SceneCache Hierarchy;

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
    
        static HierarchyDecorator()
        {
            EditorApplication.delayCall = () =>
            {
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItem;
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItem;
                
                EditorSceneManager.sceneOpened -= AddScene;
                EditorSceneManager.sceneOpened += AddScene;
                EditorSceneManager.sceneClosed -= RemoveScene;
                EditorSceneManager.sceneClosed += RemoveScene;

                // Need to manually register initial scenes
                
                int count = SceneManager.sceneCount;
                for (int i = 0; i < count; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    AddScene(scene, OpenSceneMode.Single);
                }
            };

            // Handle package updating

        }

        private static void AddScene(Scene scene, OpenSceneMode mode)
        {
            HierarchyCache.RegisterScene(scene);
            //Debug.Log("Added scene " + scene.name + " to Hierarchy.");
        }

        private static void RemoveScene(Scene scene)
        {
            HierarchyCache.RemoveScene(scene);
            //Debug.Log("Removed scene " + scene.name + " from Hierarchy.");
        }

        private static void OnHierarchyItem(int instanceID, Rect selectionRect)
        {
            if (EditorApplication.isUpdating)
            {
                return;
            }

            if (Settings == null)
            {
                Settings = GetOrCreateSettings ();
                UpdateComponentData();
                return;
            }

            HierarchyManager.OnGUI(instanceID, selectionRect);
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

            Settings settings = AssetUtility.FindOrCreateScriptable<Settings> (SETTINGS_TYPE_STRING, SETTINGS_NAME_STRING, Constants.SETTINGS_ASSET_FOLDER);
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

        public static void UpdateComponentData()
        {
            Settings.Components.UpdateData();
            Settings.Components.UpdateComponents(true);
        }
    }
}
