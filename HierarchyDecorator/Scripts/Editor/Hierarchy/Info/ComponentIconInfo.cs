
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private static readonly Color s_DisabledColor = new Color(1f, 1f, 1f, 0.4f);

        private Dictionary<Texture, List<ComponentItem>> stackedComponents = new Dictionary<Texture, List<ComponentItem>>();
        private Dictionary<Texture, GUIContent> stackedContent = new Dictionary<Texture, GUIContent>();
        private IList<ComponentItem> items = new List<ComponentItem>();

        private bool requireWarning;
        private int componentCount;

#if UNITY_2019_1_OR_NEWER
        private readonly GUIContent warningGUI = EditorGUIUtility.IconContent("warning");
#else
        private readonly GUIContent warningGUI = EditorGUIUtility.IconContent("console.warnicon");
#endif
        protected override void OnDrawInit(HierarchyItem item, Settings settings)
        {
            requireWarning = false;

            List<ComponentItem> newComponents = new List<ComponentItem>();
            foreach (ComponentItem component in item.Components.GetItems())
            {
                if (component.IsNullComponent && settings.Components.ShowMissingScriptWarning)
                {
                    requireWarning = true;
                    continue;
                }

                if (!CanShow(component, settings))
                {
                    continue;
                }

                newComponents.Add(component);
            }

            items = newComponents;
            componentCount = items.Count;

            if (stackedComponents.Count > 0)
            {
                stackedComponents.Clear();
                stackedContent.Clear();
            }
        }

        protected override int CalculateGridCount()
        {
            return componentCount;
        }

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            if (!settings.styleData.displayIcons && settings.styleData.HasStyle(item.DisplayName))
            {
                return false;
            }

            return settings.Components.Enabled;
        }

        protected override void DrawInfo(Rect rect, HierarchyItem item, Settings settings)
        {
            bool stackScripts = settings.Components.StackScripts;

            rect = GetIconPosition(rect);

            for (int i = 0; i < componentCount; i++)
            {
                if (IsIconOutOfBounds(rect))
                {
                    break;
                }

                ComponentItem component = items[i];

                // Feature - Stack Scripts: Only draw once

                if (stackScripts)
                {
                    Texture texture = component.Content.image;
                    if (!stackedComponents.TryGetValue(texture, out List<ComponentItem> cache))
                    {
                        stackedComponents[texture] = new List<ComponentItem>();
                        stackedContent[texture] = new GUIContent(string.Empty, component.Content.image);
                    }

                    stackedComponents[texture].Add(component);
                    stackedContent[texture].tooltip += "\n" + component.DisplayName;
                }
                else // Draw
                {
                    DrawComponentIcon(rect, component, settings);
                    rect.x -= INDENT_SIZE;
                }
            }

            if (stackScripts)
            {
                foreach (var content in stackedContent)
                {
                    DrawStackedComponentToggle(rect, content.Value);
                    rect.x -= INDENT_SIZE;
                }
            }

            if (requireWarning)
            {
                DrawMissingComponent(rect);
            }
        }

        // GUI

        private void DrawComponentIcon(Rect rect, ComponentItem item, Settings settings)
        {
            DrawContextMenu(rect, item);
            
            if (item.HasToggle && settings.Components.ClickToToggleComponent)
            {
                DrawComponentToggle(rect, item);
            }
            else
            {
                DrawIcon(rect, item.Content);
            }
        }

        private void DrawStackedComponentToggle(Rect rect, GUIContent content)
        {
            Event ev = Event.current;
            IList<ComponentItem> items = stackedComponents[content.image];

            bool state = items[0].Active;
            if (HasToggledComponent(rect, ev))
            {
                ev.Use();
                state = !state;
                foreach (ComponentItem component in items)
                {
                    if (component.Active != state)
                    {
                        ToggleComponent(component);
                    }
                }
            }

            DrawActiveComponent(rect, content, state);
        }

        private void DrawComponentToggle(Rect rect, ComponentItem component)
        {
            Event ev = Event.current;

            if (HasToggledComponent(rect, ev))
            {
                ev.Use();
                ToggleComponent(component);
            }

            DrawActiveComponent(rect, component.Content, component.Active);
        }

        private void DrawActiveComponent(Rect rect, GUIContent content, bool active)
        {
            if (!active)
            {
                GUI.color = s_DisabledColor;
            }
            DrawIcon(rect, content);
            if (!active)
            {
                GUI.color = Color.white;
            }
        }

        private bool HasToggledComponent(Rect rect, Event ev)
        {
            return ev.type == EventType.MouseDown && ev.button == 0 && rect.Contains(ev.mousePosition);
        }

        private void DrawMissingComponent(Rect rect)
        {
            rect = GetIconPosition(rect);
            DrawIcon(rect, warningGUI);
        }

        private void DrawIcon(Rect rect, GUIContent content)
        {
            GUI.Label(rect, content, Style.ComponentIconStyle);
        }

        private void DrawContextMenu(Rect rect, ComponentItem item)
        {
            Event ev = Event.current;

            if (ev.type == EventType.MouseDown && ev.button == 1 && rect.Contains(ev.mousePosition))
            {
                var method = typeof(EditorUtility).GetMethod("DisplayObjectContextMenu", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(Rect), typeof(Object), typeof(int) }, null);
                method.Invoke(null, new object[] { rect, item.Component, 0 });
                ev.Use();
            }
        }

        private void ToggleComponent(ComponentItem component)
        {
            component.ToggleActive();
            if (Selection.Contains(component.Component.gameObject))
            {
                EditorUtility.SetDirty(component.Component.gameObject);
            }
        }

        private bool CanShow(ComponentItem item, Settings settings)
        {
            if (item.IsNullComponent || item.Type.Excluded)
            {
                return false;
            }

            bool shown = item.Type.Shown;

            if (item.IsBuiltIn)
            {
                return settings.Components.DisplayBuiltIn || shown;
            }

            return settings.Components.DisplayMonoScripts || shown;
        }

        private Rect GetIconPosition(Rect rect)
        {
            rect.x += rect.width - INDENT_SIZE;
            rect.width = INDENT_SIZE;
            return rect;
        }

        private bool IsIconOutOfBounds(Rect rect)
        {
            return rect.x < (LabelRect.x + LabelRect.width);
        }
    }
}
