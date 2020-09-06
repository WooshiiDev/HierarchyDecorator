using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class HierarchyDecoratorGUI
    {
    public static void LineSpacer(float height = 1)
        {
        Rect rect = EditorGUILayout.GetControlRect (false, 1);
        rect.height = height;

        EditorGUI.DrawRect (rect, Color.grey);
        }

    public static void LineSpacer(Color color, float height = 1)
        {
        Rect rect = EditorGUILayout.GetControlRect (false, 1);
        rect.height = height;

        EditorGUI.DrawRect (rect, color);
        }

    public static bool GetSerializedFoldout(SerializedProperty property)
        {
        EditorGUI.BeginChangeCheck ();
        property.isExpanded = EditorGUILayout.Foldout (property.isExpanded, property.displayName, true);
        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties ();

        return property.isExpanded;
        }

    public static void ToggleAuto(ref bool value, string label, GUIStyle style = null)
        {
        if (style == null)
            style = EditorStyles.toggle;

        EditorGUILayout.BeginHorizontal ();
            {
            EditorGUIUtility.labelWidth = 0;
            value = EditorGUILayout.Toggle (value);

            EditorGUIUtility.labelWidth = 0;//style.CalcSize (new GUIContent (label)).x;
            EditorGUILayout.LabelField (label);
            }
        EditorGUILayout.EndHorizontal ();
        }

    public static bool ButtonAction(string label, System.Action callback)
        {
        bool pressed = false;

        if (pressed = GUILayout.Button (label))
            callback?.Invoke ();

        return pressed;
        }

    public static bool ButtonAction(string label, GUIStyle style, System.Action callback)
        {
        bool pressed = false;

        if (pressed = GUILayout.Button (label, style))
            callback?.Invoke ();

        return pressed;
        }
    }
