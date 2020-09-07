using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    public enum LineStyle
        {
        NONE    = 0,
        TOP     = 1,
        BOTTOM  = 2,
        BOTH    = 3
        }

    [System.Serializable]
    public class StyleSettings
        {
        [Header ("Font")]
        public Font font = null;
        public int fontSize = 11;
        public TextAnchor fontAlignment = TextAnchor.MiddleCenter;
        public FontStyle fontStyle = FontStyle.Bold;

        [Header ("Colors")]
        public Color fontColor = Color.black;
        public Color backgroundColor = Color.white;

        [Header ("Line")]
        public LineStyle displayedLine = LineStyle.NONE;
        public float lineHeight = 1f;
        public Color lineColor = Color.black;
        }

    [System.Serializable]
    public class HierarchyStyle
        {
        public string prefix;
        public string guiStyle = "Header";

        public StyleSettings lightMode = new StyleSettings();
        public StyleSettings darkMode = new StyleSettings();

        public HierarchyStyle(string prefix, string name = "Header")
            {
            this.prefix = prefix;
            this.guiStyle = name;
            }

        public StyleSettings GetCurrentSettings()
            {
            return (EditorGUIUtility.isProSkin) ? darkMode : lightMode;
            }

        public void SetAlignment(TextAnchor anchor)
            {
            lightMode.fontAlignment = darkMode.fontAlignment = anchor;
            }

        public void SetFontSize(int size)
            {
            lightMode.fontSize = darkMode.fontSize = size;
            }

        public void SetStyle(FontStyle style)
            {
            lightMode.fontStyle = darkMode.fontStyle = style;
            }

        public void SetLineStyle(LineStyle lineStyle)
            {
            lightMode.displayedLine = darkMode.displayedLine = lineStyle;
            }
        }
    }
