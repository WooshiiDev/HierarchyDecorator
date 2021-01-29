using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal abstract class SettingsTab
        {
        //GUI 
        private bool isShown;
        private readonly GUIContent content;

        // References
        protected readonly Settings settings;
        protected readonly SerializedObject serializedSettings;

        /// <summary>
        /// Constructor used to cache the data required
        /// </summary>
        /// <param name="settings">Current settings used for the hierarchy</param>
        public SettingsTab(string name, string icon)
            {
            settings = Settings.GetOrCreateSettings ();
            serializedSettings = Settings.GetSerializedSettings ();

            content = new GUIContent (name, GUIHelper.GetUnityIcon (icon));
            }

        /// <summary>
        /// Draw the settings tab
        /// </summary>
        public void OnGUI()
            {
            EditorGUILayout.BeginVertical (Style.tabBackgroundStyle, GUILayout.MinHeight (32f));
                {
                if (IsShown ())
                    {
                    OnTitleGUI ();
                    OnContentGUI ();
                    EditorGUILayout.Space ();
                    }
                }
            EditorGUILayout.EndVertical ();
            }

        /// <summary>
        /// Is the current tab open or closed, hiding the settings?
        /// </summary>
        protected bool IsShown()
            {
            return isShown = EditorGUILayout.Foldout (isShown, content, true, Style.foldoutHeaderStyle);
            }

        /// <summary>
        /// The title gui drawn, primarily to display a header of some form
        /// </summary>
        protected abstract void OnTitleGUI();

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected abstract void OnContentGUI();
        }
    }
