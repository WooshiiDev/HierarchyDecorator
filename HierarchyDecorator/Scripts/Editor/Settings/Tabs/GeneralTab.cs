using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace HierarchyDecorator
    {
    internal class GeneralTab : SettingsTab
        {
        private GlobalSettings global;

        public GeneralTab() : base("General", "d_CustomTool")
            {
            global = settings.globalStyle;
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
            // ==================
            // ==== Features ====
            // ==================
            EditorGUILayout.LabelField ("Features", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref global.showActiveToggles, "Enable GameObject Toggles");
                GUIHelper.ToggleAuto (ref global.showComponents, "Enable Component Icons");
                GUIHelper.ToggleAuto (ref global.showMonoBehaviours, "Show All Component Icons");

                EditorGUILayout.HelpBox ("This will display all MonoBehaviour derived types that exist, with their custom icon. When enabled, using the custom icons will not be needed.", MessageType.Info);
                }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space ();

            // ===============
            // ==== Style ====
            // ===============
            EditorGUILayout.LabelField ("Style", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref global.twoToneBackground, "Show Two Tone Background");
                }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space ();

            // ================
            // ==== Layers ====
            // ================
            EditorGUILayout.LabelField ("Layer Display", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref global.showLayers, "Show Layers");
                GUIHelper.ToggleAuto (ref global.editableLayers, "Show layer selection on click");
                GUIHelper.ToggleAuto (ref global.applyChildLayers, "Update layer on children");
                }
            EditorGUI.indentLevel--;
            }
        }
    }
