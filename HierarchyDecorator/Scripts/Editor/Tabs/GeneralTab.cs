using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [RegisterTab(0)]
    public class GeneralTab : SettingsTab
    {
        public GeneralTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "globalData", "General", "d_CustomTool")
        {
            CreateDrawableGroup ("Features")
                .RegisterSerializedProperty (serializedTab, "showActiveToggles", "showComponentIcons", "twoToneBackground");

            CreateDrawableGroup ("Layers")
                .RegisterSerializedProperty (serializedTab, "showLayers", "editableLayers", "applyChildLayers");

            CreateDrawableGroup("Breadcrumbs")
                .RegisterSerializedProperty(serializedTab, "showBreadcrumbs", "instanceBreadcrumbs", "fullDepthBreadcrumbs");
        }
    }
}