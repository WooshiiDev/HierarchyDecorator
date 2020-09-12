using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
    {
    internal class PrefixTab : SettingsTab
        {
        private SerializedProperty prefixes;
        private int prefixIndex;
        private int modeSelection;

        private static List<string> prefixNames;

        public PrefixTab() : base()
            {
            prefixes = serializedSettings.FindProperty ("prefixes");

            prefixNames = new List<string> ();

            for (int i = 0; i < prefixes.arraySize; i++)
                prefixNames.Add ("Prefix " + prefixes.GetArrayElementAtIndex (i).displayName);
            }

        public override void OnTitleHeaderGUI()
            {
            //EditorGUILayout.LabelField ("Prefix Settings");
            }

        public override void OnTitleContentGUI()
            {
              }
            
        public override void OnBodyHeaderGUI()
            {
            EditorGUILayout.BeginHorizontal ();
                {
                EditorGUILayout.LabelField ("Prefix Settings");
                GUIHelper.SerializedArraySelection (prefixes, ref prefixIndex, UpdateNames, UpdateNames);
                }
            EditorGUILayout.EndHorizontal ();

            prefixIndex = GUILayout.SelectionGrid (prefixIndex, prefixNames.ToArray (), 3, EditorStyles.centeredGreyMiniLabel);
            }

        public override void OnBodyContentGUI()
            {
            if (prefixes.arraySize == 0)
                {
                EditorGUILayout.EndVertical ();
                return;
                }

            var prefix = prefixes.GetArrayElementAtIndex (prefixIndex);
            var mode = (modeSelection == 0) ? prefix.FindPropertyRelative ("lightMode") : prefix.FindPropertyRelative ("darkMode");
            //mode.isExpanded = true;

            var prefixString = prefix.FindPropertyRelative ("prefix");
            var guiStyle = prefix.FindPropertyRelative ("guiStyle");

            EditorGUI.BeginChangeCheck ();
                {
                int styleSelection = GetStyleIndex (guiStyle.stringValue);
                string[] styles = settings.GetStyleNames ();

                EditorGUILayout.PropertyField (prefixString);

                if (styles.Length == 0)
                    EditorGUILayout.LabelField ("Cannot find style, please add a style to the Style Tab");
                else
                    guiStyle.stringValue = styles[EditorGUILayout.Popup ("Style", styleSelection, styles)];

                EditorGUILayout.Space ();

                //Gotta do this for horizontal alignment
                EditorGUILayout.BeginHorizontal ();
                    {
                    mode.isExpanded = EditorGUILayout.Foldout (true, mode.displayName, true);
                    modeSelection = GUILayout.SelectionGrid (modeSelection, new[] { "Light", "Dark" }, 2, EditorStyles.centeredGreyMiniLabel);
                    }
                EditorGUILayout.EndHorizontal ();


                SerializedProperty iterator = mode.Copy();

                if (mode.isExpanded)
                    {
                    while (iterator.NextVisible (true))
                        {
                        if (SerializedProperty.EqualContents (iterator, mode.GetEndProperty ()))
                            break;

                        EditorGUILayout.PropertyField (iterator);
                        }
                    }

                //EditorGUILayout.PropertyField (mode);
                }
            if (EditorGUI.EndChangeCheck ())
                {
                UpdateNames ();
                prefixes.serializedObject.ApplyModifiedProperties ();

                EditorApplication.RepaintHierarchyWindow ();
                }
            }

        private void UpdateNames()
            {
            prefixNames.Clear ();

            for (int i = 0; i < prefixes.arraySize; i++)
                {
                var name = "Prefix " + prefixes.GetArrayElementAtIndex (i).displayName;

                //if (!prefixNames.Contains (name))
                    prefixNames.Add (name);
                }
            }

        private int GetStyleIndex(string styleName)
            {
            for (int i = 0; i < settings.styles.Count; i++)
                {
                string name = settings.styles[i].name;

                if (styleName == name)
                    return i;
                }

            return -1;
            }
        }
    }
