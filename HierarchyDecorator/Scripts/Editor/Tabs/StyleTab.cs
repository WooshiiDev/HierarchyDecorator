using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using static UnityEngine.GraphicsBuffer;

namespace HierarchyDecorator
{
    [RegisterTab(1)]
    public class StyleTab : SettingsTab
    {
        // Readonly 

        private const float ELEMENT_X_OFFSET = 4f;
        private const float ELEMENT_WIDTH_MARGIN = 19f;
        private const float ELEMENT_WIDTH_PADDING = 20f;

        private const float ELEMENT_HEIGHT_SPACING = 1f;
        private const float ELEMENT_HEIGHT_OFFSET = 4f;

        // References

        private ReorderableList styleList;
        private SerializedProperty serializedStyles;

        private readonly Color BACKGROUND_COLOR = new Color (0.235f, 0.360f, 0.580f);
        private readonly Color OUTLINE_COLOR = new Color (0.15f, 0.15f, 0.15f, 1f);

        private readonly GUIContent[] Modes = { new GUIContent("Light Mode"), new GUIContent("Dark Mode") };
        private readonly string[] SettingList = { "prefix", "noSpaceAfterPrefix", "name", "font", "fontSize", "fontStyle", "fontAlignment",  "textFormatting" };

        public StyleTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "styleData", "Visual", "d_InputField Icon")
        {
            serializedStyles = serializedTab.FindPropertyRelative ("styles");

            styleList = new ReorderableList (serializedSettings, serializedStyles, true, false, false, false)
            {
                drawHeaderCallback = DrawHeader,
                drawFooterCallback = DrawFooter,

                drawElementCallback = DrawElement,
                drawElementBackgroundCallback = DrawElementBackground,

                //drawNoneElementCallback = DrawNoElements,

                elementHeightCallback = GetPropertyHeight,

                headerHeight = 0,
                footerHeight = 19f,

                showDefaultBackground = false,
                
            };

            // Fix for Unity 2021.X for buggy unexpanded list elements
#if UNITY_2021_1_OR_NEWER
            for (int i = 0; i < serializedStyles.arraySize; i++)
            {
                var style = serializedStyles.GetArrayElementAtIndex (i);
                var temp = style.isExpanded;
                style.isExpanded = false;
            }
#endif
            SerializedProperty darkModeBack = serializedTab.FindPropertyRelative("darkMode");
            SerializedProperty lightModeBack = serializedTab.FindPropertyRelative("lightMode");

            CreateDrawableGroup("Background")
                .RegisterSerializedProperty(serializedTab, "twoToneBackground")
                .RegisterSerializedGroup(darkModeBack, "Dark Mode", "colorOne", "colorTwo")
                .RegisterSerializedGroup(lightModeBack, "Light Mode", "colorOne", "colorTwo");

            CreateDrawableGroup ("Styles")
                .RegisterSerializedProperty(serializedTab, "displayTags", "displayLayers", "displayIcons")
                .RegisterReorderable (styleList);
        }

        // Reorderable List GUI

        private void DrawHeader(Rect rect) { }

        private void DrawFooter(Rect rect)
        {
            rect = GetReorderableFooterRect (rect);

            Handles.BeginGUI ();
            Handles.DrawSolidRectangleWithOutline (rect, Color.clear, OUTLINE_COLOR);
            Handles.EndGUI ();

            // Draw optionals
            if (GUI.Button (rect, "Add New Style", Style.CenteredBoldLabel))
            {
                CreateNewStyle();
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= serializedStyles.arraySize)
            {
                return;
            }

            SerializedProperty styleSettings = serializedStyles.GetArrayElementAtIndex (index);

            // Draw header that includes the style

            Rect titleRect = GetElementHeaderRect (rect);
            Rect tileStyleRect = GetElementStyleRect(rect);

            SerializedProperty nameProp = styleSettings.FindPropertyRelative ("name");
            HierarchyGUI.DrawHierarchyStyle (settings.styleData[index], tileStyleRect, titleRect, nameProp.stringValue, false);

            // Draw foldout

            Rect foldoutRect = GetFoldoutRect(rect);

            EditorGUI.BeginChangeCheck ();
            {
                styleSettings.isExpanded = GUI.Toggle (foldoutRect, styleSettings.isExpanded, GUIContent.none, Style.Toggle);

                // Draw style settings

                Rect settingsRect = GetElementBoxRect (rect);
                if (styleSettings.isExpanded)
                {
                    // Draw the background

                    GUI.Box (settingsRect, "");

                    // Add some padding to the rect to fit it into the box

                    Rect propertyRect = GetElementPropertyRect (rect);
                    propertyRect = DrawPropertyList (propertyRect, styleSettings, SettingList);
                    propertyRect.y += propertyRect.height;

                    DrawModes (propertyRect, styleSettings);
                }

                // Draw Deletion

                Rect deletionRect = GetElementDeleteRect (rect);
             
                GUIContent content = new GUIContent ("X");
                if (GUI.Button(deletionRect, content, Style.CenteredBoldLabel))
                {
                    serializedStyles.DeleteArrayElementAtIndex (index);
                    return;
                }
            }

            // Save settings and update the GUIStyle

            if (EditorGUI.EndChangeCheck ())
            {
                settings.styleData[index].UpdateStyle (EditorGUIUtility.isProSkin);
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = GetElementSpacerRect (rect);
            GUIHelper.LineSpacer (rect, OUTLINE_COLOR);

            rect.y += rect.height;
            GUIHelper.LineSpacer (rect, OUTLINE_COLOR);
        }

        private void DrawNoElements(Rect rect)
        {
            EditorGUI.LabelField (rect, "No styles to display.");
        }

        private void CreateNewStyle()
        {
            HierarchyStyle style = new HierarchyStyle();
            style.name = "New Style";
            style.UpdateStyle(EditorGUIUtility.isProSkin);

            settings.styleData.styles.Add(style);
        }

        // Height Calculation

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

            return GetPropertyHeight (serializedStyles.GetArrayElementAtIndex (index));
        }

        private float GetPropertyHeight(SerializedProperty property)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight + 1;
            }

            // All settings listed equally spaced with padding
            int listCount = (SettingList.Length + 1);
            int modeCount = 4;

            return (listCount + modeCount) * (EditorGUIUtility.singleLineHeight + ELEMENT_HEIGHT_SPACING) + EditorGUIUtility.singleLineHeight/2;
        }

        // Property Display

        private Rect DrawPropertyList(Rect rect, SerializedProperty property, params string[] propertyNames)
        {
            if (propertyNames == null)
            {
                Debug.LogWarning ($"Attempt to draw null child properties of {property.name} ");
                return rect;
            }

            // Iterate over properties and draw

            float height = 0;
            for (int i = 0; i < propertyNames.Length; i++)
            {
                SerializedProperty prop = property.FindPropertyRelative (propertyNames[i]);

                if (prop == null)
                {
                    Debug.LogWarning ($"Cannot find child property of property {prop.name}.");
                    continue;
                }

                rect.height = EditorGUI.GetPropertyHeight (prop);
                EditorGUI.PropertyField (rect, prop, true);
                rect.y += EditorGUIUtility.singleLineHeight + ELEMENT_HEIGHT_SPACING;

                height += rect.height;
            }

            // Get full rect size to return

            rect.y -= height;
            rect.height = height;

            return rect;
        }

        public void DrawModes(Rect rect, SerializedProperty property)
        {
            SerializedProperty modeProperty = property.FindPropertyRelative ("modes");

            if (modeProperty == null)
            {
                Debug.LogWarning ($"Cannot find modes property in property {property.name}. Has the correct property been assigned?");
                return;
            }

            modeProperty.isExpanded = true;

            rect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty lightMode = modeProperty.GetArrayElementAtIndex (0);
            SerializedProperty darkMode = modeProperty.GetArrayElementAtIndex (1);

            // Make sure that only one is selected if the case that both have been 'expanded'

            if (lightMode.isExpanded == darkMode.isExpanded)
            {
                bool isPro = EditorGUIUtility.isProSkin;

                lightMode.isExpanded = !isPro;
                darkMode.isExpanded = isPro;
            }

            // Draw selection and toggle mode selected

            EditorGUI.BeginChangeCheck ();

            int selected = lightMode.isExpanded ? 0 : 1;
            int selection = GUI.SelectionGrid (rect, selected, Modes, 2, Style.CenteredBoldLabel);

            rect.y += EditorGUIUtility.singleLineHeight + ELEMENT_HEIGHT_SPACING;

            SerializedProperty prop = modeProperty.GetArrayElementAtIndex (selection);
            EditorGUI.PropertyField (rect, prop, Modes[selection], true);

            if (EditorGUI.EndChangeCheck ())
            {
                lightMode.isExpanded = selection == 0;
                darkMode.isExpanded = !lightMode.isExpanded;
            }
        }

        // Rect Helpers

        private Rect GetElementBaseRect(Rect rect)
        {
            rect.x += ELEMENT_X_OFFSET;
            rect.width -= 26f;

            return rect;
        }

        private Rect GetElementBoxRect(Rect rect)
        {
            rect = GetElementBaseRect (rect);

            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height -= EditorGUIUtility.singleLineHeight - 2f;

            return rect;
        }

        // --- Style Header

        private Rect GetElementStyleRect(Rect rect)
        {
            rect = GetElementBaseRect (rect);
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y++;

            return rect;
        }

        private Rect GetElementHeaderRect(Rect rect)
        {
            // Label uses the same rect, but shrinks height to a single line, and adds width padding
            rect = GetElementStyleRect (rect);
         
            rect.width -= ELEMENT_WIDTH_PADDING * 2f;
            rect.x += ELEMENT_WIDTH_PADDING;

            return rect;
        }

        private Rect GetFoldoutRect(Rect rect)
        {
            rect = GetElementStyleRect (rect);
            rect.y++;

            return rect;
        }

        // --- Footer

        private Rect GetReorderableFooterRect(Rect rect)
        {
            float centreOffset = rect.width / 2 ;

            rect.y -= 3f;
            rect.x += centreOffset/2;
            rect.width = centreOffset;

            return rect;
        }

        // --- Style Elements

        private Rect GetElementModeRect(Rect rect)
        {
            rect = GetElementBaseRect (rect);
            rect.height = EditorGUIUtility.singleLineHeight;

            return rect;
        }

        private Rect GetElementPropertyRect(Rect rect)
        {
            rect = GetElementBoxRect (rect);

            rect.x += ELEMENT_WIDTH_MARGIN;
            rect.width -= ELEMENT_WIDTH_MARGIN * 2f;

            rect.y += ELEMENT_HEIGHT_OFFSET;

            return rect;
        }

        private Rect GetElementSpacerRect(Rect rect)
        {
            rect.x -= 4f;
            rect.width += 8f;

            return rect;
        }

        private Rect GetElementDeleteRect(Rect rect)
        {
            rect = GetElementStyleRect (rect);
        
            float newWidth = rect.x;

            rect.x += rect.width;
            rect.width = newWidth;

            return rect;
        }
    }
}