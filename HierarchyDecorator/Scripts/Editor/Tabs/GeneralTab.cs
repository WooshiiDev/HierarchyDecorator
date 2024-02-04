using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [RegisterTab(0)]
    public class GeneralTab : SettingsTab
    {
        private string[] TagFields = new[] { "showTags" };
        private string[] LayerFields = new[] { "showLayers", "applyChildLayers" };

        public GeneralTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "globalData", "General", "d_CustomTool")
        {
            // --- General Features

            CreateDrawableGroup("Toggles")
                .RegisterSerializedProperty(serializedTab, "showActiveToggles", "activeSwiping", "swipeSameState", "swipeSelectionOnly", "depthMode");

            // --- Layers

            CreateDrawableGroup("Tags & Layers")
                .RegisterSerializedProperty(serializedTab, TagFields)
                .RegisterSerializedProperty(serializedTab, LayerFields).Space()
                .RegisterSerializedProperty(serializedTab, "tagLayerLayout");

            // --- Breadcrumbs

            SerializedProperty crumbA = serializedTab.FindPropertyRelative("instanceBreadcrumbs");
            SerializedProperty crumbB = serializedTab.FindPropertyRelative("fullDepthBreadcrumbs");

            SerializedProperty[] instanceCrumbs = SerializedPropertyUtility.GetChildProperties(crumbA, "show", "color", "style", "displayHorizontal");
            SerializedProperty[] depthCrumbs = SerializedPropertyUtility.GetChildProperties(crumbB, "show", "color", "style", "displayHorizontal");

            CreateDrawableGroup("Breadcrumbs")
                .RegisterSerializedProperty(serializedTab, "showBreadcrumbs")
                .RegisterDrawer(new SerializedGroupDrawer("Instance", instanceCrumbs))
                .RegisterDrawer(new SerializedGroupDrawer("Hierarchy", depthCrumbs));
        }
    }
}