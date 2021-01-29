using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HierarchyDecorator
    {
    internal class PrefixTab : SettingsTab
        {
        private ReorderableList prefixList;
        private SerializedProperty prefixes;

        private readonly Color BACKGROUND_COLOR = new Color (0.235f, 0.360f, 0.580f);
        private readonly Color OUTLINE_COLOR = new Color (0.15f, 0.15f, 0.15f, 1f);

        public PrefixTab() : base ("Prefixes", "d_InputField Icon")
            {
            prefixes = serializedSettings.FindProperty ("prefixes");
            prefixList = new ReorderableList (serializedSettings, prefixes)
                {
                drawHeaderCallback = DrawPrefixHeader,

                drawElementCallback = DrawPrefixElements,
                drawElementBackgroundCallback = DrawPrefixBackground,

                drawFooterCallback = DrawPrefixFooter,

                elementHeightCallback = GetPropertyHeight,

                headerHeight = 12f,

                showDefaultBackground = false,
                draggable = false
                };
            }

        #region Reorderable List

        private void DrawPrefixHeader(Rect rect)
            {
            rect.y += 2f;

            Rect buttonRect = rect;
            buttonRect.width = rect.width*0.5f;

            // Draw optionals
            if (GUI.Button (buttonRect, "Expand All", EditorStyles.centeredGreyMiniLabel))
                {
                SerializedProperty listProperty = prefixList.serializedProperty;
                for (int i = 0; i < prefixList.count; ++i)
                    listProperty.GetArrayElementAtIndex (i).isExpanded = true;
                }

            buttonRect.x += buttonRect.width;
            if (GUI.Button (buttonRect, "Hide All", EditorStyles.centeredGreyMiniLabel))
                {
                SerializedProperty listProperty = prefixList.serializedProperty;
                for (int i = 0; i < prefixList.count; ++i)
                    listProperty.GetArrayElementAtIndex (i).isExpanded = false;
                }
            }

        private void DrawPrefixElements(Rect rect, int index, bool isActive, bool isFocused)
            {
            SerializedProperty prefix = prefixes.GetArrayElementAtIndex (index);

            Rect removeRect = rect;
            removeRect.x += removeRect.width - 32f;
            removeRect.width = 32f;
            removeRect.height = 18f;

            if (GUI.Button (removeRect, "-", Style.listControlStyle))
                {
                Undo.RecordObject (settings, "Removed prefix " + prefix.displayName);
                prefixes.DeleteArrayElementAtIndex (index);
                serializedSettings.ApplyModifiedProperties ();
                return;
                }

            EditorGUI.PropertyField (rect, prefix, prefix.isExpanded);
            }

        private void DrawPrefixBackground(Rect rect, int index, bool isActive, bool isFocused)
            {
            //DrawBackground (rect, isFocused);
            }

        private void DrawPrefixFooter(Rect rect)
            {
            rect.y -= 4f;

            Rect buttonRect = rect;
            buttonRect.x -= 3f;
            buttonRect.width += 7f;

            // Draw optionals
            if (GUI.Button (buttonRect, "+", Style.listControlStyle))
                {
                Undo.RecordObject (settings, "Added new prefix");
                prefixes.InsertArrayElementAtIndex (prefixes.arraySize);
                serializedSettings.ApplyModifiedProperties ();
                }
            }

        private float GetPropertyHeight(int index)
            {
            return GetPropertyHeight (prefixes.GetArrayElementAtIndex (index));
            }

        private float GetPropertyHeight(SerializedProperty property)
            {
            return EditorGUI.GetPropertyHeight (property);
            }

        private void DrawBackground(Rect rect, bool isFocused)
            {
            rect.x -= 16f;
            rect.width += 16f;

            EditorGUI.LabelField (rect, "", GUI.skin.box);

            rect.x += 16f;
            rect.width -= 16f;

            Color fill = (isFocused) ? BACKGROUND_COLOR : Color.clear;

            Handles.BeginGUI ();
            Handles.DrawSolidRectangleWithOutline (rect, Color.clear, OUTLINE_COLOR);
            Handles.EndGUI ();
            }

        #endregion

        /// <summary>
        /// The title gui drawn, primarily to display a header of some form
        /// </summary>
        protected override void OnTitleGUI()
            {
        
            }

        /// <summary>
        /// The main content area for the settings
        /// </summary>
        protected override void OnContentGUI()
            {
            prefixList.DoLayoutList ();
            }
        }
    }
