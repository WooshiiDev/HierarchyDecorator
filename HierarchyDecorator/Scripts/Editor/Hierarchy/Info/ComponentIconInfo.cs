using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private static Dictionary<Type, ComponentType> s_allTypes = new Dictionary<Type, ComponentType>();

        private readonly Type MonoType = typeof(MonoBehaviour);
        private readonly GUIContent MonoContent = EditorGUIUtility.IconContent("cs Script Icon");

        private List<Type> componentTypes = new List<Type> ();
        private Component[] components = new Component[0];
        private int validComponentCount;

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
            string stackOutput = string.Empty;

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];

                if (component == null & settings.Components.ShowMissingScriptWarning)
                {
                    DrawMissingComponent(rect);
                    return;
                }

                // Get Type

                Type type = component.GetType();
                ComponentType componentType;

                if (!s_allTypes.TryGetValue(type, out componentType))
                {
                    if (GetComponent(type, out componentType) || RegisterComponent(component, out componentType))
                    {
                        if (componentType.IsBuiltIn)
                        {
                            s_allTypes.Add(type, componentType);
                        }
                    }
                }

                if (componentType == null)
                {
                    continue;
                }

                if (componentType.IsBuiltIn && settings.Components.IsExcluded(type))
                {
                    continue;
                }

                if (componentType.IsBuiltIn)
                {
                    DrawComponent(rect, componentType, settings);
                }
                else
                if (!stackScripts)
                {
                    DrawMonobehaviour(rect, type, componentType, settings);
                }
                else
                {
                    stackOutput += componentType.DiplayName + "\n";
                }
            }

            if (stackScripts && !string.IsNullOrEmpty(stackOutput))
            {
                GUIContent content = new GUIContent(MonoContent)
                {
                    tooltip = stackOutput.Trim()
                };

                DrawComponentIcon(rect, content);
            }

            bool GetComponent(Type type, out ComponentType componentType)
            {
                if (settings.Components.TryGetComponent(type, out componentType))
                {
                    return true;
                }

                return settings.Components.TryGetCustomComponent(type, out componentType);
            }

            bool RegisterComponent(Component component, out ComponentType componentType)
            {
                if (settings.Components.RegisterCustomComponent(component))
                {
                    return GetComponent(component.GetType(), out componentType);
                }

                componentType = null;
                return false;
            }
        }

        protected override void OnDrawInit(GameObject instance, Settings settings)
        {
            validComponentCount = 1;

            components = instance.GetComponents<Component> ();
            componentTypes.Clear ();
            hasMonoBehaviour = false;
        }

        // GUI

        private void DrawMonobehaviour(Rect rect, Type type, ComponentType componentType, Settings settings)
        {
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
            DrawComponentIcon(rect, componentType.Content);
        }

        private void DrawComponent(Rect rect, ComponentType component, Settings settings)
        {
            if (!settings.Components.DisplayBuiltIn)
            {
                if (!component.Shown)
                {
                    return;
                }
            }

            componentTypes.Add(component.Type);
            DrawComponentIcon(rect, component.Content);
        }

        private void DrawComponentIcon(Rect rect, GUIContent content)
        {
            rect = GetIconPosition (rect);

            if (rect.x < (LabelRect.x + LabelRect.width))
            {
                return;
            }

            GUI.Label (rect, content, Style.ComponentIconStyle);
            validComponentCount++;
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
