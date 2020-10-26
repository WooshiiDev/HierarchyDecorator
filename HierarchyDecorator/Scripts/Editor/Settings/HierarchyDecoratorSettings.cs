using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
    {
    /// <summary>
    /// ScriptableObject containing all settings and relevant data for the hierarchy
    /// </summary>
    internal class HierarchyDecoratorSettings : ScriptableObject, ISerializationCallbackReceiver
        {
        public GlobalSettings globalStyle;

        /// <summary>
        /// Collection of all prefixes used for custom hierarchy overlays
        /// </summary>
        public List<HierarchyStyle> prefixes;

        /// <summary>
        /// Collection of custom <see cref="GUIStyle"/>'s used for the hierarchy designs
        /// </summary>
        public List<GUIStyle> styles;

        private readonly List<HierarchyStyle> importantPrefixes = new List<HierarchyStyle> ()
            {
            new HierarchyStyle ("=" , "Header"),
            new HierarchyStyle ("-" , "Toolbar"),
            new HierarchyStyle ("+" ),
            };

        /// <summary>
        /// Components shown in the inspector
        /// </summary>
        public Dictionary<string, ComponentType> shownComponents = new Dictionary<string, ComponentType> ();

        public List<CustomComponentType> customTypes = new List<CustomComponentType> ();

        /// <summary>
        /// List of all components
        /// Required for easier handling of serialization
        /// </summary>
        public List<ComponentType> components;

        //Collection of every component type unity has 
        private static Type[] allTypes;

        //Constants
        public const string typeString = "HierarchyDecoratorSettings";
        
        private void OnValidate()
            {
            EditorApplication.RepaintHierarchyWindow ();
            }

        #region Creation

        /// <summary>
        /// Load the asset for settings, or create one if it doesn't already exist
        /// </summary>
        /// <returns>The loaded settings</returns>
        internal static HierarchyDecoratorSettings GetOrCreateSettings()
            {
            HierarchyDecoratorSettings settings = null;

            //Load from the saved GUID 
            if (EditorPrefs.HasKey (Constants.PREF_GUID))
                {
                string savedPath = AssetDatabase.GUIDToAssetPath (EditorPrefs.GetString (Constants.PREF_GUID));

                if (!string.IsNullOrEmpty (savedPath))
                    return settings = AssetDatabase.LoadAssetAtPath<HierarchyDecoratorSettings> (savedPath);
                }

            string[] guids = AssetDatabase.FindAssets ($"t:{typeString}");

            //Create an asset if none exist
            if (guids.Length == 0)
                {
                settings = AssetUtility.CreateScriptableAtPath<HierarchyDecoratorSettings> ("HierarchyDecoratorSettings", Constants.SETTINGS_ASSET_PATH);
                settings.SetDefaults ();

                Debug.Log ($"Hiearchy Decorator found no previous settings, creating one at {Constants.SETTINGS_ASSET_PATH}.");
                }

            string path = guids[0];
            path = AssetDatabase.GUIDToAssetPath (path);

            settings = AssetDatabase.LoadAssetAtPath<HierarchyDecoratorSettings> (path);
            EditorPrefs.SetString (Constants.PREF_GUID, AssetDatabase.AssetPathToGUID (guids[0]));

            return settings;
            }

        /// <summary>
        /// Convert into serialized object for handling GUI
        /// </summary>
        /// <returns>Serialized version of the settings</returns>
        internal static SerializedObject GetSerializedSettings()
            {
            return new SerializedObject (GetOrCreateSettings ());
            }

        /// <summary>
        /// Setup defaults for the new settings asset
        /// </summary>
        internal void SetDefaults()
            {
            //Settings
            globalStyle = new GlobalSettings ();

            //Collections
            prefixes = importantPrefixes;
            components = new List<ComponentType> ();
            styles = new List<GUIStyle> ()
                {
                CreateGUIStyle ("Header",       EditorStyles.boldLabel),
                CreateGUIStyle ("Toolbar",      EditorStyles.toolbarButton),
                CreateGUIStyle ("Grid Centered",EditorStyles.centeredGreyMiniLabel),
                };

            //Specifics for defaults
            var toolbar = prefixes[1];
            toolbar.SetAlignment (TextAnchor.MiddleLeft);
            toolbar.SetFontSize (10);
            toolbar.SetStyle (FontStyle.Normal);

            // - Grid Centered - 
            var grid = styles[2];
            grid.border = new RectOffset (15, 15, 15, 15);
            grid.stretchWidth = true;

            UpdateSettings ();
            }

        #endregion

        #region Editor Serialization

        //This is just to get psudeo-type selection working
        //Can pass them back to the dictionary when required
        public void OnBeforeSerialize() => UpdateSettings ();

        public void OnAfterDeserialize() => UpdateSettings ();

        public void UpdateSettings()
            {
            //Reflection for component types
            if (allTypes == null)
                allTypes = ReflectionUtility.GetTypesFromAllAssemblies (typeof (Component));

            bool hasMissing = components.Count != allTypes.Length;

            if (hasMissing)
                components.Clear ();

            for (int i = 0; i < allTypes.Length; i++)
                {
                var type = allTypes[i];

                if (hasMissing)
                    components.Add (new ComponentType (type));

                ComponentType component = components[i];

                if (component.type == null)
                    component.UpdateType (type);

                if (!shownComponents.ContainsKey (component.name))
                    shownComponents.Add (component.name, component);
                }
            }

        #endregion

        #region GUIStyles

        /// <summary>
        /// Get a GUI style from the custom style list
        /// </summary>
        /// <param name="name">The name of the style to find</param>
        /// <returns>Returns the gui style found, or the bold label as default if one is not</returns>
        public GUIStyle GetGUIStyle(string name)
            {
            GUIStyle style = styles.FirstOrDefault (s => s.name == name);
            return style ?? EditorStyles.boldLabel;
            }

        private GUIStyle CreateGUIStyle(string name, GUIStyle styleBase = null)
            {

            //Generally optimistic settings
            return new GUIStyle (styleBase)
                {
                name = name,

                stretchHeight = false,
                stretchWidth = false,

                fontSize = 11,
                fontStyle = FontStyle.Bold,

                fixedHeight = 0,
                fixedWidth = 0,

                alignment = TextAnchor.MiddleCenter,
                };
            }

        public string[] GetStyleNames()
            {
            string[] names = new string[styles.Count];

            for (int i = 0; i < names.Length; i++)
                names[i] = styles[i].name;

            return names;
            }
        #endregion
        }
    }
