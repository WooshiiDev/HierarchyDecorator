using UnityEditor;

namespace HierarchyDecorator
    {
    internal static class SerializedPropertyUtility
        {
        public static void DrawChildrenProperties(SerializedProperty property, bool showChildrenRecursive)
            {
            var iterator = property.Copy ();
            var endProperty = iterator.Copy ();

            EditorGUI.indentLevel++;

            while (iterator.NextVisible(true))
                {
                if (SerializedProperty.EqualContents (iterator, endProperty.GetEndProperty ()))
                    break;

                EditorGUILayout.PropertyField (iterator, showChildrenRecursive);
                }

            EditorGUI.indentLevel--;
            }
        }
    }
