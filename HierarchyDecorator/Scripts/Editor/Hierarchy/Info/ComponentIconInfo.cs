using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine;
using System.ComponentModel;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private static readonly Color s_DisabledColor = new Color(1f, 1f, 1f, 0.4f);
        private Dictionary<Texture, GUIContent> stackTextures = new Dictionary<Texture, GUIContent>();

        private List<ComponentItem> stackItems = new List<ComponentItem>();
        private IList<ComponentItem> items = new List<ComponentItem>();

        private bool requireWarning;
        private int componentCount;

#if UNITY_2019_1_OR_NEWER
        private readonly GUIContent warningGUI = EditorGUIUtility.IconContent("warning");
#else
        private readonly GUIContent warningGUI = EditorGUIUtility.IconContent ("console.warnicon");
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

            if (stackTextures.Count > 0)
            {
                stackTextures.Clear();
                stackItems.Clear();
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
                    if (!stackTextures.TryGetValue(texture, out GUIContent content))
                    {
                        content = new GUIContent(string.Empty, texture, component.DisplayName);
                        stackTextures.Add(texture, content);
                    }
                    else
                    {
                        content.tooltip += "\n" + component.DisplayName;
                    }
                }
                else // Draw
                {
                    DrawComponentIcon(rect, component);
                    rect.x -= INDENT_SIZE;
                }
            }

            if (stackScripts)
            {
                foreach (GUIContent content in stackTextures.Values)
                {
                    DrawIcon(GetIconPosition(rect), content);
                    rect.x -= INDENT_SIZE;
                }
            }

            if (requireWarning)
            {
                DrawMissingComponent(rect);
            }
        }

        // GUI

        private void DrawComponentIcon(Rect rect, ComponentItem item)
        {
            DrawContextMenu(rect, item);

            if (item.CanToggle)
            {
                DrawComponentToggle(rect, item);
            }
            else
            {
                DrawIcon(rect, item.Content);
            }
        }

        private void DrawComponentToggle(Rect rect, ComponentItem item)
        {
            Event ev = Event.current;

            if (ev.type == EventType.MouseDown && ev.button == 0 && rect.Contains(ev.mousePosition))
            {
                item.ToggleActive();
                ev.Use();

                if (Selection.Contains(item.Component.gameObject))
                {
                    EditorUtility.SetDirty(item.Component.gameObject);
                }
            }

            bool active = item.Active;
            if (!active)
            {
                GUI.color = s_DisabledColor;
            }
            DrawIcon(rect, item.Content);
            if (!active)
            {
                GUI.color = Color.white;
            }
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

        // Helpers

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
