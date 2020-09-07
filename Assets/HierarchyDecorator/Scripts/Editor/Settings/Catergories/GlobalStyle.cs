using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal class GlobalStyle
        {
        public bool twoToneBackground;

        public Color GetTwoToneColour(Rect selectionRect)
            {
            bool hasRemainder = selectionRect.y % 32 != 0;

            if (EditorGUIUtility.isProSkin)
                {
                return hasRemainder
                    ? new Color (0.25f, 0.25f, 0.25f, 1)
                    : new Color (0.225f, 0.225f, 0.225f, 1);
                }
            else
                {
                return hasRemainder
                    ? new Color (0.8f, 0.8f, 0.8f, 1)
                    : new Color (0.75f, 0.75f, 0.75f, 1);
                }
            }

        public void OnDraw()
            {
            HierarchyDecoratorGUI.ToggleAuto (ref twoToneBackground, "Show Two Tone Background");

            }
        }
    }
