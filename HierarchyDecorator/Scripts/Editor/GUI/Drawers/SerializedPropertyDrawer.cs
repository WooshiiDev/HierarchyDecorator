using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class SerializedPropertyElement : GUIDrawer<SerializedProperty>
    {
        public SerializedPropertyElement(SerializedProperty target) : base (target) { }

        protected override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            switch (Target.propertyType)
            {
                default:
                    EditorGUILayout.PropertyField (Target);
                    break;

                case SerializedPropertyType.Boolean:
                    Target.boolValue = GUILayout.Toggle (Target.boolValue, Target.displayName);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                Target.serializedObject.ApplyModifiedProperties();
            }
        }

        protected override float GetHeight()
        {
            return EditorGUI.GetPropertyHeight(Target, Target.isExpanded);
        }
    }
}