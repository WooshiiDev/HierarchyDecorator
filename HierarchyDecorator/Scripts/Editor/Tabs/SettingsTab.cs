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

        protected List<DrawerGroup> settingGroups = new List<DrawerGroup>();

        public bool IsDirty { get; private set; } = false;

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
            Content = new GUIContent (name);
#endif
        }

        /// <summary>
        /// Draw the settings tab
        /// </summary>
        public void OnGUI()
        {
            if (IsDirty)
            {
                IsDirty = false;
            }

#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.BeginVertical (Style.TabBackground, GUILayout.MinHeight (32f));
#else
            EditorGUILayout.BeginVertical (Style.TabBackground, GUILayout.MinHeight (16f));
#endif

            EditorGUI.BeginChangeCheck ();
            {
                int len = settingGroups.Count;
                for (int i = 0; i < len; i++)
                {
                    settingGroups[i].OnGUI ();

                    if (i != len - 1)
                    {
                        HierarchyGUI.Space ();
                    }
                }

                OnContentGUI ();
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplyModifiedProperties();
                IsDirty = true;
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
        /// Create a drawable group.
        /// </summary>
        /// <param name="title">The group title.</param>
        /// <returns>Returns the created group.</returns>
        protected DrawerGroup CreateDrawableGroup(string title)
        {
            DrawerGroup group = new DrawerGroup (title);
            AddDrawerGroup(group);
            return group;
        }

        /// <summary>
        /// Register a drawer group.
        /// </summary>
        /// <param name="drawer">The drawer to add.</param>
        public void AddDrawerGroup(DrawerGroup drawer)
        {
            if (drawer == null)
            {
                Debug.LogError("Attempt to add null drawer to settings tab.");
                return;
            }

            if (ContainsGroup(drawer))
            {
                Debug.LogWarning("Attempt to add DrawableGroup with a title that already exists. Should specify each with different names.");
            }

            settingGroups.Add(drawer);
        }

        /// <summary>
        /// Check if the given group already has been registered to the settings tab.
        /// </summary>
        /// <param name="group">The group to check</param>
        /// <returns>Returns true if the group has been registered otherwise, will return false.</returns>
        public bool ContainsGroup(DrawerGroup group)
        {
            if (group == null)
            {
                return false;
            }

            return settingGroups.FindIndex(i => i.Title == group.Title) != -1;
        }
    }
}