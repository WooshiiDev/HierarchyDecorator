using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{


    [InitializeOnLoad]
    public static class Constants
    {
        public static class Paths
        {
            /// <summary>
            /// Pref path used to cache the GUID of the settings asset.
            /// </summary>
            public static string PREF_GUID = Application.productName + "_HD_GUID";

            /// <summary>
            /// Path used for showing Hierarchy Decorator settings in Preferences. See <see cref="HierarchyDecoratorProvider"/>.
            /// </summary>
            public const string SETTINGS_PATH = "Hierarchy Decorator";

            /// <summary>
            /// Default creation path for the <see cref="Settings"/> asset.
            /// </summary>
            public const string DEFAULT_ASSET_PATH = "Assets/HierarchyDecorator/Settings.asset";

            /// <summary>
            /// The default folder path when creating a <see cref="Settings"/> asset.
            /// </summary>
            public const string DEFAULT_ASSET_FOLDER = "Assets/HierarchyDecorator/";
        }
       

        // --- Colours

        // - Two Tone Colours

        public readonly static Color DarkModeEvenColor = new Color (0.25f, 0.25f, 0.25f, 1f);
        public readonly static Color DarkModeOddColor = new Color (0.225f, 0.225f, 0.225f, 1f);

        public readonly static Color LightModeEvenColor = new Color (0.8f, 0.8f, 0.8f, 1f);
        public readonly static Color LightModeOddColor = new Color (0.765f, 0.765f, 0.765f, 1f);

        // - Standard Colours

        public readonly static Color SelectionColour = new Color (58f / 255f, 178f / 255f, 178f / 255f, 1);
    
        public readonly static Color HoverColour = new Color (150f / 255f, 150f / 255f, 150f / 255f, 1);

        public readonly static Color InactiveColour = new Color (0.20f, 0.20f, 0.20f, 0.35f);

        public readonly static Color InactivePrefabColour = new Color (0.48f, 0.67f, 0.95f, 0.6f);

        public readonly static Color DarkBackgroundColour = new Color (0.219f, 0.219f, 0.219f);

        public readonly static Color LightBackgroundColour = new Color (0.8f, 0.8f, 0.8f, 1);

        public static Color DefaultBackgroundColor => EditorGUIUtility.isProSkin ? DarkBackgroundColour : LightBackgroundColour;

        // --- Instance Data

        public static string[] LayerMasks => UnityEditorInternal.InternalEditorUtility.layers;
        public readonly static LayerMask AllLayers = ~0;

        // --- Editor Settings

        public readonly static CategoryFilter DefaultFilter = new CategoryFilter("Other", string.Empty, FilterType.NONE);

        public readonly static CategoryFilter[] ComponentFilters =
        {
            new CategoryFilter ("2D", "2D", FilterType.NAME),

            // Animation
            new CategoryFilter ("Animation", "Anim", FilterType.NAME),
            new CategoryFilter ("Animation", "Constraint", FilterType.NAME),

            // Audio
            new CategoryFilter ("Audio", "Audio", FilterType.NAME),

            // Mesh
            new CategoryFilter ("Mesh", "Renderer", FilterType.NAME),
            new CategoryFilter ("Mesh", "Mesh", FilterType.NAME),

            // Physics
            new CategoryFilter ("Physics", "Collider", FilterType.NAME),
            new CategoryFilter ("Physics", "Joint", FilterType.NAME),
            new CategoryFilter ("Physics", "Rigidbody", FilterType.NAME),

            // Networking
            new CategoryFilter ("Network", "Network", FilterType.NAME),

            new CategoryFilter ("UI", "Canvas", FilterType.NAME),
            new CategoryFilter ("UI", "UnityEngine.EventSystems.UIBehaviour, UnityEngine.UI", FilterType.TYPE),
            new CategoryFilter ("UI", "UnityEngine.GUIElement, UnityEngine", FilterType.TYPE)
        };
    }

    internal static class Textures
    {
        internal readonly static Texture2D CheckboxEmpty = Resources.Load<Texture2D> ("Icons/checkbox_filled");
        internal readonly static Texture2D CheckboxFilled = Resources.Load<Texture2D> ("Icons/checkbox_empty");

        internal readonly static Texture2D Checked = Resources.Load<Texture2D> ("Icons/checked");
        internal readonly static Texture2D Checkbox = Resources.Load<Texture2D> ("Icons/checkBox");

        internal readonly static Texture2D Checkmark = Resources.Load<Texture2D> ("Icons/checkmark");

        internal readonly static Texture2D Banner = Resources.Load<Texture2D> ("HierarchyDecoratorLogo");
    }
}