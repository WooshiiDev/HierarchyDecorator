using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public abstract class SettingsTab
    {
        protected readonly Settings settings;
        protected readonly SerializedObject serializedSettings;
        protected readonly SerializedProperty serializedTab;

        public readonly GUIContent Content;

        /// <summary>
        /// Constructor used to cache the data required
        /// </summary>
        /// <param name="settings">Current settings used for the hierarchy</param>
        public SettingsTab(Settings settings, SerializedObject serializedSettings, string serializedTabName, string name, string icon)
        {
            this.settings = settings;
            this.serializedSettings = serializedSettings;
            this.serializedTab = serializedSettings.FindProperty(serializedTabName);

#if UNITY_2019_4_OR_NEWER
            Content = new GUIContent (name, GUIHelper.GetUnityIcon (icon));
#else
            content = new GUIContent (name);
#endif
        }

        /// <summary>
        /// Draw the settings tab
        /// </summary>
        public void OnGUI()
        {
#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.BeginVertical (Style.TabBackground, GUILayout.MinHeight (32f));
#else
            EditorGUILayout.BeginVertical (Style.TabBackground, GUILayout.MinHeight (16f));
#endif

            OnContentGUI ();

            EditorGUILayout.EndVertical ();
        }

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected abstract void OnContentGUI();
    }
}