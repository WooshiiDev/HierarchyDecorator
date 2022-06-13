using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private List<Type> componentTypes = new List<Type> ();
        private Component[] components = new Component[0];

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

            return settings.globalData.showComponentIcons;
        }

        protected override void DrawInfo(Rect rect, GameObject instance, Settings settings)
        {
            for (int i = components.Length; i-- > 0;)
            {
                Component component = components[i];

                if (component == null)
                {
                    if (settings.componentData.showMissingScriptsWarning)
                    {
                        DrawMissingComponent (rect);
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }

                Type type = component.GetType ();

                if (componentTypes.Contains (type))
                {
                    continue;
                }

                if (type.IsSubclassOf (typeof (MonoBehaviour)) && settings.componentData.FindCustomComponentFromType(type, out CustomComponentType componentType))
                {
                    DrawMonobehaviour (rect, component, componentType, settings);
                }
                else
                {
                    DrawComponent (rect, type, instance, settings);
                }
            }
        }

        protected override void OnDrawInit(GameObject instance, Settings settings)
        {
            components = instance.GetComponents<Component> ();
            componentTypes.Clear ();
        }

        // GUI

        private void DrawMonobehaviour(Rect rect, Component component, CustomComponentType componentType, Settings settings)
        {
            Type type = component.GetType ();

            if (!settings.globalData.showAllComponents)
            {
                if (componentType.script == null)
                {
                    return;
                }

                if (!componentType.shown)
                {
                    return;
                }
            }

            string path = AssetDatabase.GetAssetPath (MonoScript.FromMonoBehaviour (component as MonoBehaviour));
            GUIContent content = new GUIContent (AssetDatabase.GetCachedIcon (path));

            componentTypes.Add (type);
            DrawComponentIcon (rect, content, type);
        }

        private void DrawComponent(Rect rect, Type type, GameObject instance, Settings settings)
        {
            // Need to check for specifics if globally all components are not on
            if (!settings.globalData.showAllComponents)
            {
                ComponentType componentType = settings.componentData.unityComponents.FirstOrDefault (t => t.type == type);

                if (componentType == null)
                {
                    componentType = settings.componentData.customComponents.FirstOrDefault (t => t.type == type);
                }

                if (componentType == null || !componentType.shown)
                {
                    return;
                }
            }

            GUIContent content = EditorGUIUtility.ObjectContent (null, type);

            if (content.image == null)
            {
                return;
            }

            componentTypes.Add (type);
            DrawComponentIcon (rect, content, type);
        }

        private void DrawComponentIcon(Rect rect, GUIContent content, Type type)
        {
            rect = GetIconPosition (rect);

            if (rect.x < (LabelRect.x + LabelRect.width))
            {
                return;
            }

            content.tooltip = type.Name;
            content.text = "";

            //GUI.DrawTexture (rect, content.image, ScaleMode.ScaleToFit);
            GUI.Label (rect, content, Style.ComponentIconStyle);
        }

        private void DrawMissingComponent(Rect rect)
        {
            rect = GetIconPosition (rect, true);

            warningGUI.text = "";

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
