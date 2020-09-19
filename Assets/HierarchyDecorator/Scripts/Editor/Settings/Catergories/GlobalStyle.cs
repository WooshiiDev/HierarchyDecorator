using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    [System.Serializable]
    internal class GlobalSettings
        {   
        //Toggle
        public bool showComponents = true;
        public bool showMonoBehaviours = true;
        public bool showActiveToggles = true;

        //Style
        public bool twoToneBackground;

        //Layer Mask
        public bool showLayers = true;
        public bool editableLayers = true;
        public bool applyChildLayers = true;

        public Color GetTwoToneColour(Rect selectionRect)
            {
            bool hasRemainder = selectionRect.y % 32 != 0;

            if (EditorGUIUtility.isProSkin)
                {
                return hasRemainder
                    ? new Color (0.25f, 0.25f, 0.25f, 1)
                    : new Color (0.225f, 0.225f, 0.225f, 1);
                }
            else
                {
                return hasRemainder
                    ? new Color (0.8f, 0.8f, 0.8f, 1)
                    : new Color (0.75f, 0.75f, 0.75f, 1);
                }
            }

        public void OnDraw()
            {
            // === ======== ====
            // === Features ====
            // === ======== ====
            EditorGUILayout.LabelField ("Features", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref showActiveToggles, "Show GameObject Toggles");
                GUIHelper.ToggleAuto (ref showComponents, "Show Common Components");
                GUIHelper.ToggleAuto (ref showMonoBehaviours, "Show All MonoBehaviour Icons");

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
                GUIHelper.ToggleAuto (ref twoToneBackground, "Show Two Tone Background");
                }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space ();

            // ==============
            // ====Layers====
            // ==============
            EditorGUILayout.LabelField ("Layer Display", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
                {
                GUIHelper.ToggleAuto (ref showLayers, "Show Layers");
                GUIHelper.ToggleAuto (ref editableLayers, "Show layer selection on click");
                GUIHelper.ToggleAuto (ref applyChildLayers, "Update layer on children");
                }
            EditorGUI.indentLevel--;
                
            }
        }
    }
