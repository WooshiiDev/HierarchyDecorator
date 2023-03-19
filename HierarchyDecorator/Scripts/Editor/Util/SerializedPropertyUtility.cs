using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public static class SerializedPropertyUtility
    {
        private const float DUAL_PROPERTY_MARGIN = 8f;

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

        /// <summary>
        /// Draw a property at the given rect with provided styles.
        /// </summary>
        /// <param name="rect">The rect position to draw the property.</param>
        /// <param name="property">The property.</param>
        /// <param name="prefixStyle">The style for the prefix.</param>
        /// <param name="fieldStyle">The style for the field.</param>
        /// <returns>Returns the total rect for the prefix and property.</returns>
        public static Rect DrawPrefixAndProperty(Rect rect, SerializedProperty property, GUIStyle prefixStyle, GUIStyle fieldStyle, bool resizeField = false)
        {
            // Create labels and calculate sizes

            GUIContent label = new GUIContent (property.displayName + ": ");
            GUIContent fieldLabel = new GUIContent (GetPropertyFieldString(property));

            float labelWidth = prefixStyle.CalcSize (label).x;
            float fieldWidth = fieldStyle.CalcSize (fieldLabel).x;

            // Resize the label to remove spacing and draw prefix

            float guiLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = labelWidth;
            Rect prefixRect = EditorGUI.PrefixLabel (rect, label, prefixStyle);

            // Calculate the field size and draw

            Rect fieldRect = prefixRect;

            if (resizeField)
            {
                fieldRect.width = fieldWidth;
            }

            DrawPropertyFieldByType (fieldRect, property, fieldStyle);
            rect.width = labelWidth + fieldWidth + DUAL_PROPERTY_MARGIN;

            EditorGUIUtility.labelWidth = guiLabelWidth;

            return rect;
        }

        /// <summary>
        /// Draw two properties horizontally beside each other.
        /// </summary>
        /// <param name="rect">The total rect for both properties.</param>
        /// <param name="propertyA">The first property to draw.</param>
        /// <param name="propertyB">The second property to draw.</param>
        /// <param name="equalSizing">Should the properties be sized equally?</param>
        public static void DrawDualProperties(Rect rect, SerializedProperty propertyA, SerializedProperty propertyB, bool equalSizing = true)
        {
            if (equalSizing)
            {
                rect.width *= 0.5f;
                rect.width -= DUAL_PROPERTY_MARGIN;
            }

            rect.height = EditorGUI.GetPropertyHeight (propertyA);

            Rect propRect = DrawPrefixAndProperty (rect, propertyA, EditorStyles.label, EditorStyles.textField);

            if (!equalSizing)
            {
                propRect.x += propRect.width;
                propRect.width = rect.width - propRect.width;
            }
            else
            {
                propRect.x += rect.width;
                propRect.width = rect.width;
            }

            propRect.x += DUAL_PROPERTY_MARGIN;

            DrawPrefixAndProperty (propRect, propertyB, EditorStyles.label, EditorStyles.textField);
        }

        /// <summary>
        /// Draw a property based on its type rather than directly through the SerializedProperty, to allow for styles.
        /// </summary>
        /// <param name="rect">The property rect.</param>
        /// <param name="property">The property.</param>
        /// <param name="style">The style to apply to the GUI.</param>
        public static void DrawPropertyFieldByType(Rect rect, SerializedProperty property, GUIStyle style)
        {
            switch (property.propertyType)
            {
                default:
                    EditorGUI.PropertyField (rect, property, GUIContent.none, true);
                    break;

                case SerializedPropertyType.String:
                    property.stringValue = EditorGUI.TextField (rect, property.stringValue, style);
                    break;

                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.IntField (rect, property.intValue, style);
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.FloatField (rect, property.floatValue, style);
                    break;

                case SerializedPropertyType.Boolean:
                    property.boolValue = EditorGUI.Toggle (rect, property.boolValue, style);
                    break;

                case SerializedPropertyType.ObjectReference:
                    EditorGUI.ObjectField (rect, property, GUIContent.none);
                    break;
            }
        }

        /// <summary>
        /// Get the serialized property value as a string.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Returns the property value as a string.</returns>
        public static string GetPropertyFieldString(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                default:
                    return property.name;

                // Primitives/Built-in

                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString ();

                case SerializedPropertyType.Integer:
                case SerializedPropertyType.LayerMask:
                    return property.intValue.ToString ();

                case SerializedPropertyType.Float:
                    return property.floatValue.ToString ();

                case SerializedPropertyType.String:
                    return property.stringValue;

                case SerializedPropertyType.Enum:
                    return property.enumValueIndex.ToString ();

                case SerializedPropertyType.Character:
                    return property.stringValue.ToString ();

                // Vectors

                case SerializedPropertyType.Vector2:
                    return property.vector2Value.ToString ();

                case SerializedPropertyType.Vector3:
                    return property.vector3Value.ToString ();

                case SerializedPropertyType.Vector4:
                    return property.vector4Value.ToString ();

                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue.ToString ();

                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue.ToString ();

                // Quaternion

                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue.ToString ();

                // Rect

                case SerializedPropertyType.Rect:
                    return property.rectValue.ToString ();

                case SerializedPropertyType.RectInt:
                    return property.rectIntValue.ToString ();

                // Bounds

                case SerializedPropertyType.Bounds:
                    return property.boundsValue.ToString ();

                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue.ToString ();

                // Other Unity Types

                case SerializedPropertyType.Color:
                    return property.colorValue.ToString ();

                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue.ToString ();

                case SerializedPropertyType.Gradient:
                    return property.objectReferenceValue.ToString ();

                // Sizing

                case SerializedPropertyType.ArraySize:
                    return property.arraySize.ToString ();

                case SerializedPropertyType.FixedBufferSize:
                    return property.fixedBufferSize.ToString ();

                // References

                case SerializedPropertyType.ObjectReference:
                {
                    if (property.objectReferenceValue != null)
                    {
                        return property.objectReferenceValue.ToString ();
                    }

                    return "None";
                }

                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue.ToString ();

                case SerializedPropertyType.ManagedReference:
                    return property.displayName;
            }
        }

        public static SerializedProperty[] GetChildProperties(SerializedProperty property, params string[] childNames)
        {
            int len = childNames.Length;
            SerializedProperty[] properties = new SerializedProperty[len];

            for (int i = 0; i < len; i++)
            {
                properties[i] = property.FindPropertyRelative(childNames[i]);
            }

            return properties;
        }
    }
}