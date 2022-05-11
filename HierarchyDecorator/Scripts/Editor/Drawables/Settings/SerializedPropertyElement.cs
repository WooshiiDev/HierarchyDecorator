using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class SerializedPropertyElement : DrawableElement<SerializedProperty>
    {
        public SerializedPropertyElement(SerializedProperty target) : base (target) { }

        protected override void OnElementDraw()
        {
            switch (Target.propertyType)
            {
                default:
                    EditorGUILayout.PropertyField (Target);
                    break;

                case SerializedPropertyType.Boolean:
                    Target.boolValue = GUILayout.Toggle (Target.boolValue, Target.displayName);
                    break;
            }
        }
    }
}
