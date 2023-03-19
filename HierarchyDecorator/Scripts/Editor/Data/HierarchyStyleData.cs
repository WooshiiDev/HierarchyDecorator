using System.Collections.Generic;
using UnityEngine;

namespace HierarchyDecorator
{
    [System.Serializable]
    public class ColorSetting
    {
        [SerializeField] private Color colorOne;
        [SerializeField] private Color colorTwo;

        public ColorSetting(Color a, Color b)
        {
            colorOne = a;
            colorTwo = b;
        }

        public Color GetColor(Rect rect)
        {
            int y = Mathf.RoundToInt(rect.y);
            
            if (y % 32 == 0)
            {
                return colorOne;
            }

            return colorTwo;
        }
    }

    [System.Serializable]
    public class HierarchyStyleData
    {
        public bool displayLayers = true;
        public bool displayIcons = true;

        public List<HierarchyStyle> styles = new List<HierarchyStyle> ()
        {
             new HierarchyStyle(
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

            new HierarchyStyle(
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

            new HierarchyStyle(
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

        public int Count => styles.Count;

        // Background
        
        public bool twoToneBackground = true;
        public ColorSetting lightMode = new ColorSetting(new Color(0.8f, 0.8f, 0.8f, 1f), new Color(0.765f, 0.765f, 0.765f, 1f));
        public ColorSetting darkMode = new ColorSetting(new Color(0.245f, 0.245f, 0.245f, 1f), new Color(0.225f, 0.225f, 0.225f, 1f));

        // --- Methods

        // Styles

        public void UpdateStyles(bool isDarkMode)
        {
            foreach (HierarchyStyle style in styles)
            {
                style.UpdateStyle (isDarkMode);
            }
        }

        public bool TryGetStyleFromPrefix(string prefix, out HierarchyStyle style)
        {
            if (Count == 0)
            {
                style = null;
                return false;
            }

            if (prefix.Contains (" "))
            {
                prefix = prefix.TrimStart ().Split (' ')[0];
            }

            for (int i = 0; i < styles.Count; i++)
            {
                style = styles[i];

                if (prefix.Equals (style.prefix))
                {
                    return true;
                }
            }

            style = null;
            return false;
        }

        public bool HasStyle(string prefix)
        {
            if (prefix.Contains (" "))
            {
                prefix = prefix.TrimStart ().Split (' ')[0];
            }

            for (int i = 0; i < styles.Count; i++)
            {
                if (styles[i].prefix == prefix)
                {
                    return true;
                }
            }

            return false;
        }

        // Color Mode

        public ColorSetting GetColorMode(bool isPro)
        {
            if (isPro)
            {
                return darkMode;
            }

            return lightMode;
        }

        public HierarchyStyle this[int i]
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }

                return styles[i];
            }
        }
    }
}
