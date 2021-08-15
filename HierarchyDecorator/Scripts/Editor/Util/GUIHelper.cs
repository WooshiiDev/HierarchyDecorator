using System;
using UnityEditor;
using UnityEngine;

public static class GUIHelper
{
    #region Style

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

    public static void LineSpacer(Rect rect, Color color, float height = 1)
    {
        rect.height = height;
        EditorGUI.DrawRect (rect, color);
    }

    public static Texture GetUnityIcon(string iconName)
    {
        return EditorGUIUtility.IconContent (iconName).image;
    }

    #endregion Style

    #region Layout

    /// <summary>
    /// Begin a horizontal group, as long as the given condition is true
    /// </summary>
    /// <param name="condition">The condition required to create the group</param>
    public static void BeginConditionalHorizontal(bool condition)
    {
        if (condition)
        {
            EditorGUILayout.BeginHorizontal ();
        }
    }

    /// <summary>
    /// End a horizontal group, as long as the given condition is true. Can be used
    /// in conjunction with <see cref="BeginConditionalHorizontal(bool)"/> by inverting the condition
    /// </summary>
    /// <param name="condition">The condition required to end the group</param>
    public static void EndConditionHorizontal(bool condition)
    {
        if (condition)
        {
            EditorGUILayout.EndHorizontal ();
        }
    }

    #endregion Layout

    #region Fields

    /// <summary>
    /// Draw a toggle and update the original boolean value
    /// </summary>
    /// <param name="value">Current boolean value for the toggle</param>
    /// <param name="label">Toggle label</param>
    public static void ToggleAuto(ref bool value, string label)
    {
        value = EditorGUILayout.ToggleLeft (label, value);
    }

    #endregion Fields

    #region GUI Actions

    public static bool ButtonAction(string label, Action callback = null)
    {
        bool pressed = false;

        if (pressed = GUILayout.Button (label))
        {
            callback?.Invoke ();
        }

        return pressed;
    }

    public static bool ButtonAction(string label, GUIStyle style, Action callback = null)
    {
        bool pressed = false;

        if (pressed = GUILayout.Button (label, style))
        {
            callback?.Invoke ();
        }

        return pressed;
    }

    public static bool FoldoutAction(bool toggle, string label, Action<bool> callback = null)
    {
        toggle = EditorGUILayout.Foldout (toggle, "General Settings", true);

        callback?.Invoke (toggle);

        return toggle;
    }

    public static bool FoldoutAction(bool toggle, string label, GUIStyle style, Action<bool> callback = null)
    {
        EditorGUILayout.BeginVertical (style);

        toggle = EditorGUILayout.Foldout (toggle, label, true);
        callback?.Invoke (toggle);

        EditorGUILayout.EndVertical ();

        return toggle;
    }

    #endregion GUI Actions

    #region Serialized GUI

    public static void SerializedArraySelection(SerializedProperty property, ref int selection, Action onAddCallback = null, Action onRemoveCallback = null)
    {
        int arraySize = property.arraySize;
        int localSelection = selection;

        EditorGUILayout.BeginHorizontal ();
        {
            EditorGUI.BeginChangeCheck ();
            {
                if (arraySize == 0)
                {
                    ButtonAction ("Add New", EditorStyles.toolbarButton, () =>
                        {
                            property.DeleteArrayElementAtIndex (0);
                            localSelection = 0;

                            onAddCallback?.Invoke ();
                        });

                    return;
                }

                if (GUILayout.Button ("Add New"))
                {
                    property.InsertArrayElementAtIndex (property.arraySize);
                    localSelection = property.arraySize - 1;

                    onAddCallback?.Invoke ();
                }

                if (GUILayout.Button ("Remove Selected"))
                {
                    property.DeleteArrayElementAtIndex (localSelection);
                    localSelection--;

                    if (localSelection < 0)
                    {
                        localSelection = 0;
                    }

                    onRemoveCallback?.Invoke ();
                }
            }
            if (EditorGUI.EndChangeCheck ())
            {
                property.serializedObject.ApplyModifiedProperties ();
                property.serializedObject.Update ();
                selection = localSelection;
            }
        }
        EditorGUILayout.EndHorizontal ();
    }

    public static bool GetSerializedFoldout(SerializedProperty property, string nameOverride = null)
    {
        nameOverride = nameOverride ?? property.displayName;

        EditorGUI.BeginChangeCheck ();
        property.isExpanded = EditorGUILayout.Foldout (property.isExpanded, nameOverride, true);
        if (EditorGUI.EndChangeCheck ())
        {
            property.serializedObject.ApplyModifiedProperties ();
        }

        return property.isExpanded;
    }

    #endregion Serialized GUI
}