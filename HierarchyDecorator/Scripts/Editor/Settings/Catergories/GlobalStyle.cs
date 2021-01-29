using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    [System.Serializable]
    internal class GlobalSettings
        {
        //Toggle
        public bool showActiveToggles = true;
        public bool showComponents = true;
        public bool showMonoBehaviours = true;

        //Style
        public bool twoToneBackground = true;

        //Layer Mask
        public bool showLayers = true;
        public bool editableLayers = true;
        public bool applyChildLayers = true;

        public GlobalSettings() { }

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
                    : new Color (0.765f, 0.765f, 0.765f, 1);
                }
            }
        }
    }
