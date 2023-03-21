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

        private List<Type> componentTypes = new List<Type> ();
        private Component[] components = new Component[0];

        private bool hasMonoBehaviour = false;

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

            return componentTypes.Count;
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
            for (int i = components.Length; i-- > 0;)
            {
                Component component = components[i];

                if (component == null)
                {
                    if (settings.Components.ShowMissingScriptWarning)
                    {
                        DrawMissingComponent (rect);
                        return;
                    }

                    continue;
                }

                Type type = component.GetType ();
                
                bool isInvalid = componentTypes.Contains(type);

                if (!isInvalid)
                {
                    if (hasMonoBehaviour && settings.Components.StackScripts)
                    {
                        isInvalid = type.IsSubclassOf(typeof(MonoBehaviour));
                    }
                }

                if (isInvalid || settings.Components.IsExcluded(type))
                {
                    continue;
                }

                if (settings.Components.TryGetComponent(type, out _))
                {
                    DrawComponent(rect, type, instance, settings);
                }
                else
                {
                    // If no built in component is found, attempt to draw as custom

                    if (settings.Components.TryGetCustomComponent(type, out ComponentType customType))
                    {
                        DrawMonobehaviour(rect, component, customType, settings);
                    }
                    else
                    if (settings.Components.DisplayMonoScripts)
                    {
                        settings.Components.RegisterCustomComponent(component);
                    }
                }

                if (!hasMonoBehaviour)
                {
                    hasMonoBehaviour = type.IsSubclassOf(typeof(MonoBehaviour));
                }
            }
        }

        protected override void OnDrawInit(GameObject instance, Settings settings)
        {
            components = instance.GetComponents<Component> ();
            componentTypes.Clear ();
            hasMonoBehaviour = false;
        }

        // GUI

        private void DrawMonobehaviour(Rect rect, Component component, ComponentType componentType, Settings settings)
        {
            Type type = component.GetType ();

            if (!settings.Components.DisplayMonoScripts)
            {
                if (componentType.Script == null)
                {
                    return;
                }

                if (!componentType.Shown)
                {
                    return;
                }
            }

            componentTypes.Add(type);

            GUIContent content = componentType.Content;

            if (settings.Components.StackScripts && type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                type = MonoType;
                content = MonoContent;
            }

            DrawComponentIcon (rect, content, type);
        }

        private void DrawComponent(Rect rect, Type type, GameObject instance, Settings settings)
        {
            // Get the corresponding component type

            ComponentType component = null;
            if (!settings.Components.TryGetComponent(type, out component))
            {
                return;
            }

            if (!settings.Components.DisplayBuiltIn && !component.Shown)
            {
                return;
            }

            GUIContent content = component.Content;

            if (settings.Components.StackScripts && type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                type = MonoType;
                content = MonoContent;
            }

            if (content.image == null)
            {
                return;
            }

            componentTypes.Add(type);
            DrawComponentIcon(rect, content, type);
        }

        private void DrawComponentIcon(Rect rect, GUIContent content, Type type)
        {
            rect = GetIconPosition (rect);

            if (rect.x < (LabelRect.x + LabelRect.width))
            {
                return;
            }

            content.tooltip = type.Name;

            //GUI.DrawTexture (rect, content.image, ScaleMode.ScaleToFit);
            GUI.Label (rect, content, Style.ComponentIconStyle);
        }

        private void DrawMissingComponent(Rect rect)
        {
            rect = GetIconPosition (rect, true);
            GUI.Label (rect, warningGUI, Style.ComponentIconStyle);
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
