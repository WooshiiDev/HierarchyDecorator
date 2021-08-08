using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    internal abstract class SettingsTab
    {
        //GUI
        private bool isShown;
        private readonly GUIContent content;

        // References
        protected Settings settings;
        protected SerializedObject serializedSettings;

        /// <summary>
        /// Constructor used to cache the data required
        /// </summary>
        /// <param name="settings">Current settings used for the hierarchy</param>
        public SettingsTab(Settings settings, SerializedObject serializedSettings, string name, string icon)
        {
            this.settings = settings;
            this.serializedSettings = serializedSettings;

            content = new GUIContent (name, GUIHelper.GetUnityIcon (icon));
        }

        /// <summary>
        /// Draw the settings tab
        /// </summary>
        public void OnGUI()
        {
#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.BeginVertical (Style.TabBackgroundStyle, GUILayout.MinHeight (32f));
#else
            EditorGUILayout.BeginVertical (Style.TabBackgroundStyle, GUILayout.MinHeight (16f));
#endif
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
            return isShown = EditorGUILayout.Foldout (isShown, content, true, Style.FoldoutHeaderStyle);
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