using UnityEngine;
using UnityEditor;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace HierarchyDecorator
{
    public class SettingsPreferences : SettingsProvider
    {
        //--- Settings ---
        private static Settings settings;

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
                settings = HierarchyDecorator.Settings;
                serializedSettings = HierarchyDecorator.GetSerializedSettings ();
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
            if (settingsEditor == null)
            {
                Editor.CreateCachedEditor (settings, null, ref settingsEditor);
                return;
            }

            settingsEditor.OnInspectorGUI ();
        }
    }
}