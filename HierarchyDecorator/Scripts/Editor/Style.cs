using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    //TODO: [Wooshii] Add comment descriptions for styles.
    internal static class Style
    {
        private const string TOGGLE_MIXED = "OL ToggleMixed";

        public static readonly GUIStyle Label;
        public static readonly GUIStyle BoldLabel;
        public static readonly GUIStyle Foldout;

        public static readonly GUIStyle CenteredBoldLabel;
        public static readonly GUIStyle CenteredLabel;
        
        public static readonly GUIStyle TinyText;
        public static readonly GUIStyle SmallDropdown;
        public static readonly GUIStyle ComponentIconStyle;

        public static readonly GUIStyle Title;
        public static readonly GUIStyle InspectorPadding;
        public static readonly GUIStyle NoPadding;

        // --- Setting Tabs

        public static readonly GUIStyle TabBackground;
        public static readonly GUIStyle BoxHeader;

        // --- Foldouts

        public static readonly GUIStyle LargeButtonStyle;
        public static readonly GUIStyle LargeButtonSmallTextStyle;

        // --- Hierarchy Styles

        public static readonly GUIStyle Toggle;
        public static readonly GUIStyle ToggleMixed;

        // --- Fields

        public static readonly GUIStyle ToolbarNoSpace;
        public static readonly GUIStyle ToolbarButtonLeft;
        public static readonly GUIStyle ToolbarButtonResizable;

        // --- Widgets

        public static readonly GUIStyle ToolbarTextField;

        static Style()
        {
            // Labels 

            Label = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.UpperLeft
            };

            BoldLabel = new GUIStyle(Label)
            {
                fontStyle = FontStyle.Bold
            };

            Foldout = new GUIStyle(EditorStyles.foldout)
            {
                alignment = TextAnchor.UpperLeft,
            };

            ToolbarNoSpace = new GUIStyle(EditorStyles.toolbar)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),

                fixedHeight = 0,
                fixedWidth = 0,
            };

            ToolbarButtonLeft = new GUIStyle("ToolbarButtonLeft")
            {
                alignment = TextAnchor.MiddleLeft,
            };

            ToolbarButtonResizable = new GUIStyle(ToolbarButtonLeft)
            {
                fixedWidth = 0,
                fixedHeight = 0,
                alignment = TextAnchor.MiddleCenter
            };

            BoxHeader = new GUIStyle(GUI.skin.box)
            {
#if UNITY_2019_1_OR_NEWER
                stretchHeight = true,
#endif
                stretchWidth = true,

                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,

                fontSize = 12,

                normal =
                {
                    textColor = EditorStyles.label.normal.textColor,
                },

                margin = new RectOffset(0,0,0,0),
                padding = new RectOffset(0,0,0,0)
            };

            TabBackground = new GUIStyle (EditorStyles.helpBox)
            {
                stretchHeight = true,
                fixedHeight = 0,

                margin = new RectOffset (0, 0, 0, 0),
                padding = new RectOffset(8, 8, 8, 8),

                alignment = TextAnchor.MiddleLeft
            };

            CenteredLabel = new GUIStyle(EditorStyles.boldLabel)
            {
                fixedHeight = 0,
                alignment = TextAnchor.MiddleCenter
            };

            CenteredBoldLabel = new GUIStyle (EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 11,
                fixedHeight = 0,
                fontStyle = FontStyle.Bold,
            };

            SmallDropdown = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                normal =
                {
                    textColor = new Color(0.6f, 0.6f, 0.6f)
                }
            };

            TinyText = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 8,
                normal =
                {
                    textColor = new Color(0.6f, 0.6f, 0.6f)
                }
            };

            ComponentIconStyle = new GUIStyle (EditorStyles.label)
            {
                padding = new RectOffset (0, 0, 0, 0),
                margin = new RectOffset (0, 0, 0, 0),

                fixedHeight = 16,
                fixedWidth = 16,

                imagePosition = ImagePosition.ImageOnly
            };

            Title = new GUIStyle (EditorStyles.boldLabel)
            {
                fontSize = 18,
                fixedHeight = 21,
            };

            InspectorPadding = new GUIStyle (EditorStyles.inspectorFullWidthMargins)
            {
                padding = new RectOffset (4, 4, 4, 4),
            };

            NoPadding = new GUIStyle(EditorStyles.inspectorFullWidthMargins)
            {

            };

            LargeButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fixedHeight = 32f,
                fixedWidth = 0,

                clipping = TextClipping.Clip,
                margin = new RectOffset(1, 1, 1, 1)
            };

            LargeButtonSmallTextStyle = new GUIStyle (EditorStyles.miniButton)
            {
                fixedHeight = 32f,
                fontSize = 12,
            };

            // Hierarchy Styles

            Toggle = new GUIStyle ("OL Toggle")
            {
                normal =
                {
                    textColor = EditorStyles.label.normal.textColor,
                },
            };

#if UNITY_2019_4_OR_NEWER
            ToggleMixed = new GUIStyle (TOGGLE_MIXED)
            {
                clipping = TextClipping.Clip,

                normal =
                {
                    textColor = EditorStyles.label.normal.textColor,
                },

                active =
                {
                    textColor = EditorStyles.label.normal.textColor,
                },

                hover =
                {
                    textColor = EditorStyles.label.normal.textColor,
                },

                focused =
                {
                    background = null,
                    scaledBackgrounds = null,

                    textColor = EditorStyles.label.normal.textColor,
                },
            };
#else
            ToggleMixed = new GUIStyle (Toggle);
#endif

#if UNITY_2022_1_OR_NEWER
            ToolbarTextField = new GUIStyle("ToolbarSearchTextField")
            {
                fixedWidth = 0,
                margin = new RectOffset(2, 1, 1, 0),

            };
#else
            ToolbarTextField = new GUIStyle("ToolbarSeachTextField")
            {
                fixedWidth = 0,
                margin = new RectOffset(2, 1, 1, 0),

            };
#endif
        }
    }
}