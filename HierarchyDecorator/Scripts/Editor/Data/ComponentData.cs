using System;
using System.Collections.Generic;
using UnityEngine;

namespace HierarchyDecorator
{
    [System.Serializable]
    public class ComponentData
    {
        public bool showMissingScriptsWarning;

        // Collecitons
        public List<ComponentType> unityComponents = new List<ComponentType> ();
        public List<CustomComponentType> customComponents = new List<CustomComponentType> ();

        // Component Checks
        private string unityVersion;
        private bool isDirty;

        // Reflected Types
        private Type[] allTypes;
     
        public ComponentData()
        {
            unityComponents = new List<ComponentType> ();
            customComponents = new List<CustomComponentType> ();
        }

        /// <summary>
        /// Update Component Data
        /// </summary>
        public void UpdateData(bool updateCustomComponents = false)
        {
            //Reflection for component types
            if (allTypes == null)
            {
                allTypes = ReflectionUtility.GetTypesFromAllAssemblies (typeof (Component));
            }

            if (unityVersion != Application.unityVersion)
            {
                Debug.LogWarning ("HierarchyDecorator Unity Version was invalid, searching for components.");

                unityVersion = Application.unityVersion;
                isDirty = true;
            }
            else
            {
                isDirty = unityComponents.Count != allTypes.Length;
            }

            // Update the components if required
            if (isDirty)
            {
                unityComponents.Clear ();

                for (int i = 0; i < allTypes.Length; i++)
                {
                    unityComponents.Add (new ComponentType (allTypes[i]));
                }

                Debug.LogWarning (string.Format("HierarchyDecorator found {0} Unity components.", allTypes.Length));

                isDirty = false;
            }
            else
            {
                for (int i = 0; i < allTypes.Length; i++)
                {
                    ComponentType component = unityComponents[i];

                    if (component.type == null)
                    {
                        component.UpdateType (allTypes[i]);
                    }
                }
            }

            if (updateCustomComponents)
            {
                UpdateCustomComponentData ();
            }
        }

        /// <summary>
        /// Find a custom component with the given type
        /// </summary>
        /// <param name="type">The type to find</param>
        /// <param name="component">The component returned if one is found. Otherwise this will be null</param>
        /// <returns>Returns a boolean based on if a component was found</returns>
        public bool FindCustomComponentFromType(Type type, out CustomComponentType component)
        {
            for (int i = 0; i < customComponents.Count; i++)
            {
                CustomComponentType customComponent = customComponents[i];

                if (customComponent.script == null || customComponent.type == null)
                {
                    customComponent.UpdateScriptType ();
                }

                if (customComponent.script == null)
                {
                    continue;
                }

                // Not a good work around
                if (customComponent.type == type)
                {
                    component = customComponent;
                    return true;
                }
            }

            component = null;
            return false;
        }

        /// <summary>
        /// Update the custom components
        /// </summary>
        public void UpdateCustomComponentData()
        {
            for (int i = 0; i < customComponents.Count; i++)
            {
                customComponents[i].UpdateScriptType ();
            }
        }
    }
}