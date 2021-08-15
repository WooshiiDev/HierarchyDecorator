using System.Collections.Generic;
using UnityEditor;

namespace HierarchyDecorator
{
    public class GeneralTab : SettingsTab
    {
        private readonly SettingGroup[] groups = new SettingGroup[]
        {
            new SettingGroup("Features", new string[] {"showActiveToggles", "showComponentIcons", "twoToneBackground"}),
            new SettingGroup("Layers", new string[] {"showLayers", "editableLayers", "applyChildLayers"}),
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

            groups[0].DisplaySettings (serializedTab);
            groups[1].DisplaySettings (serializedTab);

            EditorGUILayout.Space ();

            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplyModifiedProperties ();
            }
        }
    }
}