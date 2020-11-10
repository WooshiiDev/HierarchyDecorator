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
        private int tabSelection;

        private Vector2 scrollView;

        // --- Others ---
        private GUIStyle verticalStyle;
        private GUIStyle greyMidStyle;

        //Tab Information
        public SettingsTab currentTab;

        public List<SettingsTab> tabs = new List<SettingsTab> ();
        private List<string> names = new List<string> ();

        private void OnEnable()
            {
            t = target as Settings;

            RegisterTab (new GeneralTab ());
            RegisterTab (new PrefixTab ());
            //RegisterTab (new StyleTab ());
            RegisterTab (new IconTab ());
            RegisterTab (new InfoTab ());

            currentTab = tabs[0];
            }

        public override void OnInspectorGUI()
            {
            if (serializedObject == null)
                return;

            serializedObject.UpdateIfRequiredOrScript ();

            if (verticalStyle == null)
                {
                verticalStyle = new GUIStyle (GUI.skin.window)
                    {
                    padding = new RectOffset (0, 0, 10, 10),
                    fontSize = 10
                    };

                greyMidStyle = new GUIStyle (EditorStyles.centeredGreyMiniLabel)
                    {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    };
                }

            DrawHeaderContent ();
            DrawBodyContent ();
            }

        private void DrawHeaderContent()
            {
            EditorGUILayout.BeginVertical (verticalStyle, GUILayout.Height (30));
                {
                EditorGUI.BeginChangeCheck ();
                tabSelection = GUILayout.SelectionGrid (tabSelection, names.ToArray(), names.Count, greyMidStyle);
                if (EditorGUI.EndChangeCheck ())
                    currentTab = tabs[tabSelection];

                currentTab.OnTitleHeaderGUI ();
                currentTab.OnTitleContentGUI (); 
                }
            EditorGUILayout.EndVertical ();

            EditorGUILayout.Space ();
            }

        private void DrawBodyContent()
            {
            EditorGUI.BeginChangeCheck ();
                {
                EditorGUILayout.BeginVertical (verticalStyle);
                    {
                    EditorGUI.indentLevel++;
                        {
                        currentTab.OnBodyHeaderGUI ();
                        EditorGUILayout.Space ();
                        currentTab.OnBodyContentGUI ();
                        }
                    EditorGUI.indentLevel--;
                    }
                EditorGUILayout.EndVertical ();
                }
            if (EditorGUI.EndChangeCheck ())
                {
                EditorUtility.SetDirty (t);
                EditorApplication.RepaintHierarchyWindow ();
                }
            }

        public void RegisterTab(SettingsTab tab)
            {
            if (!names.Contains (tab.Name))
                {
                tabs.Add (tab);
                names.Add (tab.Name);
                }
            }
        } 
    }
