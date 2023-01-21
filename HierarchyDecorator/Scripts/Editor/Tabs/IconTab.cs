using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
{
    [RegisterTab ()]
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
        private float sidebarWidth;

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

        private readonly SerializedProperty ShowAllProperty;
        private readonly SerializedProperty ShowMissingProperty;

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

        private Vector2 scrollPosition;
        private float customScrollHeight;

        // Properties

        private Event Event => Event.current;

        // Constructor

        public IconTab(Settings settings, SerializedObject serializedSettings) : base(settings, serializedSettings, "components", "Icons", "d_FilterByType")
        {
            // Setup References
            components = settings.Components;

            ShowAllProperty = serializedTab.FindPropertyRelative("showAllComponents");
            ShowMissingProperty = serializedTab.FindPropertyRelative("showMissingScriptWarning");

            SerializedUnityGroups = GetSerializedArrayElements("unityGroups");
            SerializedCustomGroups = GetSerializedArrayElements("customGroups");

            CreateUnityGroups();

            // Register Groups

            CreateDrawableGroup("Settings")
                .RegisterSerializedProperty(ShowAllProperty)
                .RegisterSerializedProperty(ShowMissingProperty);
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

            unityGroups.Add(Labels.ALL_COMPONENTS_LABEL, allIcons.ToArray());

            // Store group names

            names.Add("");
            groupNames = names.ToArray();
            Array.Sort(groupNames);

            // Assign global group to 'All'

            groupNames[0] = Labels.ALL_COMPONENTS_LABEL;
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

            EditorGUI.BeginDisabledGroup(IsSearching());
            Rect rect = EditorGUILayout.BeginVertical(Style.ToolbarNoSpace, GUILayout.Width(70f), GUILayout.MinHeight(height));
            {
                EditorGUI.BeginChangeCheck();

                int index = GUILayout.SelectionGrid(categoryIndex, groupNames, 1, Style.ToolbarButtonLeft);
                isOnCustom = GUILayout.Toggle(isOnCustom, Labels.CUSTOM_COMPONENTS_LABEL, Style.ToolbarButtonLeft);

                if (EditorGUI.EndChangeCheck())
                {
                    if (isOnCustom)
                    {
                        categoryIndex = -1;
                    }
                    else
                    if (index != categoryIndex)
                    {
                        categoryIndex = index;
                        isOnCustom = false;
                    }
                }
            }
            GUILayout.FlexibleSpace();

            DrawBorder(rect);

            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            sidebarWidth = rect.width;
        }

        private void DrawComponents()
        {
            EditorGUI.BeginDisabledGroup(ShowAllProperty.boolValue);
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
                    string group = groupNames[categoryIndex];
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

            EditorGUILayout.BeginHorizontal(Style.ToolbarNoSpace, GUILayout.Height(21f));
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
            filter = filter.ToLower();

            IconInfo[] selectedTypes = unityGroups[Labels.ALL_COMPONENTS_LABEL];
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
                GUILayout.FlexibleSpace();

                float newHeight = 0;

                // Set the height of the group window, but keep space for the add group button at the bottom
                windowRect.height = Values.ICON_WINDOW_HEIGHT - Values.TOOLBAR_HEIGHT + 2;

                // Scroll viewport - customScrollHeight calculated from previous drawn frame 

                Rect scrollViewport = windowRect;
                scrollViewport.position = Vector2.zero;
                scrollViewport.height = customScrollHeight;

                // Overflow/Scroll checks to shift over gui width 

                bool hasOverflow = customScrollHeight > windowRect.height;

                if (hasOverflow)
                {
                    scrollViewport.width -= 13f;
                }

                // Wrap groups in scroll

                scrollPosition = GUI.BeginScrollView(windowRect, scrollPosition, scrollViewport);
                {
                    // Shrink the width if scrollbar exists

                    if (hasOverflow)
                    {
                        windowRect.width -= 13f;
                    }

                    // Create basic group rect with the height set for the toolbar

                    Rect groupRect = scrollViewport;
                    groupRect.height = Values.TOOLBAR_HEIGHT;

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

                        Rect currentRect = DrawCustomGroup(groupRect, i, group, serializedGroup);

                        // Update positioning

                        float currentHeight = currentRect.height;
                        groupRect.y += currentHeight;
                        newHeight += currentHeight;

                        // Draw border to finish off the groups

                        Rect borderRect = groupRect;
                        borderRect.height = 0f;

                        DrawBorder(borderRect);
                    }

                    HandleGroupEvents();
                }

                // End the scroll view that we began above.

                GUI.EndScrollView();
                customScrollHeight = newHeight;


                Rect buttonRect = EditorGUILayout.BeginHorizontal(Style.ToolbarNoSpace);
                if (GUILayout.Button(Labels.ADD_GROUP_LABEL, Style.ToolbarButtonResizable))
                {
                    components.AddCustomGroup("New Group");
                    EditorUtility.SetDirty(settings);

                    serializedSettings.Update();
                    SerializedCustomGroups = GetSerializedArrayElements("customGroups");
                }
                EditorGUILayout.EndHorizontal();
                DrawBorder(buttonRect);
            }
            EditorGUILayout.EndVertical();

            // Assign the scroll height for the next draw call

        }

        private Rect DrawCustomGroup(Rect rect, int customIndex, ComponentGroup group, SerializedProperty serializedGroup)
        {
            Rect fullRect = rect;

            // Draw background

            GUI.Box(rect, GUIContent.none, EditorStyles.toolbar);

            if (DrawCustomGroupHeader(rect, customIndex, group, serializedGroup))
            {
                int count = group.Count;
                for (int i = 0; i < count; i++)
                {
                    // Required here for group size checks

                    if (i >= group.Count)
                    {
                        return rect;
                    }

                    ComponentType component = group.Get(i);
                    DrawCustomComponent(rect, customIndex, group, component);

                    // Deleted component, decrement loop

                    if (component == null)
                    {
                        count--;
                        i--;
                        continue;
                    }

                    rect.y += 20f;
                    fullRect.height += 20f;
                }
            }

            DrawBorder(fullRect);

            if (hoveredGroup != group && fullRect.Contains(Event.mousePosition))
            {
                hoveredRect = fullRect;
                hoveredGroup = group;
            }

            return fullRect;
        }

        private bool DrawCustomGroupHeader(Rect rect, int groupIndex, ComponentGroup group, SerializedProperty serializedGroup)
        {
            // Positioning 

            Rect foldoutRect = GetCustomFoldoutRect(rect);
            Rect labelRect = GetCustomHeaderRect(rect);
            Rect toolbarRect = GetCustomToolbarRect(windowRect, rect);

            // Foldout

            bool toggleFold = EditorGUI.Foldout(foldoutRect, serializedGroup.isExpanded, GUIContent.none, false);

            if ((toggleFold != serializedGroup.isExpanded))
            {
                serializedGroup.isExpanded = toggleFold;
            }

            // Label

            string name = EditorGUI.DelayedTextField(labelRect, group.Name, EditorStyles.boldLabel);

            group.Name = name;

            int index = GUI.Toolbar(toolbarRect, -1, toolbarContent, EditorStyles.toolbarButton);

            if (index != -1)
            {
                GUI.FocusControl(null);
            }

            switch (index)
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
                    ShowCustomGroupMenu(toolbarRect, groupIndex, group);
                    break;
            }

            return serializedGroup.isExpanded;
        }

        private void ShowCustomGroupMenu(Rect rect, int index, ComponentGroup group)
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

            Vector2 mousePosition = Event.current.mousePosition;

            Rect menuRect = rect;
            menuRect.x = mousePosition.x;

            menu.DropDown(menuRect);
        }

        private void DrawCustomComponent(Rect rect, int index, ComponentGroup group, ComponentType component)
        {
            component.Shown = EditorGUI.Toggle(GetCustomToggleRect(rect), component.Shown, Style.ToggleMixed);

            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);

            if (GUI.Button(GetCustomScriptRect(rect), component.Content, Style.ToolbarButtonLeft))
            {
                selectedComponentIndex = index;
                EditorGUIUtility.ShowObjectPicker<MonoScript>(component.Script, false, "", controlID);
            }

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

            if (GUI.Button(GetCustomDeleteRect(rect), Labels.DELETE_COMPONENT_LABEL, Style.ToolbarButtonResizable))
            {
                group.Remove(component);
                EditorUtility.SetDirty(settings);
                serializedSettings.Update();
            }
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

        // --- Rects

        private Rect GetCustomFoldoutRect(Rect rect)
        {
            rect.x += 16f;
            rect.width = 16f;

            return rect;
        }

        private Rect GetCustomHeaderRect(Rect rect)
        {
            rect.x += 20f;
            rect.width -= Values.CUSTOM_TOOLBAR_WIDTH + 19f;
            return rect;
        }

        private Rect GetCustomToolbarRect(Rect windowRect, Rect rect)
        {
            Rect toolbarRect = rect;
            toolbarRect.height = Values.TOOLBAR_HEIGHT;
            toolbarRect.width = Values.CUSTOM_TOOLBAR_WIDTH;
            toolbarRect.x += windowRect.width - Values.CUSTOM_TOOLBAR_WIDTH;

            return toolbarRect;
        }

        // --- Custom Component Rects

        private Rect GetCustomComponentRect(Rect rect)
        {
            rect.y += 21f;
            rect.x += 2f;
            rect.width -= 2f;

            return rect;
        }

        private Rect GetCustomToggleRect(Rect rect)
        {
            rect = GetCustomComponentRect(rect);
            rect.width = 24f;
            rect.y += 2f;

            return rect;
        }

        private Rect GetCustomScriptRect(Rect rect)
        {
            rect = GetCustomComponentRect(rect);
            rect.x += 20f;
            rect.width -= (Values.CUSTOM_TOOLBAR_WIDTH / toolbarContent.Length) + 19f;

            return rect;
        }

        private Rect GetCustomDeleteRect(Rect rect)
        {
            rect = GetCustomComponentRect(rect);
            rect.width = Values.CUSTOM_TOOLBAR_WIDTH / toolbarContent.Length;
            rect.x = windowRect.width - rect.width;

            return rect;
        }

        private Rect GetGroupHoverRect(Rect rect)
        {
            rect.x++;
            rect.width -= 2;
            rect.y++;
            rect.height--;

            return rect;
        }
    }
}