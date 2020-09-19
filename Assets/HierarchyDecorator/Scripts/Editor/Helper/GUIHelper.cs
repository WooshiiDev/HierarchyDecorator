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

    #endregion 

    #region Automated Fields

    public static void ToggleAuto(ref bool value, string label, GUIStyle style = null)
        {
        if (style == null)
            style = EditorStyles.toggle;

        EditorGUILayout.BeginHorizontal ();
            {
            EditorGUIUtility.labelWidth = 0;//style.CalcSize (new GUIContent (label)).x;
            EditorGUILayout.LabelField (label);

            EditorGUIUtility.labelWidth = 0;
            value = EditorGUILayout.Toggle (value);
            }
        EditorGUILayout.EndHorizontal ();
        }

    #endregion

    #region Button

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

    #endregion

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
                        localSelection = 0;

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
            property.serializedObject.ApplyModifiedProperties ();

        return property.isExpanded;
        }

    #endregion
    }
