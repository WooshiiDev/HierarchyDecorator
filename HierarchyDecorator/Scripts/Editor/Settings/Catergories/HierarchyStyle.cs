using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    [System.Serializable]
    public class ModeOptions
        {
        [Header ("Font")]
        public Font font = null;
        public int fontSize = 11;
        public TextAnchor fontAlignment = TextAnchor.MiddleCenter;
        public FontStyle fontStyle = FontStyle.Bold;

        [Header ("Colors")]
        public Color fontColor = Color.black;
        public Color backgroundColor = Color.white;

        [Header ("Others")]
        public bool hasOutline;
        public Color outlineColor = Color.black;
        }

    [System.Serializable]
    public class PrefixSettings
        {
        public string name = "New Prefix";
        public string prefix;
        public string guiStyle = "Header";

        public ModeOptions lightMode = new ModeOptions();
        public ModeOptions darkMode = new ModeOptions();

        public PrefixSettings(string prefix, string name = "Header")
            {
            this.prefix = prefix;
            this.guiStyle = name;
            }

        public ModeOptions GetCurrentSettings()
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

        public void SetOutline(bool hasOutline)
            {
            lightMode.hasOutline = darkMode.hasOutline = hasOutline;
            }
        }
    }
