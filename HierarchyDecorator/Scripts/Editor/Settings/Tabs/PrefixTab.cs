using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace HierarchyDecorator
{
    internal class PrefixTab : SettingsTab
    {
        private ReorderableList prefixList;
        private SerializedProperty prefixes;

        private readonly Color BACKGROUND_COLOR = new Color (0.235f, 0.360f, 0.580f);
        private readonly Color OUTLINE_COLOR = new Color (0.15f, 0.15f, 0.15f, 1f);

        private readonly string[] guiStyleNames;
        private string[] modes = new string[] { "Light Mode", "Dark Mode" };

        public PrefixTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "Prefixes", "d_InputField Icon")
        {
            prefixes = serializedSettings.FindProperty ("prefixes");
            prefixList = new ReorderableList (serializedSettings, prefixes)
            {
                drawHeaderCallback = DrawPrefixHeader,

                drawElementCallback = DrawPrefixElements,
                drawElementBackgroundCallback = DrawPrefixBackground,
                drawNoneElementCallback = DrawNoElements,

                drawFooterCallback = DrawPrefixFooter,

                elementHeightCallback = GetPropertyHeight,

                headerHeight = 12f,

                showDefaultBackground = false,
                draggable = false
            };

            int styleLen = settings.styles.Count;

            guiStyleNames = new string[styleLen];
            for (int i = 0; i < styleLen; i++)
            {
                guiStyleNames[i] = settings.styles[i].name;
            }
        }

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

        #region Reorderable List

        private void DrawPrefixHeader(Rect rect)
        {
            Rect buttonRect = rect;
            buttonRect.width = rect.width * 0.5f;

            // Height of header is 10px, need to expand it
#if UNITY_2019_1_OR_NEWER
            buttonRect.y += 2f;
            GUIStyle style = EditorStyles.centeredGreyMiniLabel;
#else       
            buttonRect.y -= 6f;
            buttonRect.height = 15f;

            GUIStyle style = EditorStyles.centeredGreyMiniLabel;
#endif

            // Draw optionals
            if (GUI.Button (buttonRect, "Expand All", style))
            {
                SerializedProperty listProperty = prefixList.serializedProperty;
                for (int i = 0; i < prefixList.count; ++i)
                {
                    listProperty.GetArrayElementAtIndex (i).isExpanded = true;
                }
            }

            buttonRect.x += buttonRect.width;
            if (GUI.Button (buttonRect, "Hide All", style))
            {
                SerializedProperty listProperty = prefixList.serializedProperty;
                for (int i = 0; i < prefixList.count; ++i)
                {
                    listProperty.GetArrayElementAtIndex (i).isExpanded = false;
                }
            }
        }

        private void DrawPrefixElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= prefixes.arraySize)
            {
                return;
            }

            SerializedProperty prefix = prefixes.GetArrayElementAtIndex (index);

            Rect removeRect = rect;
            removeRect.x += removeRect.width - 32f;
            removeRect.width = 32f;
            removeRect.height = 18f;

            if (GUI.Button (removeRect, "-", Style.ListControlStyle))
            {
                prefixes.DeleteArrayElementAtIndex (index);
                serializedSettings.ApplyModifiedProperties ();

                return;
            }

            EditorGUI.BeginChangeCheck ();
            {
                SerializedProperty prefixProp = prefix.FindPropertyRelative ("prefix");
                SerializedProperty nameProp = prefix.FindPropertyRelative ("name");

                string title = string.Format ("{0} {1}", prefixProp.stringValue, nameProp.stringValue);
                float foldoutSize = 19f;

                rect.x += 12f;
                rect.width -= 12f;

                Rect headerRect = rect;
                headerRect.height = foldoutSize;

                prefix.isExpanded = EditorGUI.Foldout (headerRect, prefix.isExpanded, title, true);

                if (prefix.isExpanded)
                {
                    rect.y += 21f;

                    var customDraws = new Dictionary<string, Action<Rect, SerializedProperty>> ()
                    {
                        { "modes", DrawModes }
                    };

                    SerializedPropertyUtility.DrawChildrenProperties (rect, prefix, prefix.isExpanded, customDraws);
                }

            }
            if (EditorGUI.EndChangeCheck ())
            {
                serializedSettings.ApplyModifiedProperties ();
                serializedSettings.Update ();
            }
        }

        private void DrawPrefixBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            //DrawBackground (rect, isFocused);
        }

        private void DrawPrefixFooter(Rect rect)
        {
            Rect buttonRect = rect;
            buttonRect.x -= 3f;
            buttonRect.width += 7f;

#if UNITY_2019_1_OR_NEWER
            buttonRect.y -= 4f;
#else
            buttonRect.y -= 8f;
            buttonRect.height = 21f;
#endif
            
            // Draw optionals
            if (GUI.Button (buttonRect, "+", Style.ListControlStyle))
            {
                Undo.RecordObject (settings, "Added new prefix");
                settings.prefixes.Add (new PrefixSettings ());
                serializedSettings.Update ();
            }
        }


        private void DrawNoElements(Rect rect)
        {
            EditorGUI.LabelField (rect, "No Prefixes to display.");
        }

        private float GetPropertyHeight(int index)
        {
            if (prefixes == null || prefixes.arraySize == 0)
            {
                return 0;
            }

            if (index >= prefixes.arraySize)
            {
                return 0;
            }

            SerializedProperty property = prefixes.GetArrayElementAtIndex (index);
            return GetPropertyHeight (property);
        }

        private float GetPropertyHeight(SerializedProperty property)
        {
            bool isExpanded = property.isExpanded;

            // Remove a single line due to the drawing of theme settings
            return EditorGUI.GetPropertyHeight (property, isExpanded) - (isExpanded ? 19f : -1f);
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

#endregion Reorderable List

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
    }
}