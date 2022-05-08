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
        private Settings t;
        private List<SettingsTab> tabs = new List<SettingsTab> ();

        private void OnEnable()
        {
            t = target as Settings;
            RegisterTabs ();
        }

        private void OnDisable()
        {
            if (serializedObject != null)
            {
                serializedObject.Dispose ();

            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update ();

            DrawTitle ();

#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.Space ();
#else
            GUILayout.Space(16f);
#endif
            foreach (SettingsTab tab in tabs)
            {
                EditorGUI.indentLevel++;
                tab.OnGUI ();
                EditorGUI.indentLevel--;
            }

            if (serializedObject.UpdateIfRequiredOrScript())
            {
                EditorApplication.RepaintHierarchyWindow ();
            }
        }

        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal ();
            {
                EditorGUILayout.LabelField ("Hierarchy Settings", Style.Title);

                GUILayout.FlexibleSpace ();

                if (GUILayout.Button ("GitHub Repository", EditorStyles.miniButtonMid))
                {
                    Application.OpenURL ("https://github.com/WooshiiDev/HierarchyDecorator/");
                }

                EditorGUILayout.Space ();

                if (GUILayout.Button ("Twitter", EditorStyles.miniButtonMid))
                {
                    Application.OpenURL ("https://twitter.com/WooshiiDev");
                }

                EditorGUILayout.Space ();
            }
            EditorGUILayout.EndHorizontal ();
        }

        private void RegisterTabs()
        {
            // Get all types that have the RegisterTab attribute

            foreach (Type type in GetTabs())
            {
                if (!type.IsSubclassOf (typeof (SettingsTab)))
                {
                    Debug.LogWarning ($"{type.Name} uses the RegisterTab attribute but does not inherit from SettingsTab.");
                    continue;
                }

                // Get the priority from the attribute to know where it should appear

                int priority = type.GetCustomAttribute<RegisterTabAttribute> ().priority;
                SettingsTab tab = Activator.CreateInstance (type, t, serializedObject) as SettingsTab;

                // Insert the tab if there is a slot for it, otherwise add it to the end

                if (tabs.Count > priority)
                {
                    tabs.Insert (priority, tab);
                }
                else
                {
                    tabs.Add (tab);
                }
            }
        }

        private Type[] GetTabs()
        {
            return ReflectionUtility.GetTypesFromAssemblies (
                t => t.GetCustomAttribute<RegisterTabAttribute> () != null
                );
        }
    }
}