using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace HierarchyDecorator
{
    public class StyleTab : SettingsTab
    {
        private ReorderableList styleList;

        private readonly Color BACKGROUND_COLOR = new Color (0.235f, 0.360f, 0.580f);
        private readonly Color OUTLINE_COLOR = new Color (0.15f, 0.15f, 0.15f, 1f);

        private string[] modes = new string[] { "Light Mode", "Dark Mode" };

        private SerializedProperty serializedStyles;
        private SettingGroup styleGlobalSettings;

        public StyleTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, serializedSettings.FindProperty ("styleData"), "Styles", "d_InputField Icon")
        {
            serializedStyles = serializedTab.FindPropertyRelative ("styles");

            styleList = new ReorderableList (serializedSettings, serializedStyles)
            {
                drawHeaderCallback = DrawHeader,
                drawFooterCallback = DrawFooter,

                drawElementCallback = DrawStyleElements,
                drawElementBackgroundCallback = DrawElementBackground,
                drawNoneElementCallback = DrawNoElements,

                elementHeightCallback = GetPropertyHeight,

                headerHeight = 0,
                footerHeight = 19f,

                showDefaultBackground = false,
                draggable = true,
                displayAdd = false,
                displayRemove = false
            };

            styleGlobalSettings = new SettingGroup ("Style Global Features", "displayLayers", "displayIcons");
        }

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected override void OnContentGUI()
        {
            EditorGUI.BeginChangeCheck ();

            styleGlobalSettings.DisplaySettings (serializedTab);

            if (EditorGUI.EndChangeCheck())
            {
                serializedSettings.ApplyModifiedProperties ();
            }

            EditorGUILayout.Space ();
            EditorGUILayout.LabelField ("Hierarchy Styles", EditorStyles.boldLabel);
            styleList.DoLayoutList ();
        }

        // Reorderable List GUI

        private void DrawHeader(Rect rect)
        {

        }

        private void DrawStyleElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.x += 3f;
            rect.width -= 3f;

            if (index >= serializedStyles.arraySize)
            {
                return;
            }

            SerializedProperty style = serializedStyles.GetArrayElementAtIndex (index);
            SerializedProperty nameProp = style.FindPropertyRelative ("name");

            Rect removeRect = rect;
            removeRect.x = removeRect.width + 6f;
            removeRect.width = 48f;
            removeRect.height = 18f;
            if (GUI.Button (removeRect, "Delete", EditorStyles.centeredGreyMiniLabel))
            {
                serializedStyles.DeleteArrayElementAtIndex (index);
                serializedSettings.ApplyModifiedProperties ();
                serializedSettings.Update ();
                return;
            }

            // Handle Foldout - the foldout doesn't need to stretch the entire height
            rect.y--;
            Rect foldoutRect = rect;
            foldoutRect.height = 19f;
            foldoutRect.width -= 49f;

            Rect labelRect = foldoutRect;
            labelRect.width += 10f;
            labelRect.height -= 2f;

            // Likewise with the style and the label
            Rect styleRect = labelRect;
            styleRect.y++;

            foldoutRect.x += 2;
            foldoutRect.width = 19f;

            EditorGUI.BeginChangeCheck ();
            {
                HierarchyGUI.DrawHierarchyStyle (settings.styleData[index], styleRect, labelRect, nameProp.stringValue, false);

                // Use GUI over EditorGUI due to overlapping on draggable
                style.isExpanded = GUI.Toggle (foldoutRect, style.isExpanded, "", EditorStyles.foldout);

                Rect propertyRect = rect;
                propertyRect.y += 21f;
                propertyRect.width -= 49f;

                if (style.isExpanded)
                {
                    Rect box = propertyRect;
                    box.height -= 18f;
                    box.x = styleRect.x;
                    box.width = styleRect.width;
                    box.y -= 3f;

                    GUI.Box (box, "");

                    var customDraws = new Dictionary<string, Action<Rect, SerializedProperty>> ()
                    {
                        { "modes", DrawModes }
                    };

                    SerializedPropertyUtility.DrawChildrenProperties (propertyRect, style, style.isExpanded, customDraws);
                }
            }
            if (EditorGUI.EndChangeCheck ())
            {
                serializedSettings.ApplyModifiedProperties ();
                serializedSettings.Update ();

                settings.styleData[index].UpdateStyle (EditorGUIUtility.isProSkin);
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawBackground (rect, isFocused);
        }

        private void DrawFooter(Rect rect)
        {
            rect.y -= 4f;

            rect.x += 0.25f * rect.width;
            rect.width *= 0.5f;

            Handles.BeginGUI ();
            Handles.DrawSolidRectangleWithOutline (rect, Color.clear, OUTLINE_COLOR);
            Handles.EndGUI ();

            GUI.Box (rect, "");

            // Draw optionals
            if (GUI.Button (rect, "Add New Style", Style.CenteredBoldLabel))
            {
                serializedTab.InsertArrayElementAtIndex (serializedTab.arraySize);
                serializedSettings.ApplyModifiedProperties ();
                serializedSettings.Update ();
            }
        }

        private void DrawNoElements(Rect rect)
        {
            EditorGUI.LabelField (rect, "No styles to display.");
        }

        private float GetPropertyHeight(int index)
        {
            if (serializedStyles.arraySize == 0)
            {
                return 0;
            }

            if (index >= serializedStyles.arraySize)
            {
                return 0;
            }

            SerializedProperty property = serializedStyles.GetArrayElementAtIndex (index);
            return GetPropertyHeight (property);
        }

        private float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight (property, property.isExpanded) + (property.isExpanded ? -36f : 0);
        }

        private void DrawBackground(Rect rect, bool isFocused)
        {
            Rect fullWidthRect = rect;
            fullWidthRect.x -= 3f;
            fullWidthRect.width += 6f;
            GUI.Box (fullWidthRect, "");

            Handles.BeginGUI ();
            {

                Rect spacerRect = fullWidthRect;
                spacerRect.height = 0;

                Handles.BeginGUI ();
                {
                    Handles.DrawSolidRectangleWithOutline (spacerRect, Color.clear, OUTLINE_COLOR);
                    spacerRect.y += rect.height;
                    Handles.DrawSolidRectangleWithOutline (spacerRect, Color.clear, OUTLINE_COLOR);

                    Rect splitRect = rect;
                    splitRect.x += 23f;
                    splitRect.width = 0;

                    Handles.DrawSolidRectangleWithOutline (splitRect, Color.clear, OUTLINE_COLOR);

                    splitRect.x = rect.width - 22f;

                    Handles.DrawSolidRectangleWithOutline (splitRect, Color.clear, OUTLINE_COLOR);

                }
                Handles.EndGUI ();


                //Rect splitRect = rect;
                //splitRect.x += 24f;
                //splitRect.width -= 72f;

                //Handles.DrawSolidRectangleWithOutline (splitRect, Color.clear, OUTLINE_COLOR);

                //Rect deleteRect = rect;
                //deleteRect.x = deleteRect.width - 72f;
                //deleteRect.width = 44f;
                //Handles.DrawSolidRectangleWithOutline (deleteRect, Color.cyan, OUTLINE_COLOR);
            }
            Handles.EndGUI ();
        }

        public void DrawModes(Rect rect, SerializedProperty property)
        {
            if (property.arraySize == 0)
            {
                return;
            }

            property.isExpanded = true;

            Rect selectionRect = rect;
            selectionRect.height = 21f;

            Rect modeRect = rect;
            modeRect.height -= 21f;
            modeRect.y += 21f;

            SerializedProperty lightMode = property.GetArrayElementAtIndex (0);
            SerializedProperty darkMode = property.GetArrayElementAtIndex (1);

            if (lightMode.isExpanded == darkMode.isExpanded)
            {
                bool isPro = EditorGUIUtility.isProSkin;

                lightMode.isExpanded = !isPro;
                darkMode.isExpanded = isPro;
            }

            EditorGUI.BeginChangeCheck ();

            int selected = lightMode.isExpanded ? 0 : 1;
            int selection = GUI.SelectionGrid (selectionRect, selected, modes, 2, EditorStyles.centeredGreyMiniLabel);

            SerializedProperty selectedMode = (selection == 0) ? lightMode : darkMode;
            EditorGUI.PropertyField (modeRect, selectedMode, new GUIContent (modes[selection]), true);

            if (EditorGUI.EndChangeCheck())
            {
                lightMode.isExpanded = selection == 0;
                darkMode.isExpanded = !lightMode.isExpanded;
            }
        }

        // Rect Helpers

        private Rect GetFullWidthInspector(Rect rect)
        {
            rect.x = 8f;

            return rect;
        }
    }
}