using System.Collections.Generic;
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

        protected List<DrawerGroup> settingGroups = new List<DrawerGroup> ();

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

            EditorGUI.BeginChangeCheck ();
            {
                for (int i = 0; i < settingGroups.Count; i++)
                {
                    settingGroups[i].OnGUI ();
                    HierarchyGUI.Space ();
                }

                OnContentGUI ();
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplyModifiedProperties ();
            }

            EditorGUILayout.EndVertical ();
        }

        /// <summary>
        /// GUI Method called after setting groups are drawn.
        /// </summary>
        protected virtual void OnContentGUI()
        {

        }

        /// <summary>
        /// Craete 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        protected DrawerGroup CreateDrawableGroup(string title)
        {
            DrawerGroup group = new DrawerGroup (title);
            
            if (settingGroups.FindIndex(i => i.Title == title) != -1)
            {
                Debug.LogWarning ("Attempt to add DrawableGroup with a title that already exists. Should specify each with different names.");
            }

            settingGroups.Add (group);
            return group;
        }
    }
}