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

    /// <summary>
    /// Formatting options used by <see cref="HierarchyStyle"/> to format style text
    /// </summary>
    public enum TextFormatting
    {
        ToUpper,
        ToLower,
        DontChange
    }

    [System.Serializable]
    public class HierarchyStyle
    {
        public string prefix = "<PREFIX>";
        public string name = "New Style";

        public Font font = null;

        public int fontSize = 11;
        public TextAnchor fontAlignment = TextAnchor.MiddleCenter;
        public FontStyle fontStyle = FontStyle.Bold;
        public TextFormatting textFormatting = TextFormatting.ToUpper;

        public ModeOptions[] modes;

        [HideInInspector] public GUIStyle style = new GUIStyle ();

        // Constructor

        public HierarchyStyle()
        {
            modes = new ModeOptions[]
            {
                new ModeOptions (Color.black, Color.white),
                new ModeOptions (Color.white, Color.black)
            };
        }

        public HierarchyStyle(string prefix, string name, ModeOptions lightMode, ModeOptions darkMode)
        {
            this.prefix = prefix;
            this.name = name;

            modes = new ModeOptions[]
            {
                lightMode,
                darkMode
            };
        }

        public void UpdateStyle(bool isDarkMode)
        {
            style.name = name;
            style.fontStyle = fontStyle;
            style.fontSize = fontSize;
            style.alignment = fontAlignment;

            style.font = font;
            style.normal.textColor = GetCurrentMode(isDarkMode).fontColour;
        }

        /// <summary>
        /// Returns string formatted according to styling options of <see cref="HierarchyStyle"/> instance
        /// </summary>
        /// <returns>Formatted string if formatting is required, otherwise same string</returns>
        /// <exception cref="System.Exception">In case instance has invalid parameters</exception>
        public string FormatString(string text)
        {
            switch (textFormatting)
            {
            default:
                throw new System.Exception("Unhandled formatting type " + textFormatting);

            case TextFormatting.ToUpper:
                return text.ToUpper();
                
            case TextFormatting.ToLower:
                return text.ToLower();
                
            case TextFormatting.DontChange:
                return text;
            }
        }

        public ModeOptions GetCurrentMode(bool isDarkMode)
        {
            return modes[isDarkMode ? 1 : 0];
        }
    }
}