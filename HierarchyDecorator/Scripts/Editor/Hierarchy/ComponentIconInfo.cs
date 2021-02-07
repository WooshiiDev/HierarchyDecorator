using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal class ComponentIconInfo : HierarchyInfo
        {
        private List<Type> componentTypes;

        public ComponentIconInfo(Settings settings) : base (settings)
            {
            componentTypes = new List<Type> ();
            }

        public override int GetRowSize()
            {
            return componentTypes.Count;
            }

        public override bool CanDisplayInfo()
            {
            return settings.globalSettings.showComponents;
            }

        protected override void DrawInternal(Rect rect, GameObject instance)
            {
            if (instance == null)
                return;

            componentTypes.Clear ();

            Component[] components = instance.GetComponents<Component>();
            foreach (var component in components)
                {
                if (component == null)
                    continue;

                Type type = component.GetType ();

                if (componentTypes.Contains (type))
                    continue;

                if (type.IsSubclassOf (typeof (MonoBehaviour)))
                    DrawMonobehaviour (rect, component);
                else
                    DrawComponent (rect, type, instance);
                }
            }

        private void DrawMonobehaviour(Rect rect, Component component)
            {
            Type type = component.GetType ();

            if (!settings.globalSettings.showAllComponents)
                {
                // Find custom component
                if (!settings.FindCustomComponentFromType (type, out CustomComponentType componentType))
                    return;

                if (componentType.script == null)
                    return;

                if (!componentType.shown)
                    return;
                }

            string path = AssetDatabase.GetAssetPath (MonoScript.FromMonoBehaviour (component as MonoBehaviour));
            GUIContent content = new GUIContent (AssetDatabase.GetCachedIcon (path));

            componentTypes.Add (type);
            DrawComponentIcon (rect, content, type, component.gameObject.activeInHierarchy);
            }

        private void DrawComponent(Rect rect, Type type, GameObject instance)
            {
            // Need to check for specifics if globally all components are not on
            if (!settings.globalSettings.showAllComponents)
                {
                ComponentType componentType = settings.unityComponents.FirstOrDefault (t => t.type == type);
                
                if (componentType == null)
                    componentType = settings.customComponents.FirstOrDefault (t => t.type == type);

                if (componentType == null || !componentType.shown)
                    return;
                }

            GUIContent content = EditorGUIUtility.ObjectContent (null, type);

            if (content.image == null)
                return;

            componentTypes.Add (type);
            DrawComponentIcon (rect, content, type, instance.activeInHierarchy);
            }

        private void DrawComponentIcon(Rect rect, GUIContent content, Type type, bool isActive)
            {
            // Move to left-most side possible, then move along rows 
            rect.x += rect.width;
            rect.x -= 16f * GetRowSize ();
            rect.width = rect.height;

            content.tooltip = type.Name;
            content.text = "";

            GUI.Label (rect, content, Style.componentIconStyle);
            }
        }
    }
