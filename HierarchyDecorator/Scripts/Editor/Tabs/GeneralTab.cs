using System.Collections.Generic;
using UnityEditor;

namespace HierarchyDecorator
{
    public class GeneralTab : SettingsTab
    {
        private readonly SettingGroup[] groups = new SettingGroup[]
        {
            new SettingGroup("Features", new [] 
            {
                "showComponentIcons",
                "twoToneBackground"
            }),

            new SettingGroup("Toggles", new [] 
            {
                "showActiveToggles",
                "toggleClickDrag",
                "toggleSameState",
                "toggleSelectionOnly",
                "depthMode"
            }),
            new SettingGroup("Layers", new [] 
            {
                "showLayers",
                "editableLayers",
                "applyChildLayers"
            }),
        };

        public GeneralTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, serializedSettings.FindProperty ("globalData"), "General", "d_CustomTool")
        {

        }

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected override void OnContentGUI()
        {
            EditorGUI.BeginChangeCheck ();

            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].DisplaySettings (serializedTab);
            }

            EditorGUILayout.Space ();

            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplyModifiedProperties ();
            }
        }
    }
}