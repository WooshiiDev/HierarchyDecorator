﻿using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public abstract class SettingsTab
    {
        //GUI
        private bool isShown;
        private readonly GUIContent content;

        // References
        protected Settings settings;
        protected SerializedObject serializedSettings;
        protected SerializedProperty serializedTab;

        /// <summary>
        /// Constructor used to cache the data required
        /// </summary>
        /// <param name="settings">Current settings used for the hierarchy</param>
        public SettingsTab(Settings settings, SerializedObject serializedSettings, SerializedProperty serializedTab, string name, string icon)
        {
            this.settings = settings;
            this.serializedSettings = serializedSettings;
            this.serializedTab = serializedTab;

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
            if (IsShown ())
            {
                OnContentGUI ();
                EditorGUILayout.Space ();
            }

            EditorGUILayout.EndVertical ();
        }

        /// <summary>
        /// Is the current tab open or closed, hiding the settings?
        /// </summary>
        protected bool IsShown()
        {
            return serializedTab.isExpanded = EditorGUILayout.Foldout (serializedTab.isExpanded, content, true, Style.FoldoutHeaderStyle);
        }

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected abstract void OnContentGUI();
    }
}