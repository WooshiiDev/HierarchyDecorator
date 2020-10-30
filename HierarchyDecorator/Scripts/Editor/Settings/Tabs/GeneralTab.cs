using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal class GeneralTab : SettingsTab
        {
        private GlobalSettings global;

        public GeneralTab() : base()
            {
            Name = "General";
            global = settings.globalStyle;
            }

        public override void OnTitleHeaderGUI()
            {

            }

        public override void OnTitleContentGUI()
            {

            }

        public override void OnBodyHeaderGUI()
            {
            
            }

        public override void OnBodyContentGUI()
            {
            // === ======== ====
            // === Features ====
            // === ======== ====
            EditorGUILayout.LabelField ("Features", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref settings.globalStyle.showActiveToggles, "Show GameObject Toggles");
                GUIHelper.ToggleAuto (ref settings.globalStyle.showComponents, "Show Common Components");
                GUIHelper.ToggleAuto (ref settings.globalStyle.showMonoBehaviours, "Show All MonoBehaviour Icons");

                EditorGUILayout.HelpBox ("This will display all MonoBehaviour derived types that exist, with their custom icon. When enabled, using the custom icons will not be needed.", MessageType.Info);
                }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space ();

            // ==============
            // ====Style=====
            // ==============
            EditorGUILayout.LabelField ("Style", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref settings.globalStyle.twoToneBackground, "Show Two Tone Background");
                }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space ();

            // ==============
            // ====Layers====
            // ==============
            EditorGUILayout.LabelField ("Layer Display", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref settings.globalStyle.showLayers, "Show Layers");
                GUIHelper.ToggleAuto (ref settings.globalStyle.editableLayers, "Show layer selection on click");
                GUIHelper.ToggleAuto (ref settings.globalStyle.applyChildLayers, "Update layer on children");
                }
            EditorGUI.indentLevel--;
            }
        }
    }
