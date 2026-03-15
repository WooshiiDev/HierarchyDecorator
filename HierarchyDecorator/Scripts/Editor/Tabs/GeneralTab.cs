using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace HierarchyDecorator
{
    [RegisterTab(0)]
    public class GeneralTab : SettingsTab
    {
        public GeneralTab(Settings settings, SerializedObject serializedSettings) : base(settings, serializedSettings, "globalData", "General", "d_CustomTool")
        {
            // --- General Features

            CreateDrawableGroup("Toggles")
                .RegisterSerializedProperty(serializedTab, "showActiveToggles", "activeToggleType", "activeSwiping", "swipeSameState", "swipeSelectionOnly", "depthMode");

            // --- Layers
            void SetupColorSettings(SerializedProperty tagLayerSettings, out SerializedProperty show, out SerializedProperty colorSettings, out SerializedProperty useSolid, out SerializedProperty useRandom, out SerializedProperty h, out SerializedProperty s, out SerializedProperty v)
            {
                show = tagLayerSettings.FindPropertyRelative(nameof(TagLayerSettingsBase.show));
                colorSettings = tagLayerSettings.FindPropertyRelative(nameof(TagLayerSettingsBase.colorSettings));
                useSolid = colorSettings.FindPropertyRelative(nameof(TagLayerColorSettings.useSolidColor));
                useRandom = colorSettings.FindPropertyRelative(nameof(TagLayerColorSettings.useRandomColor));
                h = colorSettings.FindPropertyRelative(nameof(TagLayerColorSettings.hue));
                s = colorSettings.FindPropertyRelative(nameof(TagLayerColorSettings.saturation));
                v = colorSettings.FindPropertyRelative(nameof(TagLayerColorSettings.brightness));
            }
            SerializedProperty tagSettings = serializedTab.FindPropertyRelative(nameof(GlobalData.tagSettings));
            SetupColorSettings(tagSettings, out var tagShow, out var tagColorSettings, out var tagUseSolidColor, out var tagUseRandomColor, out var tagUseRandomColorH, out var tagUseRandomColorS, out var tagUseRandomColorV);
            void OnChangedTagRandomColor(SerializedProperty property) => OnChangedRandomizeFields(property, tagUseRandomColorH, tagUseRandomColorS, tagUseRandomColorV);

            SerializedProperty layerSettings = serializedTab.FindPropertyRelative(nameof(GlobalData.layerSettings));
            SetupColorSettings(layerSettings, out var layerShow, out var layerColorSettings, out var layerUseSolidColor, out var layerUseRandomColor, out var layerUseRandomColorH, out var layerUseRandomColorS, out var layerUseRandomColorV);
            void OnChangedLayerRandomColor(SerializedProperty property) => OnChangedRandomizeFields(property, layerUseRandomColorH, layerUseRandomColorS, layerUseRandomColorV);

            CreateDrawableGroup("Tags & Layers")
                .RegisterSerializedProperty(serializedTab, nameof(GlobalData.tagLayerLayout))
                // tag settings
                .RegisterDrawer(new SerializedGroupDrawer("Tag Settings", SerializedPropertyUtility.GetChildProperties(
                    tagSettings,
                    nameof(TagSettings.show))))
                .RegisterDrawer(new SerializedGroupDrawer(SerializedPropertyUtility.GetChildProperties(
                    tagSettings,
                    nameof(TagSettings.hideUntagged)))).EnableIf(() => tagShow.boolValue)
                .RegisterDrawer(new SerializedGroupDrawer(SerializedPropertyUtility.GetChildProperties(
                    tagColorSettings,
                    nameof(TagLayerColorSettings.useSolidColor)))).EnableIf(() => tagShow.boolValue && !tagUseRandomColor.boolValue)
                .RegisterDrawer(new SerializedGroupDrawer(2, SerializedPropertyUtility.GetChildProperties(
                    tagColorSettings,
                    nameof(TagLayerColorSettings.solidColor)))).EnableIf(() => tagShow.boolValue).ShowIf(() => tagUseSolidColor.boolValue)
                .RegisterDrawer(new SerializedGroupDrawer(SerializedPropertyUtility.GetChildProperties(
                    tagColorSettings,
                    nameof(TagLayerColorSettings.useRandomColor))))
                    .EnableIf(() => tagShow.boolValue && !tagUseSolidColor.boolValue).OnChanged(OnChangedTagRandomColor)
                .RegisterDrawer(new SerializedGroupDrawer(2, SerializedPropertyUtility.GetChildProperties(
                    tagColorSettings,
                    nameof(TagLayerColorSettings.hue),
                    nameof(TagLayerColorSettings.saturation),
                    nameof(TagLayerColorSettings.brightness))))
                    .EnableIf(() => !tagUseSolidColor.boolValue).ShowIf(() => tagShow.boolValue && tagUseRandomColor.boolValue).OnChanged(OnChangedTagRandomColor)
                // layer settings
                .RegisterDrawer(new SerializedGroupDrawer("Layer Settings", SerializedPropertyUtility.GetChildProperties(
                    layerSettings,
                    nameof(LayerSettings.show))))
                .RegisterDrawer(new SerializedGroupDrawer(SerializedPropertyUtility.GetChildProperties(
                    layerSettings,
                    nameof(LayerSettings.applyChildLayers)))).EnableIf(() => layerShow.boolValue)
                .RegisterDrawer(new SerializedGroupDrawer(SerializedPropertyUtility.GetChildProperties(
                    layerColorSettings,
                    nameof(TagLayerColorSettings.useSolidColor)))).EnableIf(() => layerShow.boolValue && !layerUseRandomColor.boolValue)
                .RegisterDrawer(new SerializedGroupDrawer(2, SerializedPropertyUtility.GetChildProperties(
                    layerColorSettings,
                    nameof(TagLayerColorSettings.solidColor)))).EnableIf(() => layerShow.boolValue).ShowIf(() => layerUseSolidColor.boolValue)
                .RegisterDrawer(new SerializedGroupDrawer(SerializedPropertyUtility.GetChildProperties(
                    layerColorSettings,
                    nameof(TagLayerColorSettings.useRandomColor))))
                    .EnableIf(() => layerShow.boolValue && !layerUseSolidColor.boolValue).OnChanged(OnChangedLayerRandomColor)
                .RegisterDrawer(new SerializedGroupDrawer(2, SerializedPropertyUtility.GetChildProperties(
                    layerColorSettings,
                    nameof(TagLayerColorSettings.hue),
                    nameof(TagLayerColorSettings.saturation),
                    nameof(TagLayerColorSettings.brightness))))
                    .EnableIf(() => layerShow.boolValue && !layerUseSolidColor.boolValue).ShowIf(() => layerShow.boolValue && layerUseRandomColor.boolValue).OnChanged(OnChangedLayerRandomColor);
            if (tagUseRandomColor.boolValue)
                OnChangedTagRandomColor(tagUseRandomColor);
            if (layerUseRandomColor.boolValue)
                OnChangedLayerRandomColor(layerUseRandomColor);
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
        
        void OnChangedRandomizeFields(SerializedProperty changedProperty, SerializedProperty h, SerializedProperty s, SerializedProperty v)
        {
            if (TagLayerColorSettings.s_labelToHashedColorCache == null)
                TagLayerColorSettings.s_labelToHashedColorCache = new Dictionary<string, Color>();
            var cache = TagLayerColorSettings.s_labelToHashedColorCache;
            var path = changedProperty.propertyPath;
            var targets = path.Contains("tagSettings") ? InternalEditorUtility.tags : InternalEditorUtility.layers;
            foreach (var target in targets)
            {
                cache[target] = GUIHelper.GetHashColor(target, h.floatValue, s.floatValue, v.floatValue);
            }
        }
    }
}