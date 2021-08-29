using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
{
    public class IconTab : SettingsTab
    {
        [Serializable]
        private class IconInfo : IComparable<IconInfo>
        {
            public readonly Type type;
            public SerializedProperty property;

            public IconInfo(Type type, SerializedProperty property)
            {
                this.type = type;
                this.property = property;
            }

            public int CompareTo(IconInfo other)
            {
                return type.FullName.CompareTo (other.type.FullName);
            }
        }

        // References
        private readonly SerializedProperty showAllProperty;
        private readonly SerializedProperty showMissingProperty;

        private readonly SerializedProperty serializedCustomComponents;
        private readonly SerializedProperty serializedUnityComponents;

        private Dictionary<string, List<IconInfo>> componentCategories = new Dictionary<string, List<IconInfo>> ();

        // GUI

        private bool[] categoryFoldout;
        private int selection;

        public IconTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, serializedSettings.FindProperty("componentData"), "Icons", "d_FilterByType")
        {
            // Setup References
            showAllProperty = serializedSettings.FindProperty ("globalData.showAllComponents");
            showMissingProperty = serializedTab.FindPropertyRelative ("showMissingScriptsWarning");

            serializedUnityComponents = serializedTab.FindPropertyRelative ("unityComponents");
            serializedCustomComponents = serializedTab.FindPropertyRelative ("customComponents");

            componentCategories = GetIconCategories ();
        }

        // GUI

        protected override void OnContentGUI()
        {
            float currentViewWidth = EditorGUIUtility.currentViewWidth;

            bool canHaveColumns = currentViewWidth > 500f;
            float horizIconWidth = (currentViewWidth * 0.75f) * 0.45f;

            EditorGUILayout.Space ();

            EditorGUI.BeginChangeCheck ();
            EditorGUILayout.PropertyField (showMissingProperty);
            if (EditorGUI.EndChangeCheck ())
            {
                serializedSettings.ApplyModifiedProperties ();
            }

            EditorGUILayout.Space ();

            EditorGUILayout.BeginHorizontal (GUILayout.MaxWidth (currentViewWidth));
            {
                EditorGUILayout.BeginVertical (Style.TabBackground, GUILayout.Width (currentViewWidth * 0.075F));
                {
                    EditorGUI.BeginChangeCheck ();
                    showAllProperty.boolValue = GUILayout.Toggle (showAllProperty.boolValue, "Show All\nIcons", Style.LargeButtonSmallTextStyle);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedSettings.ApplyModifiedProperties ();
                    }

#if UNITY_2019_1_OR_NEWER
                    EditorGUILayout.Space (6f);
#else
                    EditorGUILayout.Space ();
#endif

                    selection = GUILayout.SelectionGrid (selection, componentCategories.Keys.ToArray (), 1, Style.LargeButtonStyle);
                }
                EditorGUILayout.EndVertical ();

                // Any rare usecases

                EditorGUILayout.BeginVertical (Style.TabBackground);
                {
                    if (showAllProperty.boolValue)
                    {
                        EditorGUILayout.LabelField ("All Icons are shown as \"Show All Icons\" is enabled", Style.CenteredBoldLabel);
                    }

                    EditorGUI.BeginDisabledGroup (showAllProperty.boolValue);

                    KeyValuePair<String, List<IconInfo>> category = componentCategories.ElementAt (selection);
                    List<IconInfo> icons = category.Value;

                    bool hasColumn = false;

                    if (selection == componentCategories.Keys.Count - 1)
                    {
                        DrawGlobalToggles (serializedCustomComponents);

#if UNITY_2019_1_OR_NEWER
                        EditorGUILayout.Space (10f);
#else
                        EditorGUILayout.Space ();
#endif

                        DrawCustomComponents ();
                    }
                    else
                    {
                        DrawGlobalToggles (icons);

#if UNITY_2019_1_OR_NEWER
                        EditorGUILayout.Space (10f);
#else
                        EditorGUILayout.Space ();
#endif
                        EditorGUIUtility.SetIconSize (Vector2.one * 16f);

                        int validIndex = 0;
                        for (int j = 0; j < icons.Count; j++)
                        {
                            IconInfo icon = icons[j];
                            Type iconType = icon.type;

                            GUIContent content = EditorGUIUtility.ObjectContent (null, iconType);
                            content.text = iconType.Name;

                            // Setup group to display two column view of icons
                            hasColumn = validIndex % 2 == 0;

                            EditorGUI.BeginChangeCheck ();

                            GUIHelper.BeginConditionalHorizontal (hasColumn && canHaveColumns);
                            icon.property.boolValue = EditorGUILayout.ToggleLeft (content, icon.property.boolValue, GUILayout.Width (horizIconWidth));
                            GUIHelper.EndConditionHorizontal (!hasColumn && canHaveColumns);

                            if (EditorGUI.EndChangeCheck())
                            {
                                serializedSettings.ApplyModifiedProperties ();
                            }

                            validIndex++;
                        }
                    }

                    EditorGUIUtility.SetIconSize (Vector2.zero);

                    GUIHelper.EndConditionHorizontal (hasColumn && canHaveColumns);

                    EditorGUI.EndDisabledGroup ();
                }
                EditorGUILayout.EndVertical ();
            }
            EditorGUILayout.EndHorizontal ();
        }

        private void DrawCustomComponents()
        {
            if (serializedCustomComponents.arraySize == 0)
            {
                GUILayout.FlexibleSpace ();
                EditorGUILayout.LabelField ("No Custom Components Addded.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                SerializedProperty customType = null;
                SerializedProperty script = null;
                Object scriptValue = null;

                for (int i = 0; i < serializedCustomComponents.arraySize; i++)
                {
                    customType = serializedCustomComponents.GetArrayElementAtIndex (i);
                    script = customType.FindPropertyRelative ("script");
                    scriptValue = script.objectReferenceValue;

                    string displayName = scriptValue == null ? "Empty Custom Element" : scriptValue.name;

                    EditorGUI.indentLevel++;

                    EditorGUI.BeginChangeCheck ();

                    EditorGUILayout.BeginHorizontal (GUI.skin.box);
                    {
                        EditorGUILayout.BeginVertical ();
                        {
                            //Draw all children afterwards
                            SerializedProperty shown = customType.FindPropertyRelative ("shown");

                            EditorGUILayout.BeginHorizontal ();

                            shown.boolValue = EditorGUILayout.Toggle (shown.boolValue, GUILayout.Width (64f));
                            EditorGUILayout.ObjectField (script, new GUIContent ());

                            EditorGUILayout.EndHorizontal ();
                        }
                        EditorGUILayout.EndVertical ();

                        if (GUILayout.Button ("Delete Icon", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandHeight (true)))
                        {
                            serializedCustomComponents.DeleteArrayElementAtIndex (i);
                        }
                    }
                    EditorGUILayout.EndHorizontal ();

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedSettings.ApplyModifiedProperties ();
                    }

                    EditorGUI.indentLevel--;
                }
            }

            GUILayout.FlexibleSpace ();

            // Add Custom Component
            if (GUILayout.Button ("Add Component", Style.LargeButtonStyle))
            {
                serializedCustomComponents.InsertArrayElementAtIndex (serializedCustomComponents.arraySize);
                serializedSettings.ApplyModifiedProperties ();
            }
        }

        private void DrawGlobalToggles(List<IconInfo> components)
        {
            EditorGUILayout.BeginHorizontal ();
            {
                if (GUILayout.Button ("Enable All", Style.LargeButtonStyle))
                {
                    SetVisibilityForAll (components, true);
                }

                if (GUILayout.Button ("Disable All", Style.LargeButtonStyle))
                {
                    SetVisibilityForAll (components, false);
                }
            }
            EditorGUILayout.EndHorizontal ();
        }

        private void DrawGlobalToggles(SerializedProperty componentParent)
        {
            EditorGUILayout.BeginHorizontal ();
            {
                if (GUILayout.Button ("Enable All", Style.LargeButtonStyle))
                {
                    SetVisibilityForAll (componentParent, true);
                }

                if (GUILayout.Button ("Disable All", Style.LargeButtonStyle))
                {
                    SetVisibilityForAll (componentParent, false);
                }
            }
            EditorGUILayout.EndHorizontal ();
        }

        private void DrawCustomFoldout(int index, string name, SerializedProperty customProperty)
        {
            EditorGUILayout.BeginHorizontal ();
            GUIHelper.GetSerializedFoldout (customProperty, name);
            EditorGUILayout.EndHorizontal ();
        }

        private void SetVisibilityForAll(List<IconInfo> components, bool visibility)
        {
            Undo.RecordObject (settings, $"Toggled components to {visibility}.");

            foreach (IconInfo component in components)
            {
                component.property.boolValue = visibility;
            }

            serializedSettings.ApplyModifiedProperties ();
        }

        private void SetVisibilityForAll(SerializedProperty componentParent, bool visibility)
        {
            Undo.RecordObject (settings, $"Toggled components to {visibility}.");

            for (int i = 0; i < componentParent.arraySize; i++)
            {
                SerializedProperty component = componentParent.GetArrayElementAtIndex (i);
                component.FindPropertyRelative ("shown").boolValue = visibility;
            }

            serializedSettings.ApplyModifiedProperties ();
        }

        // Icon Setup

        private Dictionary<string, List<IconInfo>> GetIconCategories()
        {
            Dictionary<string, List<IconInfo>> categories = new Dictionary<string, List<IconInfo>>
            {
                { "General", new List<IconInfo> () }
            };

            List<ComponentType> unityComponents = settings.componentData.unityComponents;

            for (int i = 0; i < unityComponents.Count; i++)
            {
                Type type = unityComponents[i].type;

                // Ignore default icons

                Texture componentImage = EditorGUIUtility.ObjectContent (null, type).image;

                if (componentImage == null || componentImage.name == "d_DefaultAsset Icon" || componentImage.name == "DefaultAsset Icon")
                {
                    continue;
                }

                // Add icon to category

                string categoryName = GetTypeFilter (type);

                if (!componentCategories.ContainsKey (categoryName))
                {
                    componentCategories.Add (categoryName, new List<IconInfo> ());
                }

                IconInfo info = new IconInfo (type, serializedUnityComponents.GetArrayElementAtIndex (i).FindPropertyRelative ("shown"));
                componentCategories[categoryName].Add (info);
            }

            // Sort/Cleanup for better display 
            componentCategories = componentCategories.OrderBy (c => c.Key).ToDictionary (t => t.Key, t => t.Value);

            foreach (List<IconInfo> item in componentCategories.Values)
            {
                item.Sort ();
            }

            componentCategories.Add ("Custom", null);

            return componentCategories;
        }

        private string GetTypeFilter(Type type)
        {
            string categoryName = Constants.ComponentFilters.FirstOrDefault ((f) =>
            {
                Type baseType = null;

                if (f.type == FilterType.TYPE)
                {
                    baseType = Type.GetType (f.filter);
                }

                return (f.type == FilterType.NAME)
                    ? type.FullName.Contains (f.filter)
                    : type.IsAssignableFrom (baseType) || type.IsSubclassOf (baseType);
            }).name;

            if (string.IsNullOrEmpty (categoryName))
            {
                categoryName = "General";
            }

            return categoryName;
        }
    }
}