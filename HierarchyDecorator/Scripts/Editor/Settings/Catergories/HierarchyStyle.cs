using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [System.Serializable]
    public class ModeOptions
    {
        public ModeOptions(Color fontColour, Color backgroundColour)
        {
            this.fontColour = fontColour;
            this.backgroundColour = backgroundColour;
        }

        public Color fontColour = Color.black;
        public Color backgroundColour = Color.white;
    }

    [System.Serializable]
    public class PrefixSettings
    {
        public string prefix = "!!";
        public string name = "New Prefix";

        [Space(19f)]

        public Font font = null;

        public int fontSize = 11;
        public TextAnchor fontAlignment = TextAnchor.MiddleCenter;
        public FontStyle fontStyle = FontStyle.Bold;

        public ModeOptions[] modes = new ModeOptions[2];
        public ModeOptions CurrentMode => EditorGUIUtility.isProSkin ? modes[1] : modes[0];

        [HideInInspector] public GUIStyle style = new GUIStyle ();

        // Constructor

        public PrefixSettings()
        {
            modes[0] = new ModeOptions (Color.black, Color.white);
            modes[1] = new ModeOptions (Color.white, Color.black);
        }

        public PrefixSettings(string prefix, string name, ModeOptions lightMode, ModeOptions darkMode)
        {
            this.prefix = prefix;
            this.name = name;

            modes[0] = lightMode;
            modes[1] = darkMode;
        }

        public void UpdateStyle()
        {
            style.fontStyle = fontStyle;
            style.fontSize = fontSize;
            style.alignment = fontAlignment;

            style.font = font;
            style.normal.textColor = CurrentMode.fontColour;
        }
    }
}