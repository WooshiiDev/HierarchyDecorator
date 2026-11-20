using System.Collections.Generic;
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

    public enum IndentColorMode { Fill = 0, Underline = 1, LeftBar = 2 }

    [System.Serializable]
    public class BreadcrumbSettings
    {
        public bool show = true;
        public Color color = Color.grey;
        public BreadcrumbStyle style = BreadcrumbStyle.Solid;

        public bool displayHorizontal = true;
    }

    [System.Serializable]
    public class IndentColorSettings
    {
        public bool showIndentationColors = false;
        public IndentColorMode mode = IndentColorMode.Fill;

        [Tooltip ("The hue used for depth 0.")]
        [Range (0f, 1f)] public float hueStart = 0.58f;

        [Tooltip ("The amount of hue added per depth level.")]
        [Range (0f, 1f)] public float hueStep = 0.11f;

        [Range (0f, 1f)] public float saturation = 0.65f;
        [Range (0f, 1f)] public float value = 1f;
        [Range (0f, 1f)] public float alpha = 0.18f;

        [Tooltip ("Wrap the hue after this many depth levels. 0 applies the hue step without wrapping.")]
        [Min (0)] public int cycleLength = 0;

        [Tooltip ("Depths higher than this use the color of the maximum depth. Also limits the hierarchy walk performed per row.")]
        [Min (1)] public int maxDepth = 16;
    }

    [System.Serializable]
    public abstract class TagLayerSettingsBase
    {
        public bool show = true;
        public TagLayerColorSettings colorSettings;
    }

    [System.Serializable]
    public class TagSettings : TagLayerSettingsBase
    {
        public bool hideUntagged;
        public TagSettings()
        {
            colorSettings = new TagLayerColorSettings { solidColor = new Color32(143, 239, 255, 255) };
        }
    }

    [System.Serializable]
    public class LayerSettings : TagLayerSettingsBase
    {
        public bool applyChildLayers = true;
        public LayerSettings()
        {
            colorSettings = new TagLayerColorSettings { solidColor = new Color32(193, 255, 172, 255) };
        }
    }
    [System.Serializable]
    public class TagLayerColorSettings
    {
        public bool useSolidColor;
        public bool useRandomColor;
        public Color solidColor = Color.grey;
        [Range(0, 1)] public float hue;
        [Range(0, 1)] public float saturation = 0.5f;
        [Range(0, 2)] public float brightness = 2f;
        public static Dictionary<string, Color> s_labelToHashedColorCache;
    }

    [System.Serializable]
    public class GlobalData
    {
        // Toggles

        public bool showActiveToggles = true;
        public enum ToggleType {Checkbox, Dot}
        public ToggleType activeToggleType = ToggleType.Checkbox;

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
        public TagSettings tagSettings;
        public LayerSettings layerSettings;

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

        // Indentation Colors

        public IndentColorSettings indentColors = new IndentColorSettings ();

    }
}