using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    /// <summary>
    /// ScriptableObject containing all settings and relevant data for the hierarchy
    /// </summary>
    internal class Settings : ScriptableObject, ISerializationCallbackReceiver
    {
        // Settings
        public GlobalSettings globalSettings = new GlobalSettings ();
        public List<PrefixSettings> prefixes; //Collection of all prefixes
        public List<GUIStyle> styles = new List<GUIStyle> (); //List of all custom GUIStyles

        // Icon Data
        public List<ComponentType> unityComponents = new List<ComponentType> ();
        public List<CustomComponentType> customComponents = new List<CustomComponentType> ();

        // Collection of every component type unity has
        private static Type[] allTypes;

        // Constants
        public const string typeString = "Settings";

        // Settings Creation

        private void OnValidation()
        {
            EditorApplication.RepaintHierarchyWindow ();
        }

        /// <summary>
        /// Load the asset for settings, or create one if it doesn't already exist
        /// </summary>
        /// <returns>The loaded settings</returns>
        internal static Settings GetOrCreateSettings()
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

            Settings settings = AssetUtility.FindOrCreateScriptable<Settings> (typeString, Constants.SETTINGS_ASSET_FOLDER);
            settings.SetDefaults ();

            path = AssetDatabase.GetAssetPath (settings);
            EditorPrefs.SetString (Constants.PREF_GUID, AssetDatabase.AssetPathToGUID (path));

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
            // Create collections
            unityComponents = new List<ComponentType> ();
            customComponents = new List<CustomComponentType> ();

            // Defaults
            styles = HierarchyDecoratorHelper.ImportantStyles;
            prefixes = HierarchyDecoratorHelper.ImportantPrefixes;

            foreach (PrefixSettings prefix in prefixes)
            {
                prefix.UpdateStyle ();
            }

            EditorUtility.SetDirty (this);
            AssetDatabase.SaveAssets ();


            UpdateSettings ();
        }

        // Serialization

        public void OnBeforeSerialize()
        {
            UpdateSettings ();
        }

        public void OnAfterDeserialize()
        {
            UpdateSettings ();
        }

        public void UpdateSettings()
        {
            //Reflection for component types
            if (allTypes == null)
            {
                allTypes = ReflectionUtility.GetTypesFromAllAssemblies (typeof (Component));
            }

            // If we're missing any components, look for them
            if (unityComponents.Count != allTypes.Length)
            {
                unityComponents.Clear ();

                for (int i = 0; i < allTypes.Length; i++)
                {
                    unityComponents.Add (new ComponentType (allTypes[i]));
                }
            }
            else
            {
                for (int i = 0; i < allTypes.Length; i++)
                {
                    ComponentType component = unityComponents[i];

                    if (component.type == null)
                    {
                        component.UpdateType (allTypes[i]);
                    }
                }
            }
        }

        // GUI Styles

        /// <summary>
        /// Get a GUI style from the custom style list
        /// </summary>
        /// <param name="name">The name of the style to find</param>
        /// <returns>Returns the gui style found, or the bold label as default if one is not</returns>
        public GUIStyle GetGUIStyle(string name)
        {
            GUIStyle style = styles.FirstOrDefault (s => s.name == name);
            return style ?? new GUIStyle();
        }

        // Components

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The type to find</param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool FindCustomComponentFromType(Type type, out CustomComponentType component)
        {
            for (int i = 0; i < customComponents.Count; i ++)
            {
                CustomComponentType customComponent = customComponents[i];

                if (customComponent.script == null)
                {
                    continue;
                }

                // Not a good work around
                if (customComponent.type == type)
                {
                    component = customComponent;
                    return true;
                }
            }

            component = null;
            return false;
        }

        public void UpdateCustomComponentData()
        {
            for (int i = 0; i < customComponents.Count; i++)
            {
                customComponents[i].UpdateScriptType ();
            }
        }
    }

    public static class HierarchyDecoratorHelper
    {
        public static List<GUIStyle> ImportantStyles = new List<GUIStyle>
        {
            new GUIStyle()
            {
                name = "Header (Centered)",
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            },

            new GUIStyle()
            {
                name = "Subheader",
                fontSize = 10,
                alignment = TextAnchor.MiddleLeft
            },

            new GUIStyle()
            {
                name = "Mini Header (Centered)",
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.gray
                }
            }
        };

        public static List<PrefixSettings> ImportantPrefixes = new List<PrefixSettings> ()
        {
             new PrefixSettings(
                "=" ,
                "Header (Centered)",
                new ModeOptions(
                    new Color (0.1764706f, 0.1764706f, 0.1764706f),
                    new Color (0.6666667f, 0.6666667f, 0.6666667f)
                    ),

                new ModeOptions(
                    Color.white,
                    new Color (0.1764706f, 0.1764706f, 0.1764706f)
                    ))
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    fontAlignment = TextAnchor.MiddleCenter
                },

            new PrefixSettings(
                "-",
                "Subheader",
                new ModeOptions(
                    new Color (0.245283f, 0.245283f, 0.245283f),
                    new Color (0.7960785f, 0.7960785f, 0.7960785f)),

                new ModeOptions(
                    new Color (0.8584906f, 0.8584906f, 0.8584906f),
                    new Color (0.2352941f, 0.2352941f, 0.2352941f)
                    ))
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    fontAlignment = TextAnchor.MiddleLeft
                },

            new PrefixSettings(
                "+",
                "Mini Header (Centered)",
                new ModeOptions(
                    Color.white,
                    new Color (0.38568f, 0.6335747f, 0.764151f)
                    ),

                new ModeOptions(
                    Color.white,
                    new Color (0.2671325f, 0.4473481f, 0.6509434f)
                    ))
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    fontAlignment = TextAnchor.MiddleCenter
                }
        };
    }
}