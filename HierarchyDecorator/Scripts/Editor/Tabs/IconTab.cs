using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityScript.Core;

namespace HierarchyDecorator
{
    [RegisterTab ()]
    public class IconTab : SettingsTab
    {
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
        private int scriptSelectionIndex;

        private string searchText = "";
        private int groupIndex;

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
        private Dictionary<string, IconInfo[]> unityGroups = new Dictionary<string, IconInfo[]> ();

        private Rect windowRect;

        // Constructor

        public IconTab(Settings settings, SerializedObject serializedSettings) : base (settings, serializedSettings, "components", "Icons", "d_FilterByType")
        {
            // Setup References
            components = settings.Components;

            ShowAllProperty = serializedTab.FindPropertyRelative("showAllComponents");
            ShowMissingProperty = serializedTab.FindPropertyRelative ("showMissingScriptWarning");

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

            EditorGUI.BeginDisabledGroup(IsSearching());
            Rect rect = EditorGUILayout.BeginVertical(Style.ToolbarNoSpace, GUILayout.Width(70f), GUILayout.MinHeight(300f));
            {
                int index;
                bool onCustom;

                GUILayout.Label("Categories", EditorStyles.boldLabel, GUILayout.Height(21f));

                EditorGUI.BeginChangeCheck();
                {

                    index = GUILayout.SelectionGrid(groupIndex, groupNames, 1, Style.ToolbarButtonLeft);
                    onCustom = GUILayout.Toggle(isOnCustom, Labels.CUSTOM_COMPONENTS_LABEL, Style.ToolbarButtonLeft);
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
                    DrawCustomComponents(windowRect);
                }
                else // Draw the group if selected
                if (groupIndex < groupNames.Length)
                {
                    string group = groupNames[groupIndex];
                    DrawComponentsColumns(unityGroups[group]);
                }

                DrawBorder(windowRect);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawGroupHeader(IEnumerable<IconInfo> icons)
        {
            // Draw search fieldf

            Rect rect = EditorGUILayout.BeginHorizontal(Style.ToolbarNoSpace, GUILayout.Height(21f));
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
            EditorGUILayout.BeginHorizontal ();
            {
                // Get lengths, including the length of a single column

                int len = types.Length;
                int halfLen = len / 2;

                if (len % 2 == 0)
                {
                    halfLen--;
                }

                columnWidth = EditorGUIUtility.currentViewWidth / 2 - Values.COLUMN_WIDTH_OFFSET;

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

                    HierarchyGUI.Space();
                }
                EditorGUILayout.EndVertical ();
            }
            EditorGUILayout.EndHorizontal ();
            EditorGUILayout.EndScrollView();
        }

        private void DrawFilteredComponents(string filter)
        {
            filter = filter.ToLower ();
            
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

        private Vector2 customScroll;
        private Vector2 scrollPosition;
        private float customScrollHeight;

        private void DrawCustomComponents(Rect windowRect)
        {
            EditorGUILayout.BeginVertical(Style.ToolbarNoSpace);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(Labels.ADD_GROUP_LABEL, GUI.skin.button))
            {
                components.AddCustomGroup("New Group");
                EditorUtility.SetDirty(settings);

                serializedSettings.Update();
                SerializedCustomGroups = GetSerializedArrayElements("customGroups");
            }

            EditorGUILayout.EndVertical();

            float newHeight = 0;

            windowRect.width -= 1f;
            windowRect.height = Values.ICON_WINDOW_HEIGHT - Values.TOOLBAR_HEIGHT;
            scrollPosition = GUI.BeginScrollView(windowRect, scrollPosition, new Rect(0, 0, windowRect.width - 13f, customScrollHeight));
            {
                Rect groupRect = new Rect(0, 0, windowRect.width - 13f, Values.TOOLBAR_HEIGHT);
                int customLen = components.CustomGroups.Length;

                for (int i = 0; i < customLen; i++)
                {
                    ComponentGroup group = components.CustomGroups[i];
                    SerializedProperty serializedGroup = SerializedCustomGroups[i];

                    //GUI.Box(groupRect, GUIContent.none, EditorStyles.toolbar);
                    //EditorGUI.LabelField(toolbarRect, Labels.DISABLE_LABEL);

                    Rect foldoutRect = GetCustomFoldoutRect(groupRect);
                    Rect labelRect = GetCustomHeaderRect(groupRect);
                    Rect toolbarRect = GetCustomToolbarRect(windowRect, groupRect);

                    Rect fullRect = groupRect;

                    // Draw background

                    GUI.Box(groupRect, GUIContent.none, EditorStyles.toolbar);

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
                            ShowCustomGroupMenu(toolbarRect, i, group);
                            break;
                    }

                    if (toggleFold)
                    {
                        Rect componentRect = groupRect;
                        componentRect.y += 21f;
                        componentRect.height = 19f;
                        componentRect.x += 2f;
                        componentRect.width -= 2f;

                        Rect toggleRect = componentRect;
                        toggleRect.width = 24f;

                        Rect scriptRect = toggleRect;
                        scriptRect.height = 16f;
                        scriptRect.width = componentRect.width - toggleRect.width - 22f;
                        scriptRect.x += 20f;

                        Rect deleteRect = toggleRect;
                        deleteRect.width = 24f;
                        deleteRect.x += componentRect.width - deleteRect.width - 2f;

                        int count = group.Count;
                        for (int j = 0; j < count; j++)
                        {
                            // Required here for group size checks

                            if (j >= group.Count)
                            {
                                return;
                            }

                            ComponentType component = group.Get(j);

                            toggleRect.y = componentRect.y + 2;
                            scriptRect.y = componentRect.y;
                            deleteRect.y = componentRect.y;

                            component.Shown = EditorGUI.Toggle(toggleRect, component.Shown, Style.ToggleMixed);

                            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);

                            if (GUI.Button(scriptRect, component.Content, Style.ToolbarButtonLeft))
                            {
                                scriptSelectionIndex = j;
                                EditorGUIUtility.ShowObjectPicker<MonoScript>(component.Script, false, "", controlID);
                            }

                            if (EditorGUIUtility.GetObjectPickerControlID() == controlID && j == scriptSelectionIndex)
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
                                    scriptSelectionIndex = -1;
                                }
                            }

                            if (GUI.Button(deleteRect, Labels.DELETE_COMPONENT_LABEL, Style.CenteredBoldLabel))
                            {
                                group.Remove(j);
                                j--;

                                EditorUtility.SetDirty(settings);
                                serializedSettings.Update();
                            }

                            componentRect.y += 20f;
                            groupRect.y += 20f;

                            fullRect.height += 20f;
                            newHeight += 20f;
                        }
                    }
                    
                    HandleEventsOnGroup(fullRect, group);
                    DrawBorder(fullRect);

                    groupRect.y += Values.TOOLBAR_HEIGHT;
                    newHeight += Values.TOOLBAR_HEIGHT;
                }
            }

            // End the scroll view that we began above.

            GUI.EndScrollView();

            // Assign the scroll height for the next draw call

            customScrollHeight = newHeight;
        }

        private void ShowCustomGroupMenu(Rect rect, int index, ComponentGroup group)
        {
            GUIContent moveUp = new GUIContent("Move Up");
            GUIContent moveDown = new GUIContent("Move Down");
            GUIContent delete = new GUIContent("Delete Group");

            GenericMenu menu = new GenericMenu();

            // Move up

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

        private bool performDrag = false;

        private void HandleEventsOnGroup(Rect groupRect, ComponentGroup group)
        {
            if (Selection.objects.Length == 0)
            {
                return;
            }

            Event ev = Event.current;
            bool isOver = groupRect.Contains(ev.mousePosition);

            if (isOver)
            {
                switch (ev.type)
                {
                    case EventType.DragPerform:
                    case EventType.DragUpdated:

                        DragAndDrop.AcceptDrag();

                        if (!performDrag)
                        {
                            bool isValid = true;
                            foreach (var item in DragAndDrop.objectReferences)
                            {
                                if (!(item is MonoScript))
                                {
                                    //if (item is DefaultAsset)
                                    //{
                                    //    string path = AssetDatabase.GetAssetPath(item);
                                    //    if (!AssetDatabase.IsValidFolder(path))
                                    //    {
                                    //        isValid = false;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    isValid = false;
                                    //}
                                    isValid = false;
                                    break;
                                }                                
                            }

                            performDrag = isValid;
                        }


                        break;

                    case EventType.DragExited when performDrag:

                        foreach (var item in DragAndDrop.objectReferences)
                        {
                            ComponentType component = new ComponentType(typeof(MonoScript), false);
                            component.UpdateType(item as MonoScript);

                            if (component.IsValid())
                            {
                                group.Add(component);
                            }
                        }
                        performDrag = false;
                        break;
                }

                if (performDrag)
                {
                    Handles.BeginGUI();
                    {
                        Handles.DrawSolidRectangleWithOutline(groupRect, Color.clear, Color.white);
                    }
                    Handles.EndGUI();
                }
            }
        }

        private void HandleEvents(Rect windowRect)
        {
            
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
            rect.width -= (toolbarContent.Length + 1) * 26f;
            return rect;
        }

        private Rect GetCustomToolbarRect(Rect windowRect, Rect rect)
        {
            Rect toolbarRect = rect;
            toolbarRect.x += windowRect.width - 128f;
            toolbarRect.height = Values.TOOLBAR_HEIGHT;
            toolbarRect.width = rect.width - (toolbarRect.x - rect.x);

            return toolbarRect;
        }
    }
}