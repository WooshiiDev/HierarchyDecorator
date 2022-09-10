using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.ComponentModel;

namespace HierarchyDecorator
{
    [RegisterTab ()]
    public class IconTab : SettingsTab
    {
        // Readonly

        private readonly string[] INVALID_ASSET_TYPES =
        {
            null,
            "d_DefaultAsset Icon",
            "DefaultAsset Icon"
        };

        private readonly static GUIContent EllipsisContent = new GUIContent("...");

        private const string GLOBAL_CATEGORY_NAME = "All";

        // GUI Control

        private string searchText = "";
        private int categoryIndex = 0;

        private float currentColumnWidth = 0;

        private bool isOnCustom;

        // References

        private readonly SerializedProperty ShowAllProperty;
        private readonly SerializedProperty ShowMissingProperty;

        private readonly SerializedProperty[] SerializedUnityGroups;
        private SerializedProperty[] SerializedCustomGroups;

        private ReorderableList customComponentList;

        // Component Data

        private string[] unityNames = new string[0];
        private Dictionary<string, IconInfo[]> unityCategories = new Dictionary<string, IconInfo[]> ();

        private string[] customNames;
        private Dictionary<string, List<IconInfo>> customCategories = new Dictionary<string, List<IconInfo>>();

        private TextAsset bbb;

        // ===============

        private ComponentData components;
            
        // ===============

        // Constructor

        public IconTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "components", "Icons", "d_FilterByType")
        {
            // Setup References
            components = settings.Components;

            ShowAllProperty = serializedSettings.FindProperty ("globalData.showAllComponents");
            ShowMissingProperty = serializedTab.FindPropertyRelative ("showMissingScriptWarning");

            SerializedUnityGroups = GetSerializedArrayElements("unityGroups");
            SerializedCustomGroups = GetSerializedArrayElements("customGroups");

            CreateUnityCategories();
            //CreateCustomCategories();

            // Register Groups

            CreateDrawableGroup("Settings")
                .RegisterSerializedProperty (ShowMissingProperty);

            isOnCustom = true;
        }
        
        // --- Category Initialization

        private void CreateUnityCategories()
        {
            int groupLen = components.UnityGroups.Length;

            // Store the global data for categories and all icons 

            List<string> names = new List<string>();
            List<IconInfo> allIcons = new List<IconInfo>();

            unityCategories = new Dictionary<string, IconInfo[]>();

            for (int i = 0; i < groupLen; i++)
            {
                ComponentGroup group = components.UnityGroups[i];

                // Ignore groups with no components
                
                if (group.Count == 0)
                {
                    continue;
                }

                // Iterate over all icons and add them to the category

                SerializedProperty serializedGroup = SerializedUnityGroups[i];
                IconInfo[] icons = GetIconsFromGroup(group, serializedGroup);

                // If there are no registred icons, category is not required, skip over

                if (icons.Length == 0)
                {
                    continue;
                }

                // Add category and icons to collections

                string name = group.Name;

                names.Add(name);
                unityCategories.Add(name, icons);

                // Add icons to global colection

                allIcons.AddRange(icons);
            }

            // Pass over all icons to category

            unityCategories.Add(GLOBAL_CATEGORY_NAME, allIcons.ToArray());

            // Store category names

            names.Add("");
            unityNames = names.ToArray();
            Array.Sort(unityNames);
            
            // Assign global category to 'All'

            unityNames[0] = GLOBAL_CATEGORY_NAME;
        }

        private IconInfo[] GetIconsFromGroup(ComponentGroup group, SerializedProperty serializedGroup)
        {
            List<IconInfo> icons = new List<IconInfo>();

            for (int i = 0; i < group.Count; i++)
            {
                ComponentType component = group.Get(i);
                SerializedProperty serializedComponent = serializedGroup.GetArrayElementAtIndex(i);
                
                // Check the content validity - we cannot use default or internal types as they're not exposed for use

                GUIContent content = component.Content;

                if (content.image == null || Array.IndexOf(INVALID_ASSET_TYPES, component.Content.image.name) != -1)
                {
                    continue;
                }

                // Register an icon instance

                IconInfo info = new IconInfo(component, serializedComponent);
                icons.Add(info);
            }

            return icons.ToArray();
        }

        // --- GUI

        protected override void OnContentGUI()
        {
            serializedSettings.UpdateIfRequiredOrScript();

            EditorGUILayout.LabelField ("Icon Selection", Style.BoxHeader);
            HierarchyGUI.Space();

            EditorGUILayout.BeginHorizontal();
            {
                DrawSidebar();
                DrawComponents();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSidebar()
        {
            EditorGUI.BeginDisabledGroup(IsSearching());
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(60f));
                {
                    EditorGUI.BeginChangeCheck();

                    ShowAllProperty.boolValue = GUILayout.Toggle(ShowAllProperty.boolValue, "Show All", Style.LargeButtonStyle);
                    GUIHelper.LineSpacer();

                    int index = GUILayout.SelectionGrid(categoryIndex, unityNames, 1, Style.LargeButtonStyle);

                    GUIHelper.LineSpacer();

                    bool toggleBool = GUILayout.Toggle(isOnCustom, "Custom", Style.LargeButtonStyle);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (index != categoryIndex && index >= 0)
                        {
                            isOnCustom = false;
                            categoryIndex = index;
                        }
                        else
                        if (toggleBool)
                        {
                            isOnCustom = true;
                            categoryIndex = -1;
                        }

                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawComponents()
        {
            EditorGUI.BeginDisabledGroup(ShowAllProperty.boolValue);
            EditorGUILayout.BeginVertical();
            {
                if (IsSearching())
                {
                    DrawFilteredComponents(searchText);
                }
                else
                if (isOnCustom)
                {
                    DrawCustomComponents();
                }
                else
                if (categoryIndex < unityNames.Length)
                {
                    categoryIndex = Mathf.Clamp(categoryIndex, 0, unityNames.Length);

                    string category = unityNames[categoryIndex];

                    if (unityCategories.TryGetValue(category, out IconInfo[] icons))
                    {
                        DrawComponentsColumns(icons);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }

        // --- Components

        int aa = 1;
        private void DrawCustomComponents()
        {
            for (int i = 0; i < components.CustomGroups.Length; i++)
            {
                ComponentGroup group = components.CustomGroups[i];
                SerializedProperty serializedGroup = SerializedCustomGroups[i]; 

                // Draw the header

                EditorGUILayout.BeginVertical(Style.BoxHeader, GUILayout.Height(16f));
                {
                    EditorGUILayout.BeginHorizontal(Style.BoxHeader, GUILayout.ExpandHeight(true));
                    {
                        serializedGroup.isExpanded = EditorGUILayout.Toggle(serializedGroup.isExpanded, EditorStyles.foldout, GUILayout.Width(16f), GUILayout.ExpandHeight(true));

                        string name = EditorGUILayout.TextField(group.Name, EditorStyles.boldLabel, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false));

                        if (string.IsNullOrWhiteSpace(name))
                        {
                            name = "Unnamed Group";
                        }
                            
                        group.Name = name;
                        
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Add Icon", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandHeight(true)))
                        {
                            ComponentType component = new ComponentType(typeof(DefaultAsset), false);
                            component.UpdateContent();
                            group.Add(component);

                            EditorUtility.SetDirty(settings);

                            serializedGroup.isExpanded = true;
                        }

                        if (GUILayout.Button("Delete", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandHeight(true)))
                        {
                            components.RemoveCustomGroup(i);
                            i--;

                            EditorUtility.SetDirty(settings);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                   
                    if (serializedGroup.isExpanded && group.Count > 0)
                    {
                        DrawCustomComponents(group);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add New Group"))
            {
                components.AddCustomGroup(new ComponentGroup($"New Group"));
                EditorUtility.SetDirty(settings);

                serializedSettings.Update();
                SerializedCustomGroups = GetSerializedArrayElements("customGroups");
            }

        }

        private void DrawCustomComponents(ComponentGroup group)
        {
            if (group == null)
            {
                Debug.LogError("Cannot draw null group, has it been deleted?");
                return;
            }

            for (int i = 0; i < group.Count; i++)
            {
                ComponentType component = group[i];

                bool shown = component.Shown;
                MonoScript script = component.Script;
                GUIStyle toggleStyle = shown ? Style.Toggle : Style.ToggleMixed;

                EditorGUI.BeginChangeCheck();
                {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    {
                        rect.x-= 2f;
                        rect.y += 2f;
                        rect.width = 32f;

                        shown = EditorGUI.Toggle(rect, GUIContent.none, shown, toggleStyle);

                        int indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel += 1;
                        script = (MonoScript)EditorGUILayout.ObjectField(component.Script, typeof(MonoScript), false, GUILayout.ExpandWidth(true));;
                        EditorGUI.indentLevel = indent;

                        if (GUILayout.Button("X", Style.CenteredBoldLabel, GUILayout.Width(24f), GUILayout.ExpandHeight(true)))
                        {
                            group.Remove(i);
                            EditorUtility.SetDirty(settings);
                            serializedSettings.Update();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    component.Shown = shown;

                    if (script != component.Script)
                    {
                        component.UpdateType(script);
                    }
                }
            }
        }

        private void DrawComponentsColumns(IconInfo[] types)
        {
            DrawCategoryHeader(types);

            if (types.Length <= 0)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal ();
            {
                // Get lengths, including the length of a single column

                int len = types.Length;
                int halfLen = len / 2;

                if (len % 2 == 0)
                {
                    halfLen--;
                }

                EditorGUILayout.BeginVertical ();
                {
                    for (int i = 0; i < len; i++)
                    {
                        if (!isOnCustom)
                        {
                            currentColumnWidth = EditorGUIUtility.currentViewWidth / 2 - 106f;
                        }

                        IconInfo type = types[i];

                        DrawComponent (type);

                        // Second column

                        if (i == halfLen && !isOnCustom)
                        {
                            EditorGUILayout.EndVertical ();
                            EditorGUILayout.BeginVertical ();
                        }
                    }
                }
                EditorGUILayout.EndVertical ();
            }
            EditorGUILayout.EndHorizontal ();
        }

        private void DrawFilteredComponents(string filter)
        {
            filter = filter.ToLower ();
            IconInfo[] selectedTypes = isOnCustom 
                ? customCategories[GLOBAL_CATEGORY_NAME].ToArray()
                : unityCategories[GLOBAL_CATEGORY_NAME];

            List<IconInfo> filteredTypes = new List<IconInfo>();
            for (int i = 0; i < selectedTypes.Length; i++)
            {
                IconInfo type = selectedTypes[i];
                string lowerName = type.Name.ToLower();

                if (lowerName.Contains(filter))
                {
                    filteredTypes.Add(type);
                }
            }

            DrawComponentsColumns (filteredTypes.ToArray());
        }

        private void DrawComponent(IconInfo type)
        {
            if (type.Property == null)
            {
                Debug.Log("IconInfo has null property. Have properties been cached correctly?");
                return;
            }

            SerializedProperty property = type.Property;
            SerializedProperty shown = property.FindPropertyRelative("shown");

            Rect rect = EditorGUILayout.BeginHorizontal(GUILayout.Width(currentColumnWidth));
            {
                // Draw Toggle

                rect.y += 3f;
                rect.width = 32f;

                shown.boolValue = EditorGUI.Toggle(rect, shown.boolValue, Style.ToggleMixed);

                HierarchyGUI.Space(16f);

                DrawComponentLabel(type.Content);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawComponentLabel(GUIContent content)
        {
            float labelWidth = currentColumnWidth;

            string text = content.text;

            GUIContent labelContent = new GUIContent(text);
            GUIContent iconContent = new GUIContent(content.image);

            labelContent = GetComponentDisplayName(labelContent, labelWidth);

            // Offset to take into account free-positioned toggle

            if (iconContent.image != null)
            {
                EditorGUILayout.LabelField(iconContent, Style.ComponentIconStyle, GUILayout.Width(16f));
            }
            
            EditorGUILayout.LabelField(labelContent, GUILayout.Width(labelWidth));
        }

        // --- GUI Elements

        private void DrawCategoryHeader(IEnumerable<IconInfo> types)
        {
            searchText = EditorGUILayout.TextField("", searchText, "ToolbarSeachTextField");
            DrawButtonToggle(types);
        }

        private void DrawButtonToggle(IEnumerable<IconInfo> icons)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Enable All", Style.LargeButtonStyle))
            {
                foreach (IconInfo icon in icons)
                { 
                    SerializedProperty shown = icon.Property.FindPropertyRelative("shown");
                    shown.boolValue = true;
                }
            }

            if (GUILayout.Button("Disable All", Style.LargeButtonStyle))
            {
                foreach (IconInfo icon in icons)
                {
                    SerializedProperty shown = icon.Property.FindPropertyRelative("shown");
                    shown.boolValue = false;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // --- Icon Content

        private GUIContent GetComponentDisplayName(GUIContent content, float width)
        {
            // Get current length of content 

            int textLength = content.text.Length;
            int contentLen = GetIconContentVisibleLength(content, width);

            // No real reason to get a substring if the length is fine

            if (contentLen == textLength)
            {
                return content;
            }

            string originalText = content.text;
            contentLen = GetIconContentVisibleLength(content, width - GetIconContentWidth(EllipsisContent));

            if (contentLen >= 0)
            {
                content.text = content.text.Substring(0, contentLen) + "...";
            }

            return content;
        }

        private int GetIconContentVisibleLength(GUIContent content, float width)
        {
            // Calculate the length difference between the width given and the style size of the content

            int len = content.text.Length;
            float ratio = width / Style.ComponentIconStyle.CalcSize(content).x;

            // Round it to fit into text length

            return Mathf.Min(Mathf.FloorToInt(ratio * len), len);
        }

        private int GetIconContentWidth(GUIContent content)
        {
            return Mathf.CeilToInt(Style.ComponentIconStyle.CalcSize(content).x);
        }

        // --- Utils

        private SerializedProperty[] GetSerializedArrayElements(string propertyName)
        {
            SerializedProperty arrayProperty = serializedTab.FindPropertyRelative(propertyName);
            SerializedProperty[] elements = new SerializedProperty[arrayProperty.arraySize];

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                elements[i] = arrayProperty.GetArrayElementAtIndex(i).FindPropertyRelative("components");
            }

            return elements;
        }
        
        private bool IsSearching()
        {
            return !string.IsNullOrWhiteSpace(searchText);
        }
    }
}