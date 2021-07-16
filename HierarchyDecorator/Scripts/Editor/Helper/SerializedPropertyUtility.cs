using UnityEditor;

namespace HierarchyDecorator
{
    internal static class SerializedPropertyUtility
    {
        public static void DrawChildrenProperties(SerializedProperty property, bool showChildrenRecursive)
        {
            SerializedProperty iterator = property.Copy ();
            SerializedProperty endProperty = iterator.GetEndProperty ();

            EditorGUI.indentLevel++;

            while (iterator.NextVisible (true))
            {
                if (SerializedProperty.EqualContents (iterator, endProperty))
                {
                    break;
                }

                EditorGUILayout.PropertyField (iterator, showChildrenRecursive);
            }

            EditorGUI.indentLevel--;
        }
    }
}