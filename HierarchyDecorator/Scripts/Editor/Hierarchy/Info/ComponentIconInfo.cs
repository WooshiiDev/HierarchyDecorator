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

        private HashSet<Type> componentTypes = new HashSet<Type> ();
        private Component[] components = new Component[0];
        private int validComponentCount;

#if UNITY_2019_1_OR_NEWER
        private GUIContent warningGUI = EditorGUIUtility.IconContent ("warning");
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
            if (settings.styleData.HasStyle (instance.name) && !settings.styleData.displayIcons)
            {
                return false;
            }

            return settings.Components.Enabled;
        }

        protected override void DrawInfo(Rect rect, GameObject instance, Settings settings)
        {
            bool stackScripts = settings.Components.StackScripts;

            var items = HierarchyManager.Current.Components.Items;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                // Feature - Warning

                if (item.IsNullComponent && settings.Components.ShowMissingScriptWarning)
                {
                    DrawMissingComponent(rect);
                    return;
                }

                Type type = item.Component.GetType();
                if (item.IsBuiltIn)
                {
                    if (!settings.Components.DisplayBuiltIn && !item.Type.Shown)
                    {
                        continue;
                    }
                }
                if (!settings.Components.DisplayMonoScripts && !item.Type.Shown)
                {
                    continue;
                }

                // Feature - Stack Scripts: Only draw once

                if (stackScripts && componentTypes.Contains(type))
                {
                    continue;
                }

                if (settings.Components.IsExcluded(type))
                {
                    continue;
                }

                // Draw

                DrawComponentIcon(rect, item.Component, false, item.Content);
                componentTypes.Add(type);
            }
        }

        protected override void OnDrawInit(GameObject instance, Settings settings)
        {
            validComponentCount = 1;
            componentTypes.Clear ();
        }

        // GUI

        private void DrawComponentIcon(Rect rect, Component component, bool hasToggle, GUIContent content)
        {
            if (component == null)
            {
                return;
            }

            rect = GetIconPosition (rect);

            if (rect.x < (LabelRect.x + LabelRect.width))
            {
                return;
            }

            if (hasToggle)
            {
                DrawComponentToggle(rect, component, content);
            }
            else
            {
                GUI.Label(rect, content, Style.ComponentIconStyle);
            }

            validComponentCount++;
        }

        private void DrawComponentToggle(Rect rect, Component component, GUIContent content)
        {
            bool value;
            if (component is Behaviour behaviour)
            {
                value = behaviour.enabled;
            }
            else
            {
                var property = ReflectionUtility.GetProperty(component.GetType(), "enabled");
                value = (bool)property.GetValue(component);
            }

            Event ev = Event.current;

            if (ev.type == EventType.MouseDown && rect.Contains(ev.mousePosition))
            {
                if (component is Behaviour b)
                {
                    b.enabled = !value;
                    if (Selection.Contains(component.gameObject))
                    {
                        EditorUtility.SetDirty(component.gameObject);
                    }
                }
                else
                {
                    GetEnableValue(component);
                }

                ev.Use();

                
            }

            if (!value)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.4f);
            }

            GUI.Label(rect, content, Style.ComponentIconStyle);
            GUI.color = Color.white;
        }

        private void DrawMissingComponent(Rect rect)
        {
            rect = GetIconPosition (rect, true);
            GUI.Label (rect, warningGUI, Style.ComponentIconStyle);
        }

        private void GetEnableValue(Component component)
        {
            var enabled = ReflectionUtility.GetProperty(component.GetType(), "enabled");
            bool value = (bool)enabled.GetValue(component);

            enabled.SetValue(component, !value);
        }

        // Position Helpers

        private Rect GetIconPosition(Rect rect, bool isInvalid = false)
        {
            int gridCount = isInvalid ? GetGridCount () + 1 : GetGridCount ();

            // Move to left-most side possible, then move along rows
            rect.x += rect.width;
            rect.x -= INDENT_SIZE * gridCount;

            rect.width = rect.height = INDENT_SIZE;

            return rect;
        }
    }
}
