using System.Collections.Generic;
using UnityEditor;

namespace HierarchyDecorator
{
    internal class GeneralTab : SettingsTab
    {
        internal class SettingGroup
        {
            public string label;
            public List<string> settings;

            public SettingGroup(string label)
            {
                this.label = label;
                settings = new List<string> ();
            }

            public SettingGroup(string label, string[] settings = null)
            {
                this.label = label;
                this.settings = new List<string> (settings);
            }

            public void DisplaySettings(SerializedProperty property)
            {
                EditorGUILayout.LabelField (label, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                foreach (string setting in settings)
                {
                    EditorGUILayout.PropertyField (property.FindPropertyRelative (setting), true);
                }

                EditorGUI.indentLevel--;
            }
        }

        private readonly SerializedProperty serializedGlobal;

        private readonly SettingGroup[] groups = new SettingGroup[]
            {
            new SettingGroup("Features", new string[] {"showActiveToggles", "showComponents", "showAllComponents"}),
            new SettingGroup("Style", new string[] {"twoToneBackground", "stretchWidth"}),
            new SettingGroup("Features", new string[] {"showLayers", "editableLayers", "applyChildLayers"}),
            };

        public GeneralTab() : base ("General", "d_CustomTool")
        {
            serializedGlobal = serializedSettings.FindProperty ("globalSettings");
        }

        /// <summary>
        /// The title gui drawn, primarily to display a header of some form
        /// </summary>
        protected override void OnTitleGUI()
        {
        }

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected override void OnContentGUI()
        {
            foreach (SettingGroup group in groups)
            {
                group.DisplaySettings (serializedGlobal);
                EditorGUILayout.Space ();
            }
        }
    }
}