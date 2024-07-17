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

    public enum BreadcrumbStyle { Solid = 0, Dash = 1, Dotted = 2 }

    public enum TagLayerLayout { TagInFront = 0, LayerInFront = 1, TagAbove = 2, LayerAbove = 3 }

    [System.Serializable]
    public class BreadcrumbSettings
    {
        public bool show = true;
        public Color color = Color.grey;
        public BreadcrumbStyle style = BreadcrumbStyle.Solid;

        public bool displayHorizontal = true;
    }

    [System.Serializable]
    public class GlobalData
    {
        // Toggles

        public bool showActiveToggles = true;

        [Tooltip("Clicking and dragging over check boxes to toggle them.")]
        public bool activeSwiping = true;

        [Tooltip ("Only toggle the instances with the same state as the first selected.")]
        public bool swipeSameState = true;

        [Tooltip ("If a selection exists, only toggle the selected instances.")]
        public bool swipeSelectionOnly = true;

        [Tooltip ("The accepted criteria for selecting instances when swiping.")]
        public DepthMode depthMode;

        // Tags & Layers

        public TagLayerLayout tagLayerLayout;
        public bool showTags = true;
        public bool showLayers = true;
        public bool applyChildLayers = true;

        // Components 

        public bool showAllComponents = true;

        // Breadcrumbs

        public bool showBreadcrumbs = true;

        public BreadcrumbSettings instanceBreadcrumbs;
        public BreadcrumbSettings fullDepthBreadcrumbs = new BreadcrumbSettings()
        {
            style = BreadcrumbStyle.Dash,
            displayHorizontal = false,
        };

    }
}