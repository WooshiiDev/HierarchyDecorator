using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [System.Serializable]
    internal class GlobalSettings
    {
        public bool showActiveToggles = true;
        public bool showComponents = true;
        public bool showAllComponents = true;

        public bool twoToneBackground = true;

        public bool showLayers = true;
        public bool editableLayers = true;
        public bool applyChildLayers = true;

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