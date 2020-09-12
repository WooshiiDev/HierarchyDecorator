using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
    {
    internal class StyleTab : SettingsTab
        {
        private SerializedProperty styles;
        private int styleIndex;

        private static List<string> styleNames;

        public StyleTab() : base ()
            {
            styles = serializedSettings.FindProperty ("styles");

            styleNames = new List<string> ();

            for (int i = 0; i < styles.arraySize; i++)
                styleNames.Add (styles.GetArrayElementAtIndex (i).displayName);
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
                EditorGUILayout.LabelField ("Style Settings");
                GUIHelper.SerializedArraySelection (styles, ref styleIndex, UpdateNames, UpdateNames);
                }
            EditorGUILayout.EndHorizontal ();

            styleIndex = GUILayout.SelectionGrid (styleIndex, styleNames.ToArray (), 3, EditorStyles.centeredGreyMiniLabel);
            }

        public override void OnBodyContentGUI()
            {
            if (styles.arraySize == 0)
                {
                EditorGUILayout.EndVertical ();
                return;
                }

            var style = styles.GetArrayElementAtIndex (styleIndex);

            EditorGUI.BeginChangeCheck ();
                {
                EditorGUILayout.PropertyField (style);
                }
            if (EditorGUI.EndChangeCheck ())
                {
                UpdateNames ();
                styles.serializedObject.ApplyModifiedProperties ();
                }
            }

        private void UpdateNames()
            {
            styleNames.Clear ();

            for (int i = 0; i < styles.arraySize; i++)
                {
                var name = styles.GetArrayElementAtIndex (i).displayName;
                styleNames.Add (name);
                }
            }
        }
    }
