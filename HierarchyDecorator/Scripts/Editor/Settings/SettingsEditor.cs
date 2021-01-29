using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
    {
    //Seeing this makes me realize I should have made a smaller name for this.
    [CustomEditor (typeof (Settings))]
    internal class SettingsEditor : Editor
        {
        private Settings t;

        // --- GUI ---
        private static int tabSelection;
        private static Vector2 scrollView;

        // --- Others ---
        private static GUIStyle titleStyle;
        private GUIContent imageContent;

        //Tab Information
        private static List<SettingsTab> tabs;

        private void OnEnable()
            {
            t = target as Settings;

            if (tabs == null)
                {
                tabs = new List<SettingsTab> ();

                RegisterTab (new GeneralTab ());
                RegisterTab (new PrefixTab ());
                RegisterTab (new IconTab ());

                imageContent = new GUIContent (Textures.Banner);
                }

            }

        public override void OnInspectorGUI()
            {
            if (serializedObject == null)
                return;

            if (titleStyle == null)
                {
                titleStyle = new GUIStyle (EditorStyles.boldLabel)
                    {
                    fontSize = 18,
                    fixedHeight = 21,
                    };
                }

            EditorGUILayout.BeginHorizontal ();
                {
                EditorGUILayout.LabelField ("Hierarchy Settings", titleStyle);

                GUILayout.FlexibleSpace ();

                if (GUILayout.Button ("GitHub Repository", EditorStyles.miniButtonMid))
                    Application.OpenURL ("https://github.com/WooshiiDev/HierarchyDecorator/");

                EditorGUILayout.Space ();

                if (GUILayout.Button ("Twitter", EditorStyles.miniButtonMid))
                    Application.OpenURL ("https://twitter.com/WooshiiDev");

                EditorGUILayout.Space ();
                }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.Space ();

            foreach (var tab in tabs)
                {
                EditorGUI.BeginChangeCheck ();
                    {
                    EditorGUI.indentLevel++;
                        {
                        tab.OnGUI ();
                        }
                    EditorGUI.indentLevel--;
                    }
                if (EditorGUI.EndChangeCheck ())
                    {
                    EditorUtility.SetDirty (t);
                    EditorApplication.RepaintHierarchyWindow ();
                    }
                }

            serializedObject.UpdateIfRequiredOrScript ();
            }

        /// <summary>
        /// Register a tab to draw
        /// </summary>
        public void RegisterTab(SettingsTab tab)
            {
            tabs.Add (tab);
            }
        } 
    }
