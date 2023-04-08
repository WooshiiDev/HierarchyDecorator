using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private Dictionary<Type, ComponentType> foundTypes = new Dictionary<Type, ComponentType>();

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

                if (component == null && settings.Components.ShowMissingScriptWarning)
                {
                    DrawMissingComponent(rect);
                    return;
                }

                Type type = component.GetType();
                
                if (!IsValid(type) || settings.Components.IsExcluded(type))
                {
                    continue;
                }

                // Draw built in
                ComponentType componentType;
                bool hasCached = foundTypes.TryGetValue(type, out componentType);

                if (!hasCached)
                {
                    GetComponent(type, out componentType);
                }

                if (componentType != null)
                {
                    if (componentType.IsBuiltIn)
                    {
                        DrawComponent(rect, componentType, settings);
                    }
                    else
                    {
                        DrawMonobehaviour(rect, component, componentType, settings);
                    }

                    if (!hasCached)
                    {
                        foundTypes.Add(type, componentType);
                    }
                }
                else
                if (settings.Components.DisplayMonoScripts)
                {
                    settings.Components.RegisterCustomComponent(component);
                }

                if (!hasMonoBehaviour)
                {
                    hasMonoBehaviour = type.IsSubclassOf(typeof(MonoBehaviour));
                }
            }

            bool IsValid(Type type)
            {
                if (!hasMonoBehaviour || !settings.Components.StackScripts)
                {
                    return true;
                }

                return type.IsSubclassOf(MonoType);
            }

            bool GetComponent(Type type, out ComponentType componentType)
            {
                if (settings.Components.TryGetComponent(type, out componentType))
                {
                    return true;
                }

                return settings.Components.TryGetCustomComponent(type, out componentType);
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

        private void DrawComponent(Rect rect, ComponentType component, Settings settings)
        {
            if (!settings.Components.DisplayBuiltIn)
            {
                return;
            }

            Type type = component.Type;
            GUIContent content = component.Content;

            if (settings.Components.StackScripts && type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                type = MonoType;
                content = MonoContent;
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
