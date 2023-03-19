using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class SerializedGroupDrawer : GUIDrawer<SerializedProperty[]>
    {
        private readonly GUIContent title;

        public SerializedGroupDrawer(string title, params SerializedProperty[] target) : base(target) 
        {
            this.title = new GUIContent(title);
        }

        protected override void OnGUI()
        {
            HierarchyGUI.Space();
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (int i = 0; i < Target.Length; i++)
                {
                    SerializedProperty property = Target[i];

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(property);

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                EditorGUI.indentLevel--;
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