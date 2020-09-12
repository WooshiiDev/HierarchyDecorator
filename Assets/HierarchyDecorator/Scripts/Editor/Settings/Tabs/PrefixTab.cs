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
            mode.isExpanded = true;

            var prefixString = prefix.FindPropertyRelative ("prefix");
            var guiStyle = prefix.FindPropertyRelative ("guiStyle");


            EditorGUI.BeginChangeCheck ();
                {
                EditorGUILayout.PropertyField (prefixString);
                EditorGUILayout.PropertyField (guiStyle);

                modeSelection = GUILayout.SelectionGrid (modeSelection, new[] { "Light", "Dark" }, 2, EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.PropertyField (mode);
                }
            if (EditorGUI.EndChangeCheck ())
                {
                UpdateNames ();
                prefixes.serializedObject.ApplyModifiedProperties ();
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
        }
    }
