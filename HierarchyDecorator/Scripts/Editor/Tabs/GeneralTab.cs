using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [RegisterTab(0)]
    public class GeneralTab : SettingsTab
    {
        private readonly SettingGroup[] groups = new SettingGroup[]
        {
            new SettingGroup("Features", new string[] {"showActiveToggles", "showComponentIcons", "twoToneBackground"}),
            new SettingGroup("Layers", new string[] {"showLayers", "editableLayers", "applyChildLayers"}),
        };

        public GeneralTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "globalData", "General", "d_CustomTool")
        {

        }

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected override void OnContentGUI()
        {
            EditorGUI.BeginChangeCheck ();

            int len = groups.Length;
            for (int i = 0; i < len; i++)
            {
                groups[i].DisplaySettings (serializedTab);
                HierarchyGUI.Space ();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplyModifiedProperties ();
            }
        }
    }
}