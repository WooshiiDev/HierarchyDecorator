using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

using UnityEditor;
#pragma warning disable CS0649

namespace HierarchyDecorator
    {
    internal class SettingsPreferences : SettingsProvider
        {

        //--- Settings ---
        private static HierarchyDecoratorSettings settings;
        private static SerializedObject serializedSettings;
        private static Editor settingsEditor;

        //Constructor
        public SettingsPreferences(string path, SettingsScope scope = SettingsScope.User) : base (path, scope) { }

        //Activate and Deactivate for saving/loading
        public override void OnDeactivate()
            {
            base.OnDeactivate ();
            }
  
        // On Load of Window
        public override void OnActivate(System.String searchContext, VisualElement rootElement)
            {
            base.OnActivate (searchContext, rootElement);

            if (settings == null)
                {
                settings = HierarchyDecoratorSettings.GetOrCreateSettings ();
                serializedSettings = HierarchyDecoratorSettings.GetSerializedSettings ();
                }
            }

        // GUI
        public override void OnTitleBarGUI()
            {
            base.OnTitleBarGUI ();
            }

        public override void OnGUI(string searchContext)
            {
            if (settings == null)
                {
                EditorGUILayout.LabelField ("Cannot find settings in project", EditorStyles.boldLabel);
                return;
                }

            DrawSettings ();
            }

        public override void OnFooterBarGUI()
            {
            base.OnFooterBarGUI ();
            }

        private void DrawSettings()
            {
            GUIStyle skin = new GUIStyle (GUI.skin.window)
                {
                padding = new RectOffset (2, 0, 0, 0),
                margin = new RectOffset (0, 0, 0, 0),
                };
            EditorGUILayout.InspectorTitlebar (false, settings, false);

            EditorGUILayout.BeginHorizontal ();
                {
                EditorGUILayout.BeginVertical ();
                    {

                    if (settingsEditor == null)
                        Editor.CreateCachedEditor (settings, null, ref settingsEditor);

                    settingsEditor.OnInspectorGUI ();
                    }
                EditorGUILayout.EndVertical ();
                }
            EditorGUILayout.EndHorizontal ();
            }
       
        }
    }

