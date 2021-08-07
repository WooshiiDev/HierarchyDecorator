using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<PrefixSettings> prefixes;
        public List<GUIStyle> styles = new List<GUIStyle> ();

        // Component Data

        public List<ComponentType> unityComponents = new List<ComponentType> ();
        public List<CustomComponentType> customComponents = new List<CustomComponentType> ();

        private static Type[] allTypes;

        // Settings Creation

        /// <summary>
        /// Setup defaults for the new settings asset
        /// </summary>
        internal void SetDefaults(bool isDarkMode)
        {
            // Create collections
            unityComponents = new List<ComponentType> ();
            customComponents = new List<CustomComponentType> ();

            // Defaults
            styles = HierarchyDecoratorHelper.ImportantStyles;
            prefixes = HierarchyDecoratorHelper.ImportantPrefixes;

            foreach (PrefixSettings prefix in prefixes)
            {
                prefix.UpdateStyle (isDarkMode);
            }

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
        public static readonly List<GUIStyle> ImportantStyles = new List<GUIStyle>
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

        public static readonly List<PrefixSettings> ImportantPrefixes = new List<PrefixSettings> ()
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