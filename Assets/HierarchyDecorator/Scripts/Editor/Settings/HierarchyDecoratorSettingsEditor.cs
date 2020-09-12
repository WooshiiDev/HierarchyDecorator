using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
    {
    //Seeing this makes me realize I should have made a smaller name for this.
    [CustomEditor (typeof (HierarchyDecoratorSettings))]
    internal class HierarchyDecoratorSettingsEditor : Editor
        {
        private HierarchyDecoratorSettings t;

        private GeneralTab generalTab;
        private PrefixTab prefixTab;
        private StyleTab styleTab;
        private IconTab iconTab;
        private InfoTab infoTab;

        // --- GUI ---
        private string[] tabNames = { "Global", "Prefixes", "Styles", "Icons", "About"};
        private int tabSelection;

        private Vector2 scrollView;

        // --- Others ---
        private GUIStyle verticalStyle;
        private GUIStyle greyMidStyle;

        private void OnEnable()
            {
            t = target as HierarchyDecoratorSettings;

            generalTab = new GeneralTab ();
            prefixTab = new PrefixTab ();
            styleTab = new StyleTab();
            iconTab = new IconTab ();
            infoTab = new InfoTab();
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
            EditorGUILayout.BeginVertical (verticalStyle);
                {
                tabSelection = GUILayout.SelectionGrid (tabSelection, tabNames, tabNames.Length, greyMidStyle);

                switch (tabSelection)
                    {
                    case 1:
                        prefixTab.OnTitleContentGUI ();
                        break;

                    case 2:
                        styleTab.OnTitleContentGUI ();
                        break;

                    case 3:
                        iconTab.OnTitleContentGUI ();
                        break;

                    }
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
                        switch (tabSelection)
                            {
                            case 0:
                                generalTab.OnBodyHeaderGUI ();
                                EditorGUILayout.Space ();
                                generalTab.OnBodyContentGUI ();
                                break;
                            //PREFIX SELECTION
                            case 1:
                                prefixTab.OnBodyHeaderGUI ();
                                EditorGUILayout.Space ();
                                prefixTab.OnBodyContentGUI ();
                                break;

                            //STYLES
                            case 2:
                                styleTab.OnBodyHeaderGUI ();
                                EditorGUILayout.Space ();
                                styleTab.OnBodyContentGUI ();
                                break;

                            case 3:
                                iconTab.OnBodyHeaderGUI ();
                                EditorGUILayout.Space ();
                                iconTab.OnBodyContentGUI ();
                                break;

                            case 4:
                                infoTab.OnBodyHeaderGUI ();
                                EditorGUILayout.Space ();
                                infoTab.OnBodyContentGUI ();
                                break;
                            }
                        }
                    EditorGUI.indentLevel--;
                    }
                EditorGUILayout.EndVertical ();
                }
            if (EditorGUI.EndChangeCheck ())
                {
                EditorApplication.RepaintHierarchyWindow ();
                }
            }
        }
    }
