using UnityEditor;

namespace HierarchyDecorator
{
    [InitializeOnLoad]
    internal static class HierarchyDecorator
    {
        public const string SETTINGS_TYPE_STRING = "Settings";
        public const string SETTINGS_NAME_STRING = "Settings";

        public static Settings Settings { get; private set; }

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
        private static Settings GetOrCreateSettings()
        {
            if (TryLoadSettings(out Settings settings))
            {
                return settings;
            }

            return CreateSettings();
        }

        private static bool TryLoadSettings(out Settings settings)
        {
            // Make sure the key is still valid - no assuming that settings just 'exist'
            if (EditorPrefs.HasKey(Constants.PREF_GUID))
            {
                string path = AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString(Constants.PREF_GUID));

                if (AssetDatabase.GetMainAssetTypeAtPath(path) != null)
                {
                    settings =  AssetDatabase.LoadAssetAtPath<Settings>(path);
                    return true;
                }
            }

            settings = null;
            return false;
        }

        private static Settings CreateSettings()
        {
            Settings settings = AssetUtility.FindOrCreateScriptable<Settings>(SETTINGS_TYPE_STRING, SETTINGS_NAME_STRING, Constants.SETTINGS_ASSET_FOLDER);
            settings.SetDefaults(EditorGUIUtility.isProSkin);

            string path = AssetDatabase.GetAssetPath(settings);
            EditorPrefs.SetString(Constants.PREF_GUID, AssetDatabase.AssetPathToGUID(path));

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

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
