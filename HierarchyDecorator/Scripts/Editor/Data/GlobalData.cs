using UnityEngine;

namespace HierarchyDecorator
{
    public enum DepthMode
    {
        All = 0,
        SameDepth = 1,
        SameDepthOrLower = 2,
        SameDepthOrHigher = 3
    }

    [System.Serializable]
    public class GlobalData
    {
        // General 

        public bool showComponentIcons = true;
        public bool twoToneBackground = true;

        // Toggles

        public bool showActiveToggles = true;

        [Tooltip("Clicking and dragging over checkboxes to toggle them.")]
        public bool activeSwiping = true;

        [Tooltip ("Only toggle the instances with the same state as the first selected.")]
        public bool swipeSameState = true;

        [Tooltip ("If a selection exists, only toggle the selected instances.")]
        public bool swipeSelectionOnly = true;

        [Tooltip ("How to handke ")]
        public DepthMode depthMode;

        // Layers

        public bool showLayers = true;
        public bool editableLayers = true;
        public bool applyChildLayers = true;

        // Components 

        public bool showAllComponents = true;
    }
}