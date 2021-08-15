using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    internal static class SerializedPropertyUtility
    {
        public static void DrawChildrenProperties(SerializedProperty property, bool showChildrenRecursive, Dictionary<string, Action<SerializedProperty>> onCustomDraw = null)
        {
            bool hasCustomDrawing = onCustomDraw != null; 

            SerializedProperty iterator = property.Copy ();
            SerializedProperty endProperty = iterator.GetEndProperty ();

            EditorGUI.indentLevel++;

            while (iterator.NextVisible (true))
            {
                if (SerializedProperty.EqualContents (iterator, endProperty))
                {
                    break;
                }

                string propertyName = iterator.name;
                if (hasCustomDrawing && onCustomDraw.ContainsKey (propertyName))
                {
                    onCustomDraw[propertyName]?.Invoke (iterator);
                }
                else
                {
                    EditorGUILayout.PropertyField (iterator, showChildrenRecursive);
                }
            }

            EditorGUI.indentLevel--;
        }

        public static void DrawChildrenProperties(Rect rect, SerializedProperty property, bool showChildrenRecursive, Dictionary<string, Action<Rect, SerializedProperty>> onCustomDraw = null)
        {
            bool hasCustomDrawing = onCustomDraw != null;

            SerializedProperty iterator = property.Copy ();
            SerializedProperty endProperty = iterator.GetEndProperty ();

            bool enterChildren = true;
            while (iterator.NextVisible (enterChildren))
            {
                if (SerializedProperty.EqualContents (iterator, endProperty))
                {
                    break;
                }

                string propertyName = iterator.name;
                bool customHasKey = hasCustomDrawing && onCustomDraw.ContainsKey (propertyName);

                float height = EditorGUI.GetPropertyHeight (iterator, showChildrenRecursive && !customHasKey);
                rect.height = height;

                if (customHasKey)
                {
                    onCustomDraw[propertyName]?.Invoke (rect, iterator);
                }
                else
                {
                    EditorGUI.PropertyField (rect, iterator, showChildrenRecursive);
                }

                rect.y += height + 1;
                enterChildren = false;
            }
        }
    }
}