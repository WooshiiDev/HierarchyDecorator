using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class SerializedGroupDrawer : GUIDrawer<SerializedProperty[]>
    {
        private readonly GUIContent title;
        private readonly int indent;

        public SerializedGroupDrawer(params SerializedProperty[] target) : this(1, target)
        {
        }
        public SerializedGroupDrawer(string title, params SerializedProperty[] target) : base(target)
        {
            if (!string.IsNullOrEmpty(title))
                this.title = new GUIContent(title);
            indent = 1;
        }
        public SerializedGroupDrawer(int indent, params SerializedProperty[] target) : base(target)
        {
            this.indent = indent;
        }

        protected override void OnGUI()
        {
            if (title != null)
                HierarchyGUI.Space();
            EditorGUILayout.BeginVertical();
            {
                if (title != null)
                    EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                using (new GUIHelper.IndentChangedScope(indent))
                {
                    for (int i = 0; i < Target.Length; i++)
                    {
                        SerializedProperty property = Target[i];

                        EditorGUI.BeginChangeCheck();

                        EditorGUILayout.PropertyField(property);

                        if (EditorGUI.EndChangeCheck())
                        {
                            property.serializedObject.ApplyModifiedProperties();
                            optionOnChanged?.Invoke(property);
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected override float GetHeight()
        {
            float height = 0;
            for (int i = 0; i < Target.Length; i++)
            {
                SerializedProperty property = Target[i];
                height += EditorGUI.GetPropertyHeight(property, property.isExpanded);
            }
            return height;
        }
    }
}