using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private readonly Type MonoType = typeof(MonoBehaviour);
        private readonly GUIContent MonoContent = EditorGUIUtility.IconContent("cs Script Icon");

        private HashSet<Type> componentTypes = new HashSet<Type>();
        private Component[] components = new Component[0];
        private int validComponentCount;

#if UNITY_2019_1_OR_NEWER
        private GUIContent warningGUI = EditorGUIUtility.IconContent("warning");
#else
        private GUIContent warningGUI = EditorGUIUtility.IconContent ("console.warnicon");
#endif

        protected override int GetGridCount()
        {
            if (!HasInitialized)
            {
                return components.Length;
            }

            return validComponentCount;
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

                Type type = item.Component.GetType();
                if (stackScripts && componentTypes.Contains(type))
                {
                    continue;
                }

                if (settings.Components.IsExcluded(type))
                {
                    continue;
                }

                // Draw

                DrawComponentIcon(rect, item);
                //componentTypes.Add(type);
            }

            if (requiresWarning)
            {
                DrawMissingComponent(rect);
            }
        }

        protected override void OnDrawInit(GameObject instance, Settings settings)
        {
            validComponentCount = 1;
            componentTypes.Clear();
        }

        // GUI

        private void DrawComponentIcon(Rect rect, ComponentItem item)
        {
            if (item == null)
            {
                return;
            }

            rect = GetIconPosition(rect);

            if (rect.x < (LabelRect.x + LabelRect.width))
            {
                return;
            }

            if (item.CanToggle)
            {
                DrawComponentToggle(rect, item);
            }
            else
            {
                GUI.Label(rect, item.Content, Style.ComponentIconStyle);
            }

            validComponentCount++;
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
            GUI.Label(rect, item.Content, Style.ComponentIconStyle);
            if (!active)
            {
                GUI.color = c;
            }
        }

        private void DrawMissingComponent(Rect rect)
        {
            rect = GetIconPosition(rect);
            GUI.Label(rect, warningGUI, Style.ComponentIconStyle);
        }

        // Helpers

        private bool CanShow(ComponentItem item, Settings settings)
        {
            if (item.IsBuiltIn)
            {
                return settings.Components.DisplayBuiltIn || item.Type.Shown;
            }

            return settings.Components.DisplayMonoScripts || item.Type.Shown;
        }

        private Rect GetIconPosition(Rect rect)
        {
            // Move to left-most side possible, then move along rows

            rect.x += rect.width;
            rect.x -= INDENT_SIZE * GetGridCount();

            rect.width = rect.height = INDENT_SIZE;

            return rect;
        }
    }
}
