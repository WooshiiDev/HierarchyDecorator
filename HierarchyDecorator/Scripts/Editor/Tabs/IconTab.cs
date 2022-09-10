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
        // Const/Readonly

        private const string GLOBAL_GROUP_NAME = "All";

        private static readonly string[] INVALID_ASSET_TYPES =
        {
            null,
            "d_DefaultAsset Icon",
            "DefaultAsset Icon"
        };

        private readonly static GUIContent EllipsisContent = new GUIContent("...");

        // GUI Control

        private bool isOnCustom;

        private string searchText = "";
        private int groupIndex;

        private float columnWidth;
        private float sidebarWidth;

        // References

        private ComponentData components;

        private readonly SerializedProperty ShowAllProperty;
        private readonly SerializedProperty ShowMissingProperty;

        private readonly SerializedProperty[] SerializedUnityGroups;
        private SerializedProperty[] SerializedCustomGroups;
        
        // Component Data

        private string[] groupNames = new string[0];
        private Dictionary<string, IconInfo[]> unityGroups = new Dictionary<string, IconInfo[]> ();

        // Constructor

        public IconTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "components", "Icons", "d_FilterByType")
        {
            // Setup References
            components = settings.Components;

            ShowAllProperty = serializedSettings.FindProperty ("globalData.showAllComponents");
            ShowMissingProperty = serializedTab.FindPropertyRelative ("showMissingScriptWarning");

            SerializedUnityGroups = GetSerializedArrayElements("unityGroups");
            SerializedCustomGroups = GetSerializedArrayElements("customGroups");

            CreateUnityGroups();

            // Register Groups

            CreateDrawableGroup("Settings")
                .RegisterSerializedProperty (ShowMissingProperty);
        }
        
        // Methods

        // --- Group Initialization

        private void CreateUnityGroups()
        {
            int groupLen = components.UnityGroups.Length;

            // Store the global data for categories and all icons 

            List<string> names = new List<string>();
            List<IconInfo> allIcons = new List<IconInfo>();

            unityGroups = new Dictionary<string, IconInfo[]>();

            for (int i = 0; i < groupLen; i++)
            {
                ComponentGroup group = components.UnityGroups[i];

                // Ignore groups with no components
                
                if (group.Count == 0)
                {
                    continue;
                }

                // Iterate over all icons and add them to the group

                SerializedProperty serializedGroup = SerializedUnityGroups[i];
                IconInfo[] icons = GetIconsFromGroup(group, serializedGroup);

                // If there are no registred icons, group is not required, skip over

                if (icons.Length == 0)
                {
                    continue;
                }

                // Add group and icons to collections

                string name = group.Name;

                names.Add(name);
                unityGroups.Add(name, icons);

                // Add icons to global colection

                allIcons.AddRange(icons);
            }

            // Pass over all icons to group

            unityGroups.Add(GLOBAL_GROUP_NAME, allIcons.ToArray());

            // Store group names

            names.Add("");
            groupNames = names.ToArray();
            Array.Sort(groupNames);
            
            // Assign global group to 'All'

            groupNames[0] = GLOBAL_GROUP_NAME;
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
            HierarchyGUI.Space();

            // Draw Content 

            EditorGUILayout.LabelField ("Icon Selection", Style.BoxHeader);
            HierarchyGUI.Space(4f);

            EditorGUILayout.BeginHorizontal();
            {
                DrawSidebar();
                DrawComponents();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSidebar()
        {
            // Disable the tabs if we're searching

            EditorGUI.BeginDisabledGroup(IsSearching());
            Rect rect = EditorGUILayout.BeginVertical(GUILayout.Width(70f));
            {
                int index;
                bool onCustom;

                // Draw sidebar selection

                EditorGUI.BeginChangeCheck();
                {
                    ShowAllProperty.boolValue = GUILayout.Toggle(ShowAllProperty.boolValue, "Show All", Style.LargeButtonStyle);
                    GUIHelper.LineSpacer();

                    index = GUILayout.SelectionGrid(groupIndex, groupNames, 1, Style.LargeButtonStyle);
                    GUIHelper.LineSpacer();

                    onCustom = GUILayout.Toggle(isOnCustom, "Custom", Style.LargeButtonStyle);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    // Make sure the correct group is selected due to custom being handled individually

                    if (index != groupIndex && index >= 0)
                    {
                        isOnCustom = false;
                        groupIndex = index;
                    }
                    else
                    if (onCustom)
                    {
                        isOnCustom = true;
                        groupIndex = -1;
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            sidebarWidth = rect.width;
        }

        private void DrawComponents()
        {
            EditorGUI.BeginDisabledGroup(ShowAllProperty.boolValue);
            EditorGUILayout.BeginVertical(Style.BoxHeader);
            {
                // Filter components from search

                if (IsSearching())
                {
                    DrawFilteredComponents(searchText);
                }
                else // Draw custom component GUI if selected
                if (isOnCustom)
                {
                    DrawCustomComponents();
                }
                else // Draw the group if selected
                if (groupIndex < groupNames.Length)
                {
                    string group = groupNames[groupIndex];
                    DrawComponentsColumns(unityGroups[group]);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }


        private void DrawGroupHeader(IEnumerable<IconInfo> icons)
        {
            // Draw search fieldf

            searchText = EditorGUILayout.TextField(searchText, Style.ToolbarTextField);

            // Draw toggle buttons for current icons to enable/disable

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

        private void DrawComponentLabel(GUIContent content)
        {
            float labelWidth = columnWidth;

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


        // --- Components

        private void DrawComponentsColumns(IconInfo[] types)
        {
            // For any reason we have no types, return out

            if (types.Length <= 0)
            {
                return;
            }

            // Draw header 

            DrawGroupHeader(types);

            // Draw Columns

            EditorGUILayout.BeginHorizontal ();
            {
                // Get lengths, including the length of a single column

                int len = types.Length;
                int halfLen = len / 2;

                if (len % 2 == 0)
                {
                    halfLen--;
                }

                columnWidth = EditorGUIUtility.currentViewWidth / 2 - 100f;

                EditorGUILayout.BeginVertical ();
                {
                    for (int i = 0; i < len; i++)
                    {
                        IconInfo type = types[i];

                        // Draw component

                        DrawComponent (type);

                        // Change to second column when first half of the components have been drawn

                        if (i == halfLen)
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
            
            IconInfo[] selectedTypes = unityGroups[GLOBAL_GROUP_NAME];
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

        private void DrawComponent(IconInfo icon)
        {
            if (icon.Property == null)
            {
                Debug.Log("IconInfo has null property. Have properties been cached correctly?");
                return;
            }

            // Get properties

            SerializedProperty property = icon.Property;
            SerializedProperty shown = property.FindPropertyRelative("shown");

            Rect rect = EditorGUILayout.BeginHorizontal(GUILayout.Width(columnWidth));
            {
                // Draw Toggle

                rect.y += 3f;
                rect.width = 32f;

                shown.boolValue = EditorGUI.Toggle(rect, shown.boolValue, Style.ToggleMixed);

                HierarchyGUI.Space(16f);

                DrawComponentLabel(icon.Content);

                // Add flexible space to push label to the left

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        // --- Custom Componments

        private void DrawCustomComponents()
        {
            EditorGUILayout.BeginVertical(Style.BoxHeader);
            {
                for (int i = 0; i < components.CustomGroups.Length; i++)
                {
                    ComponentGroup group = components.CustomGroups[i];
                    SerializedProperty serializedGroup = SerializedCustomGroups[i];

                    // Draw the header

                    EditorGUILayout.BeginVertical(Style.BoxHeader);
                    EditorGUILayout.BeginHorizontal(Style.BoxHeader);
                    {
                        DrawCustomGroupHeader(group, serializedGroup);
                        
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
                        DrawCustomGroup(group);
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
            EditorGUILayout.EndVertical();
        }

        private void DrawCustomGroupHeader(ComponentGroup group, SerializedProperty serializedGroup)
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
        }

        private void DrawCustomGroup(ComponentGroup group)
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
                MonoScript script;
                GUIStyle toggleStyle = shown ? Style.Toggle : Style.ToggleMixed;

                EditorGUI.BeginChangeCheck();
                {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    {
                        rect.x -= 2f;
                        rect.y += 2f;
                        rect.width = 32f;

                        shown = EditorGUI.Toggle(rect, GUIContent.none, shown, toggleStyle);

                        int indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel += 1;
                        script = (MonoScript)EditorGUILayout.ObjectField(component.Script, typeof(MonoScript), false, GUILayout.ExpandWidth(true));
                        ;
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

        // --- GUI Elements


        // --- Icon Content

        private GUIContent GetComponentDisplayName(GUIContent content, float width)
        {
            if (string.IsNullOrEmpty(content.text))
            {
                return GUIContent.none;
            }

            // Get current length of content 

            int textLength = content.text.Length;
            int contentLen = GetIconContentVisibleLength(content, width);

            // No real reason to get a substring if the length is fine

            if (contentLen == textLength)
            {
                return content;
            }

            contentLen = GetIconContentVisibleLength(content, width - GetIconContentWidth(EllipsisContent));

            // If text is visible, append an ellipsis to the end

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
            float ratio = width / EditorStyles.label.CalcSize(content).x;

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