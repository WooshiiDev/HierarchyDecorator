using UnityEditor;

namespace HierarchyDecorator
{
    [InitializeOnLoad]
    internal static class HierarchyDecorator
    {
        public const string SETTINGS_TYPE_STRING = "Settings";
        public const string SETTINGS_NAME_STRING = "Settings";

        private static Settings Settings;
    
        static HierarchyDecorator()
        {
            Initialize();

            EditorApplication.update -= ValidateSettings;
            EditorApplication.update += ValidateSettings;
        }

        // Setup 

        private static void Initialize()
        {
            ValidateSettings();
            HierarchyManager.SetupCallbacks();
        }

        private static void ValidateSettings()
        {
            if (Settings != null)
            {
                return;
            }

            Settings = GetOrCreateSettings();
            UpdateComponentData();
        }

        private static void UpdateComponentData()
        {
            Settings.Components.UpdateData();
            Settings.Components.UpdateComponents(true);
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

            Settings = settings;
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
    }
}
