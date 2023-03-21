using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
{
    [RegisterTab(2)]
    public class IconTab : SettingsTab
    {
        private static class LabelContents
        {
            public static readonly GUIContent MoveUp = new GUIContent("Move Up");
            public static readonly GUIContent MoveDown = new GUIContent("Move Down");
            public static readonly GUIContent DeleteGroup = new GUIContent("Delete Group");
        }

        private static class Labels
        {
            // --- Title

            public const string TITLE = "Icon Selection";

            // --- Controls

            public const string SHOW_ALL_LABEL = "Show All";

            // --- Sidebar group names

            public const string EXCLUDED_COMPONENTS_LABEL = "Excluded";
            public const string ALL_COMPONENTS_LABEL = "All";
            public const string CUSTOM_COMPONENTS_LABEL = "Custom";

            // --- Unity component enable/disable all

            public const string ENABLE_LABEL = "Enable All";
            public const string DISABLE_LABEL = "Disable All";

            // --- Custom group labels

            public const string ADD_GROUP_LABEL = "Add Group";
            public const string DELETE_GROUP_LABEL = "Delete";

            public const string DEFAULT_GROUP_LABEL = "Unnamed Group";

            // --- Custom component labels

            public const string ADD_COMPONENT_LABEL = "Add Icon";
            public const string DELETE_COMPONENT_LABEL = "X";
        }

        private static class Values
        {
            /// <summary>
            /// 
            /// </summary>
            public const float COLUMN_WIDTH_OFFSET = 100f;

            /// <summary>
            /// 
            /// </summary>
            public const float SINGLE_ICON_SIZE = 32f;

            /// <summary>
            /// 
            /// </summary>
            public static float TOOLBAR_HEIGHT => 21f;

            public static float ICON_WINDOW_HEIGHT = 400f;

            public static float CUSTOM_TOOLBAR_WIDTH = 128f;
        }

        private static class Icons
        {
            public static readonly GUIContent Menu = EditorGUIUtility.IconContent("d__Popup");
            public static readonly GUIContent Picker = EditorGUIUtility.IconContent("d_Button Icon");

            public static readonly GUIContent AddComponent = EditorGUIUtility.IconContent("d_Toolbar Plus");
            public static readonly GUIContent DeleteComponent = EditorGUIUtility.IconContent("d_TreeEditor.Trash");

            public static readonly GUIContent EnableAll = EditorGUIUtility.IconContent("d_scenevis_visible_hover@2x");
            public static readonly GUIContent DisableAll = EditorGUIUtility.IconContent("d_SceneViewVisibility@2x");

#if UNITY_2019_1_OR_NEWER
            private static readonly GUIContent WarningGUI = new GUIContent(EditorGUIUtility.IconContent("warning"));
#else
            private static readonly GUIContent WarningGUI = EditorGUIUtility.IconContent ("console.warnicon");
#endif

            public static readonly GUIContent EmptyComponent = new GUIContent("<No Component>", WarningGUI.image);
        }

        // Const/Readonly

        private readonly GUIContent ELLIPSIS_CONTENT = new GUIContent("...");

        private readonly string[] INVALID_ASSET_TYPES =
        {
            null,
            "d_DefaultAsset Icon",
            "DefaultAsset Icon"
        };

        // GUI Control

        private bool isOnCustom;
        private int selectedComponentIndex;

        private string searchText = "";
        private int categoryIndex;

        private float columnWidth;

        private Vector2 scroll;

        private readonly GUIContent[] toolbarContent = new GUIContent[]
        {
            Icons.AddComponent,
            Icons.EnableAll,
            Icons.DisableAll,
            Icons.Menu
        };

        // References

        private ComponentData components;

        private readonly SerializedProperty[] SerializedUnityGroups;
        private SerializedProperty[] SerializedCustomGroups;

        // Component Data

        private string[] groupNames = new string[0];
        private Dictionary<string, IconInfo[]> unityGroups = new Dictionary<string, IconInfo[]>();

        private Rect windowRect;

        // Component GUI

        private GUIContent componentLabel = new GUIContent();
        private GUIContent componentIcon = new GUIContent();

        // Custom group GUI 

        private bool performDrag = false;

        private ComponentGroup hoveredGroup;
        private Rect hoveredRect;

        private List<MonoScript> selectedScripts = new List<MonoScript>();

        private EnumFlagToggleDrawer<DisplayMode> display;

        // Properties
        private Event Event => Event.current;

        // Constructor

        public IconTab(Settings settings, SerializedObject serializedSettings) : base(settings, serializedSettings, "components", "Icons", "d_FilterByType")
        {
            // Setup References
            components = settings.Components;

            SerializedUnityGroups = GetSerializedArrayElements("unityGroups");
            SerializedCustomGroups = GetSerializedArrayElements("customGroups");

            CreateUnityGroups();

            // Register Groups

            SerializedProperty displayProp = serializedTab.FindPropertyRelative("showAll");
            display = new EnumFlagToggleDrawer<DisplayMode>(displayProp);
            display.ToggleStyle = Style.ToolbarButtonLeft;

            CreateDrawableGroup("Settings")
                .RegisterSerializedProperty(serializedTab, "enableIcons", "stackMonoBehaviours", "showMissingScriptWarning");
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

                // If there are no registered icons, group is not required, skip over

                if (icons.Length == 0)
                {
                    continue;
                }

                // Add group and icons to collections

                string name = group.Name;

                names.Add(name);
                unityGroups.Add(name, icons);

                // Add icons to global collection

                allIcons.AddRange(icons);
            }

            // Pass over all icons to group

            unityGroups.Add(Labels.ALL_COMPONENTS_LABEL, allIcons.ToArray());

            // Excluded group

            SerializedProperty excludedProp = serializedTab.FindPropertyRelative("excludedComponents").FindPropertyRelative("components");
            IconInfo[] excluded = GetIconsFromGroup(components.ExcludedComponents, excludedProp);

            unityGroups.Add("Excluded", excluded);

            // Store group names - Yes this is gross but shh

            names.Add("");
            names.Add("");
            groupNames = names.ToArray();
            Array.Sort(groupNames);
            
            // Assign global group to 'All'

            groupNames[0] = Labels.EXCLUDED_COMPONENTS_LABEL;
            groupNames[1] = Labels.ALL_COMPONENTS_LABEL;

        }

        private IconInfo[] GetIconsFromGroup(ComponentGroup group, SerializedProperty serializedGroup)
        {
            List<IconInfo> icons = new List<IconInfo>();

            for (int i = 0; i < group.Count; i++)
            {
                ComponentType component = group.Get(i);
                SerializedProperty serializedComponent = serializedGroup.GetArrayElementAtIndex(i);

                // Check the content validity - we cannot use default or internal types as they're not exposed for use

                if (!component.IsValid() || Array.IndexOf(INVALID_ASSET_TYPES, component.Content.image.name) != -1)
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
            // Draw Content 

            HierarchyGUI.Space();
            EditorGUILayout.LabelField(Labels.TITLE, Style.BoxHeader, GUILayout.Height(19f));
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
            // Disable the tabs if we're searching

            float height = isOnCustom ? Values.ICON_WINDOW_HEIGHT : 300f;

            Rect rect = EditorGUILayout.BeginVertical(Style.BoxHeader, GUILayout.MaxWidth(71f), GUILayout.MinHeight(height));
            {
                display.OnDraw();

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.LabelField("Groups", Style.CenteredLabel, GUILayout.MinWidth(0));

                int index = categoryIndex;
                for (int i = 0; i < groupNames.Length; i++)
                {
                    string name = groupNames[i];

                    if (GUILayout.Toggle(index == i, name, Style.ToolbarButtonLeft))
                    {
                        index = i;
                    }
                }

                //int index = GUILayout.SelectionGrid(categoryIndex, groupNames, 1, Style.ToolbarButtonLeft);
                bool isCustom = GUILayout.Toggle(isOnCustom, Labels.CUSTOM_COMPONENTS_LABEL, Style.ToolbarButtonLeft);

                if (EditorGUI.EndChangeCheck())
                {
                    if (index != categoryIndex && index >= 0)
                    {
                        categoryIndex = index;
                        isOnCustom = false;
                    }
                    else
                    if (isCustom)
                    {
                        isOnCustom = true;
                        categoryIndex = -1;
                    }
                }
            }
            GUILayout.FlexibleSpace();

            DrawBorder(rect);

            EditorGUILayout.EndVertical();
        }

        private void DrawComponents()
        {
            string group = "";
            if (categoryIndex != -1)
            {
                group = groupNames[categoryIndex];
            }

            EditorGUI.BeginDisabledGroup(settings.Components.DisplayAll && group != "Excluded");
            windowRect = EditorGUILayout.BeginVertical();
            {
                // Filter components from search

                if (IsSearching())
                {
                    DrawFilteredComponents(searchText);
                }
                else // Draw custom component GUI if selected
                if (isOnCustom)
                {
                    DrawCustomGroups();
                }
                else // Draw the group if selected
                if (categoryIndex < groupNames.Length)
                {
                    DrawComponentsColumns(unityGroups[group]);
                }

                DrawBorder(windowRect);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawGroupHeader(IEnumerable<IconInfo> icons)
        {
            // Draw search field

            EditorGUILayout.BeginHorizontal();

            searchText = EditorGUILayout.TextField(searchText, Style.ToolbarTextField);

            if (GUILayout.Button(Labels.ENABLE_LABEL, Style.ToolbarButtonResizable))
            {
                foreach (IconInfo icon in icons)
                {
                    SerializedProperty shown = icon.Property.FindPropertyRelative("shown");
                    shown.boolValue = true;
                }
            }

            if (GUILayout.Button(Labels.DISABLE_LABEL, Style.ToolbarButtonResizable))
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

            componentLabel.text = text;
            componentIcon.image = content.image;

            componentLabel = GetComponentDisplayName(componentLabel, labelWidth);

            // Offset to take into account free-positioned toggle

            if (componentIcon.image != null)
            {
                EditorGUILayout.LabelField(componentIcon, Style.ComponentIconStyle, GUILayout.Width(16f));
            }

            EditorGUILayout.LabelField(componentLabel, GUILayout.Width(labelWidth));
        }

        private void DrawBorder(Rect rect)
        {
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, new Color(0.125f, 0.125f, 0.125f));
            Handles.EndGUI();
        }

        // --- Components

        private void DrawComponentsColumns(IconInfo[] types)
        {
            // Draw header 

            DrawGroupHeader(types);

            // For any reason we have no types, return out

            if (types.Length <= 0)
            {
                return;
            }

            // Draw Columns

            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.BeginHorizontal();
            {
                // Get lengths, including the length of a single column

                int len = types.Length;
                int halfLen = len / 2;

                if (len % 2 == 0)
                {
                    halfLen--;
                }

                columnWidth = EditorGUIUtility.currentViewWidth / 2 - Values.COLUMN_WIDTH_OFFSET;

                EditorGUILayout.BeginVertical();
                {
                    for (int i = 0; i < len; i++)
                    {
                        IconInfo type = types[i];

                        // Draw component

                        DrawComponent(type);

                        // Change to second column when first half of the components have been drawn

                        if (i == halfLen)
                        {
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                        }
                    }

                    HierarchyGUI.Space();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        private void DrawFilteredComponents(string filter)
        {
            if (isOnCustom)
            {
                GUI.FocusControl(string.Empty);
                searchText = string.Empty;
                return;
            }

            filter = filter.ToLower();

            string group = groupNames[categoryIndex];
            IconInfo[] selectedTypes = unityGroups[group];
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

            DrawComponentsColumns(filteredTypes.ToArray());
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

        // --- Custom Components

        private void DrawCustomGroups()
        {
            windowRect = EditorGUILayout.BeginVertical(Style.ToolbarNoSpace);
            {
                scroll = EditorGUILayout.BeginScrollView(scroll);

                for (int i = 0; i < components.CustomGroups.Length; i++)
                {
                    ComponentGroup group = components.CustomGroups[i];
                    SerializedProperty serializedGroup = SerializedCustomGroups[i];

                    // Should not happen, but just in case something goes wrong

                    if (group == null || serializedGroup == null)
                    {
                        Debug.LogError("Custom component group is null, have custom groups been modified incorrectly?");
                        continue;
                    }

                    // Draw each group individually 

                    DrawCustomGroup(i);
                }

                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                DrawAddGroupButton();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCustomGroup(int index)
        {
            ComponentGroup group = components.CustomGroups[index];
            SerializedProperty serializedGroup = SerializedCustomGroups[index];

            Rect groupRect = EditorGUILayout.BeginVertical();
            {
                serializedGroup.isExpanded = DrawCustomGroupHeader(index, group, serializedGroup.isExpanded);

                if (serializedGroup.isExpanded)
                {
                    int count = group.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i >= group.Count)
                        {
                            break;
                        }

                        DrawCustomComponent(i, group);
                    }
                }
            }
            EditorGUILayout.EndVertical();

            HandleGroupEvents();

            if (hoveredGroup != group && groupRect.Contains(Event.mousePosition))
            {
                hoveredRect = groupRect;
                hoveredGroup = group;
            }
        }

        private bool DrawCustomGroupHeader(int index, ComponentGroup group, bool isExpanded)
        {
            Rect header = EditorGUILayout.BeginHorizontal();
            {
                isExpanded = EditorGUILayout.Toggle(isExpanded, Style.Foldout, GUILayout.Width(16f), GUILayout.Height(15f));

                group.Name = EditorGUILayout.DelayedTextField(group.Name, Style.BoldLabel);

                GUILayout.FlexibleSpace();

                int toolbarIndex = GUILayout.Toolbar(-1, toolbarContent, EditorStyles.toolbarButton);
                switch (toolbarIndex)
                {
                    case 0: // Add component
                        group.AddEmpty();
                        break;

                    case 1:
                        group.SetAllShown(true);
                        break;

                    case 2:
                        group.SetAllShown(false);
                        break;

                    case 3: // Other options
                        ShowCustomGroupMenu(index);
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();

            header.x -= 3f;
            header.width += 3;
            DrawBorder(header);

            return isExpanded;
        }

        private void ShowCustomGroupMenu(int index)
        {
            GenericMenu menu = new GenericMenu();

            // Move up

            GUIContent moveUp = LabelContents.MoveUp;
            GUIContent moveDown = LabelContents.MoveDown;
            GUIContent delete = LabelContents.DeleteGroup;

            if (index != 0)
            {
                menu.AddItem(moveUp, false, () => components.MoveCustomGroup(index, index - 1));
            }
            else
            {
                menu.AddDisabledItem(moveUp, false);
            }

            // Move down

            if (index != settings.Components.CustomGroups.Length - 1)
            {
                menu.AddItem(moveDown, false, () => components.MoveCustomGroup(index, index + 1));
            }
            else
            {
                menu.AddDisabledItem(moveDown, false);
            }

            menu.AddSeparator(string.Empty);

            // Delete group

            menu.AddItem(delete, false, () =>
            {
                components.DeleteCustomGroup(index);
            });

            // Refine menu position

            menu.ShowAsContext();
        }

        private void DrawCustomComponent(int index, ComponentGroup group)
        {
            float deleteWidth = Values.CUSTOM_TOOLBAR_WIDTH / (toolbarContent.Length + 1);
            
            EditorGUILayout.BeginHorizontal();
            {
                ComponentType component = group.Get(index);
                component.Shown = EditorGUILayout.Toggle(component.Shown, GUILayout.Width(16f));

                int controlID = GUIUtility.GetControlID(FocusType.Keyboard);

                GUIContent content = component.IsValid() ? new GUIContent(component.Content) : Icons.EmptyComponent;

                Vector2 iconSize = Vector2.one * 16f;
                EditorGUIUtility.SetIconSize(iconSize);
                if (GUILayout.Button(content, Style.ToolbarButtonLeft))
                {
                    selectedComponentIndex = index;
                    EditorGUIUtility.ShowObjectPicker<MonoScript>(component.Script, false, "", controlID);
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);

                if (selectedComponentIndex != -1)
                {
                    if (EditorGUIUtility.GetObjectPickerControlID() == controlID && selectedComponentIndex == index)
                    {
                        string commandName = Event.current.commandName;
                        if (commandName == "ObjectSelectorUpdated")
                        {
                            MonoScript script = EditorGUIUtility.GetObjectPickerObject() as MonoScript;
                            if (component.Script != script)
                            {
                                component.UpdateType(script);
                            }
                        }
                        else
                        if (commandName == "ObjectSelectorClosed")
                        {
                            selectedComponentIndex = -1;
                        }
                    }
                }

                if (GUILayout.Button(Labels.DELETE_COMPONENT_LABEL, Style.ToolbarButtonResizable, GUILayout.Width(deleteWidth)))
                {
                    group.Remove(component);
                    EditorUtility.SetDirty(settings);
                    serializedSettings.Update();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddGroupButton()
        {
            Rect buttonRect = EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(Labels.ADD_GROUP_LABEL, Style.ToolbarButtonResizable))
            {
                // Create new group

                components.AddCustomGroup(Labels.DEFAULT_GROUP_LABEL);

                // Update settings object

                EditorUtility.SetDirty(settings);
                serializedSettings.Update();

                // Update cached groups

                SerializedCustomGroups = GetSerializedArrayElements("customGroups");
            }
            EditorGUILayout.EndHorizontal();

            DrawBorder(buttonRect);
        }

        private void HandleGroupEvents()
        {
            if (hoveredGroup == null)
            {
                return;
            }

            switch (Event.type)
            {
                // Cache all MonoTypes

                case EventType.DragPerform:
                case EventType.DragUpdated:
                    if (!performDrag)
                    {
                        foreach (var reference in DragAndDrop.objectReferences)
                        {
                            MonoScript script = reference as MonoScript;

                            if (script == null)
                            {
                                continue;
                            }

                            selectedScripts.Add(script);

                        }

                        performDrag = selectedScripts.Count > 0;
                    }
                    break;

                case EventType.DragExited when performDrag:

                    foreach (MonoScript script in selectedScripts)
                    {
                        ComponentType component = new ComponentType(typeof(MonoScript), false);
                        component.UpdateType(script);

                        if (!hoveredGroup.Contains(component) && component.IsValid())
                        {
                            hoveredGroup.Add(component);
                        }
                    }

                    performDrag = false;
                    hoveredGroup = null;
                    selectedScripts.Clear();

                    break;
            }

            if (performDrag)
            {
                Handles.BeginGUI();
                Handles.DrawSolidRectangleWithOutline(GetGroupHoverRect(hoveredRect), Color.clear, Color.white);
                Handles.EndGUI();
            }
        }


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

            contentLen = GetIconContentVisibleLength(content, width - GetIconContentWidth(ELLIPSIS_CONTENT));

            // If text is visible, append an ellipsis to the end

            if (contentLen >= 0)
            {
                content.text = content.text.Substring(0, contentLen) + ELLIPSIS_CONTENT.text;
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

        // --- Custom Component Rects

        private Rect GetGroupHoverRect(Rect rect)
        {
            rect.x -= 2f;
            rect.width++;
            rect.height--;

            return rect;
        }
    }
}