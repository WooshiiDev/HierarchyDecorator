﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentIconInfo : HierarchyInfo
    {
        private List<Type> componentTypes = new List<Type> ();

        protected override int GetGridCount()
        {
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
            componentTypes.Clear ();

            Component[] components = instance.GetComponents<Component> ();
            for (int i = components.Length; i-- > 0;)
            {
                var component = components[i];

                if (component == null)
                {
                    continue;
                }

                Type type = component.GetType ();

                if (componentTypes.Contains (type))
                {
                    continue;
                }

                if (type.IsSubclassOf (typeof (MonoBehaviour)))
                {
                    DrawMonobehaviour (rect, component, settings);
                }
                else
                {
                    DrawComponent (rect, type, instance, settings);
                }
            }
        }

        private void DrawMonobehaviour(Rect rect, Component component, Settings settings)
        {
            Type type = component.GetType ();

            if (!settings.globalData.showAllComponents)
            {
                // Find custom component
                if (!settings.componentData.FindCustomComponentFromType (type, out CustomComponentType componentType))
                {
                    return;
                }

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
            // Move to left-most side possible, then move along rows
            rect.x += rect.width;
            rect.x -= INDENT_SIZE * GetGridCount ();

            rect.width = rect.height = INDENT_SIZE;

            content.tooltip = type.Name;
            content.text = "";

            //GUI.DrawTexture (rect, content.image, ScaleMode.ScaleToFit);
            GUI.Label (rect, content, Style.ComponentIconStyle);
        }
    }
}
