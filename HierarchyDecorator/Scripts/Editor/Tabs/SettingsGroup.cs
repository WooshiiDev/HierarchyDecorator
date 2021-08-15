using System.Collections.Generic;
using UnityEditor;

namespace HierarchyDecorator
{
    public class SettingGroup
    {
        private readonly string label;
        private readonly string[] settings;

        public SettingGroup(string label, params string[] settings)
        {
            this.label = label;
            this.settings = settings;
        }

        public void DisplaySettings(SerializedProperty property)
        {
            EditorGUILayout.LabelField (label, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            foreach (string setting in settings)
            {
                EditorGUILayout.PropertyField (property.FindPropertyRelative (setting), true);
            }

            EditorGUI.indentLevel--;
        }
    }

}
