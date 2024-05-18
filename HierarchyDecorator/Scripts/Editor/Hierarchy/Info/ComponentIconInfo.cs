using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private Dictionary<Texture, GUIContent> stackTextures = new Dictionary<Texture, GUIContent>();
        private int iconCount;

#if UNITY_2019_1_OR_NEWER
        private readonly GUIContent warningGUI = EditorGUIUtility.IconContent("warning");
#else
        private readonly GUIContent warningGUI = EditorGUIUtility.IconContent ("console.warnicon");
#endif

        protected override int GetGridCount()
        {
            return iconCount;
        }

        protected override bool DrawerIsEnabled(Settings settings, GameObject instance)
        {
            if (settings.styleData.HasStyle(instance.name) && !settings.styleData.displayIcons)
            {
                return false;
            }

            return settings.Components.Enabled;
        }

        protected override void DrawInfo(Rect rect, GameObject instance, Settings settings)
        {

            bool stackScripts = settings.Components.StackScripts;

            bool requiresWarning = false;
            var items = HierarchyManager.Current.Components.Items;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                // Feature - Warning

                if (item.IsNullComponent && settings.Components.ShowMissingScriptWarning)
                {
                    requiresWarning = true;
                    continue;
                }

                if (!CanShow(item, settings))
                {
                    continue;
                }

                // Feature - Stack Scripts: Only draw once

                if (stackScripts)
                {
                    Texture texture = item.Content.image;
                    if (!stackTextures.TryGetValue(texture, out GUIContent content))
                    {
                        content = new GUIContent(string.Empty, texture, item.DisplayName);
                        stackTextures.Add(texture, content);
                    }
                    else
                    {
                        stackTextures[texture].tooltip += "\n" + item.DisplayName;
                    }
                }
                else // Draw
                {
                    DrawComponentIcon(rect, item);
                    iconCount++;
                }
            }

            if (stackScripts)
            {
                foreach (GUIContent content in stackTextures.Values)
                {
                    DrawIcon(GetIconPosition(rect), content);
                    iconCount++;
                }
            }

            if (requiresWarning)
            {
                DrawMissingComponent(rect);
            }
        }

        protected override void OnDrawInit(GameObject instance, Settings settings)
        {
            iconCount = 1;
            stackTextures.Clear();
        }

        // GUI

        private void DrawComponentIcon(Rect rect, ComponentItem item)
        {
            rect = GetIconPosition(rect);

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

            if (ev.type == EventType.MouseDown && rect.Contains(ev.mousePosition))
            {
                item.ToggleActive();
                ev.Use();

                if (Selection.Contains(item.Component.gameObject))
                {
                    EditorUtility.SetDirty(item.Component.gameObject);
                }
            }

            bool active = item.Active;
            Color c = GUI.color;
            if (!active)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.4f);
            }
            DrawIcon(rect, item.Content);
            if (!active)
            {
                GUI.color = c;
            }
        }

        private void DrawMissingComponent(Rect rect)
        {
            rect = GetIconPosition(rect);
            DrawIcon(rect, warningGUI);
        }

        private void DrawIcon(Rect rect, GUIContent content)
        {
            if (IsIconOutOfBounds(rect))
            {
                return;
            }

            GUI.Label(rect, content, Style.ComponentIconStyle);
        }

        // Helpers

        private bool CanShow(ComponentItem item, Settings settings)
        {
            if (item.Type.Excluded)
            {
                return false;
            }

            bool shown = item.Type.Shown && !item.Type.Excluded;

            if (item.IsBuiltIn)
            {
                return settings.Components.DisplayBuiltIn || shown;
            }

            return settings.Components.DisplayMonoScripts || shown;
        }

        private Rect GetIconPosition(Rect rect)
        {
            // Move to left-most side possible, then move along rows

            rect.x += rect.width;
            rect.x -= INDENT_SIZE * GetGridCount();

            rect.width = rect.height = INDENT_SIZE;

            return rect;
        }
  
        private bool IsIconOutOfBounds(Rect rect)
        {
            return rect.x < (LabelRect.x + LabelRect.width);
        }
    }
}
