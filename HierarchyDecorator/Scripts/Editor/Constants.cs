using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [InitializeOnLoad]
    public static class Constants
    {
        // ====== Settings Strings ======

        public const string PREF_GUID = "HD_GUID";

        public const string SETTINGS_PATH = "Hierarchy Decorator";

        public const string SETTINGS_ASSET_PATH = "Assets/HierarchyDecorator/Settings.asset";
        public const string SETTINGS_ASSET_FOLDER = "Assets/HierarchyDecorator/";

        // ====== Colours ======

        // ------ Two Tone Colours ------

        public readonly static Color DarkModeEvenColor = new Color (0.25f, 0.25f, 0.25f, 1f);
        public readonly static Color DarkModeOddColor = new Color (0.225f, 0.225f, 0.225f, 1f);

        public readonly static Color LightModeEvenColor = new Color (0.8f, 0.8f, 0.8f, 1f);
        public readonly static Color LightModeOddColor = new Color (0.765f, 0.765f, 0.765f, 1f);

        // ------ Standard Colours ------

        public readonly static Color SelectionColour = new Color (58f / 255f, 178f / 255f, 178f / 255f, 1);
    
        public readonly static Color HoverColour = new Color (150f / 255f, 150f / 255f, 150f / 255f, 1);

        public readonly static Color InactiveColour = new Color (0.20f, 0.20f, 0.20f, 0.3f);

        public readonly static Color InactivePrefabColour = new Color (0.48f, 0.67f, 0.95f, 0.6f);

        public readonly static Color DarkBackgroundColour = new Color (0.219f, 0.219f, 0.219f);

        public readonly static Color LightBackgroundColour = new Color (0.8f, 0.8f, 0.8f, 1);

        public static Color DefaultBackgroundColor => EditorGUIUtility.isProSkin ? DarkBackgroundColour : LightBackgroundColour;

        // ====== Instance Data ======

        public static string[] LayerMasks => UnityEditorInternal.InternalEditorUtility.layers;
        public readonly static LayerMask AllLayers = ~0;

        //====== Editor Settings ======

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

    internal static class Style
    {
        public static readonly GUIStyle FoldoutHeader;
        public static readonly GUIStyle TabBackground;
        public static readonly GUIStyle CenteredBoldLabel;
        public static readonly GUIStyle SmallDropdown;
        public static readonly GUIStyle ComponentIconStyle;
        public static readonly GUIStyle Title;
        public static readonly GUIStyle InnerWindow;

        // Foldouts
        public static readonly GUIStyle LargeButtonStyle;
        public static readonly GUIStyle LargeButtonSmallTextStyle;

        // Hierarchy Styles

        public static readonly GUIStyle Toggle;
        public static readonly GUIStyle ToggleMixed;

        private const string TOGGLE_MIXED = "OL ToggleMixed";

        static Style()
        {
            FoldoutHeader = new GUIStyle (EditorStyles.foldout)
            {
#if UNITY_2019_1_OR_NEWER
                stretchHeight = true,
#endif
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };

            TabBackground = new GUIStyle (EditorStyles.helpBox)
            {
                stretchHeight = true,
                fixedHeight = 0,

                margin = new RectOffset (0, 0, 0, 0),

                alignment = TextAnchor.MiddleLeft
            };

            CenteredBoldLabel = new GUIStyle (EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 11,
                fixedHeight = 0,
                fontStyle = FontStyle.Bold
            };

            SmallDropdown = new GUIStyle (EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                normal =
                {
                    textColor = new Color(0.6f, 0.6f, 0.6f)
                }
            };

            ComponentIconStyle = new GUIStyle (EditorStyles.label)
            {
                padding = new RectOffset (0, 0, 0, 0),
                margin = new RectOffset (0, 0, 0, 0)
            };

            Title = new GUIStyle (EditorStyles.boldLabel)
            {
                fontSize = 18,
                fixedHeight = 21,
            };

            InnerWindow = new GUIStyle (GUI.skin.window)
            {
                padding = new RectOffset (2, 0, 0, 0),
                margin = new RectOffset (0, 0, 0, 0),
            };

            LargeButtonStyle = new GUIStyle (EditorStyles.miniButton)
            {
                fixedHeight = 32f
            };

            LargeButtonSmallTextStyle = new GUIStyle (EditorStyles.miniButton)
            {
                fixedHeight = 32f,
                fontSize = 12
            };

            // Hierarchy Styles

            Toggle = new GUIStyle ("OL Toggle");

#if UNITY_2019_4_OR_NEWER
            ToggleMixed = new GUIStyle (TOGGLE_MIXED)
            {
                focused =
                {
                    background = null,
                    scaledBackgrounds = null,
                },
            };
#else
            ToggleMixed = new GUIStyle (Toggle);
#endif

        }
    }
}