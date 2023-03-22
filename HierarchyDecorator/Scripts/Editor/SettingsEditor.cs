using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    //Seeing this makes me realize I should have made a smaller name for this.
    [CustomEditor (typeof (Settings))]
    internal class SettingsEditor : Editor
    {
        private Settings settings;

        // Grid selection

        private bool hasInitialized = false;
        private SettingsTab selectedTab;

        private List<SettingsTab> tabs = new List<SettingsTab> ();
        private List<GUIContent> tabNames = new List<GUIContent> ();

        private int selectedTabIndex = 0;

        private void OnEnable()
        {
            settings = target as Settings;
            hasInitialized = false;
            Repaint();
        }

        private void Initialize()
        {
            SetupValues();
            RegisterTabs();

            hasInitialized = true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        public override void OnInspectorGUI()
        {
            if (!hasInitialized)
            {
                Initialize();
                return;
            }

            EditorGUILayout.BeginVertical(Style.InspectorPadding);

            DrawTitle();
            selectedTab?.OnGUI();

            if (selectedTab.IsDirty)
            {
                serializedObject.Update();
                EditorApplication.RepaintHierarchyWindow();
                Repaint();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTitle()
        {
            // Draw Header
            
#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.BeginVertical (Style.TabBackground, GUILayout.MinHeight (32f));
#else
            EditorGUILayout.BeginVertical (Style.TabBackground, GUILayout.MinHeight (16f));
#endif
            EditorGUILayout.BeginHorizontal ();
            {
                EditorGUILayout.LabelField ("Hierarchy Settings", Style.Title);

                GUILayout.FlexibleSpace ();

                // Link to repo for convenience

                if (GUILayout.Button ("GitHub Repository", EditorStyles.miniButtonMid))
                {
                    Application.OpenURL ("https://github.com/WooshiiDev/HierarchyDecorator/");
                }
            }
            EditorGUILayout.EndHorizontal ();

            HierarchyGUI.Space ();

            // --- Selection

            EditorGUI.BeginChangeCheck ();
            {
                selectedTabIndex = GUILayout.SelectionGrid (selectedTabIndex, tabNames.ToArray (),
                    Mathf.Min (3, tabs.Count), Style.LargeButtonSmallTextStyle);
            }
            if (EditorGUI.EndChangeCheck ())
            {
                selectedTab = tabs[selectedTabIndex];
            }

            EditorGUILayout.EndVertical ();
        }

        private void RegisterTabs()
        {
            // Get all types that have the RegisterTab attribute

            foreach (Type type in GetTabs())
            {
                // If we find any type that is not a SettingsTab alert the user

                if (!type.IsSubclassOf (typeof (SettingsTab)))
                {
                    Debug.LogWarning ($"{type.Name} uses the RegisterTab attribute but does not inherit from SettingsTab.");
                    continue;
                }

                // Get the priority from the attribute to know where it should appear

                int priority = type.GetCustomAttribute<RegisterTabAttribute> ().priority;
                SettingsTab tab = Activator.CreateInstance (type, settings, serializedObject) as SettingsTab;

                // Insert the tab if there is a slot for it, otherwise add it to the end

                if (tabs.Count > priority)
                {
                    tabs.Insert (priority, tab);
                    tabNames.Insert (priority, tab.Content);
                }
                else
                {
                    tabs.Add (tab);
                    tabNames.Add (tab.Content);
                }
            }

            selectedTab = tabs[0];
        }

        private Type[] GetTabs()
        {
            return ReflectionUtility.GetTypesFromAssemblies (
                t => t.GetCustomAttribute<RegisterTabAttribute> () != null
                );
        }

        private void SetupValues()
        {
            tabs.Clear ();
            tabNames.Clear ();

            selectedTabIndex = 0;
        }
    }
}