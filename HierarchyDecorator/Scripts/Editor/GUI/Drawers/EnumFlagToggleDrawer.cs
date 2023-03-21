using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
{
    public class EnumFlagToggleDrawer<T> : GUIDrawer<SerializedProperty> where T : Enum
    {
        private struct Entry
        {
            public readonly string Name;
            public readonly int Value;

            public Entry(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }

        private static readonly string[] SpecialCases =
        {
            "All",
            "Everything",
            "None",
            "Nothing",
        };

        private int selection = 0;
        private List<Entry> entries = new List<Entry>();

        /// <summary>
        /// Are the toggles inline with the label?
        /// </summary>
        public bool Inline { get; set; }

        /// <summary>
        /// The toggle style.
        /// </summary>
        public GUIStyle ToggleStyle { get; set; }

        /// <summary>
        /// The enum label.
        /// </summary>
        public GUIContent Label { get; set; }

        public EnumFlagToggleDrawer(SerializedProperty target) : base(target) 
        {
            selection = target.intValue;
            Label = new GUIContent(target.displayName);

            int i = 0;
            foreach (var val in Enum.GetValues(typeof(T)))
            {
                entries.Add(new Entry(Target.enumDisplayNames[i], (int)val));
                i++;
            }


            for (i = 0; i < SpecialCases.Length; i++)
            {
                SortSpecialCase(SpecialCases[i], 0);
            }
        }

        protected override float GetHeight()
        {
            return EditorGUI.GetPropertyHeight(Target, Target.isExpanded);
        }

        protected override void OnGUI()
        {
            GUIStyle style = ToggleStyle == null ? EditorStyles.miniButton : ToggleStyle;

            if (Inline)
            {
                EditorGUILayout.BeginHorizontal();
            }

            EditorGUILayout.LabelField(Target.displayName, Style.CenteredLabel, GUILayout.MinWidth(0));

            int currentValue = Target.intValue;

            for (int i = 0; i < entries.Count; i++)
            {
                Entry entry = entries[i];

                string name = entry.Name;
                int value = entry.Value;

                bool isSelected = (Target.intValue & value) != 0;

                EditorGUI.BeginChangeCheck();
                bool toggle = GUILayout.Toggle(isSelected, name, style);
                if (EditorGUI.EndChangeCheck())
                {
                    if (toggle != isSelected)
                    {
                        currentValue ^= value;
                    }
                    //currentValue = value;
                }
            }
            Target.longValue = currentValue;


            if (Inline)
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SortSpecialCase(string enumName, int defaultIndex)
        {
            int allIndex = entries.FindIndex(x => x.Name == enumName);
            if (allIndex != -1)
            {
                Entry allOption = entries[allIndex];
                entries.RemoveAt(allIndex);
                entries.Insert(defaultIndex, allOption);
            }
        }
    }
}