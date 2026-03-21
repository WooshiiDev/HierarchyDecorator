using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    [Flags]
    public enum DisplayMode
    {
        /// <summary>
        /// Draw both MonoBehaviours and built in.
        /// </summary>
        Unity = 1,

        /// <summary>
        /// Display MonoBehaviours
        /// </summary>
        Custom = 2,
    }

    /// <summary>
    /// Contains all information required for components.
    /// </summary>
    [Serializable]
    public class ComponentData
    {
        private readonly static CategoryFilter DefaultFilter = new CategoryFilter("General", string.Empty, FilterType.NONE);

        public readonly static CategoryFilter[] ComponentFilters =
        {
            new CategoryFilter ("2D", "2D", FilterType.NAME),

            // Animation
            new CategoryFilter ("Animation", "Anim", FilterType.NAME),
            new CategoryFilter ("Animation", "Constraint", FilterType.NAME),

            // Audio
            new CategoryFilter ("Audio", "Audio", FilterType.NAME),

            // Mesh
            new CategoryFilter ("Mesh", "Renderer", FilterType.NAME),
            new CategoryFilter ("Mesh", "Mesh", FilterType.NAME),

            // Physics
            new CategoryFilter ("Physics", "Collider", FilterType.NAME),
            new CategoryFilter ("Physics", "Joint", FilterType.NAME),
            new CategoryFilter ("Physics", "Rigidbody", FilterType.NAME),

            // Networking
            new CategoryFilter ("Network", "Network", FilterType.NAME),

            new CategoryFilter ("UI", "Canvas", FilterType.NAME),
            new CategoryFilter ("UI", "UnityEngine.EventSystems.UIBehaviour, UnityEngine.UI", FilterType.TYPE),
            new CategoryFilter ("UI", "UnityEngine.GUIElement, UnityEngine", FilterType.TYPE)
        };

        // - Settings

        [SerializeField] private bool enableIcons = true;
        [SerializeField, Tooltip("Will clicking the icon toggle the component")] private bool clickToToggleComponent = true;

        [SerializeField] private bool showMissingScriptWarning = true;
        [SerializeField] private DisplayMode showAll = DisplayMode.Unity | DisplayMode.Custom;
        [SerializeField] private bool stackDuplicateIcons;

        // - Components

        [SerializeField] private ComponentCache unityCache;
        [SerializeField] private ComponentCache customCache;

        // - Groups

        [SerializeField] private ComponentGroup[] unityGroups = new ComponentGroup[0];
        [SerializeField] private List<ComponentGroup> customGroups = new List<ComponentGroup>();

        private Type[] allTypes = new Type[0];

        // - Properties

        /// <summary>
        /// Is the missing script warning on?
        /// </summary>
        public bool ShowMissingScriptWarning
        {
            get
            {
                return showMissingScriptWarning;
            }
        }

        public bool StackScripts => stackDuplicateIcons;

        /// <summary>
        /// Are components enabled?
        /// </summary>
        public bool Enabled => enableIcons;

        /// <summary>
        /// Are components toggleable when clicked?
        /// </summary>
        public bool ClickToToggleComponent => clickToToggleComponent;

        /// <summary>
        /// How scripts are drawn in the hierarchy.
        /// <val
        /// </summary>
        public DisplayMode DisplayMode
        {
            get
            {
                return showAll;
            }
        }

        /// <summary>
        /// Are all components shown enabled currently?
        /// </summary>
        public bool DisplayAll => showAll.HasFlag(DisplayMode.Unity | DisplayMode.Custom);

        /// <summary>
        /// 
        /// </summary>
        public bool DisplayBuiltIn => showAll.HasFlag(DisplayMode.Unity);

        /// <summary>
        /// 
        /// </summary>
        public bool DisplayMonoScripts => showAll.HasFlag(DisplayMode.Custom);

        /// <summary>
        /// Component groups regarding built-in Unity types.
        /// </summary>
        public ComponentGroup[] UnityGroups => unityGroups;

        /// <summary>
        /// Component groups regarding custom components.
        /// </summary>
        public ComponentGroup[] CustomGroups => customGroups.ToArray();

        // --- Methods

        // --- Initialization


        /// <summary>
        /// Update Component Data
        /// </summary>
        public void UpdateData()
        {
            allTypes = TypeCache.GetTypesDerivedFrom(typeof(Component))
              .Where(t => t.Assembly.GetName().Name.StartsWith("Unity") && !t.IsAbstract)
              .OrderBy(t => t.Name)
              .ToArray();

            unityCache.Collect();
            customCache.Collect();

            // Cache unity types

            Dictionary<string, ComponentGroup> newGroups = new Dictionary<string, ComponentGroup>();
            foreach (ComponentGroup unityGroup in unityGroups)
            {
                newGroups.Add(unityGroup.Name, unityGroup);
            }

            // Cache custom types

            foreach (Type type in allTypes)
            {
                string category = GetTypeCategory(type);

                if (!newGroups.TryGetValue(category, out ComponentGroup group))
                {
                    group = new ComponentGroup(category);
                    newGroups.Add(category, group);
                }

                if (unityCache.Contains(type))
                {
                    continue;
                }

                ComponentType component = new ComponentType(type, true);

                if (!component.IsValid())
                {
                    continue;
                }

                unityCache.Register(component);
                group.Add(component.GUID);
            }
            unityGroups = newGroups.Values.ToArray();
        }

        // - Components

        public int IndexOfComponent(string id, bool isCustom)
        {
            return isCustom
                ? customCache.IndexOf(id)
                : unityCache.IndexOf(id);
        }

        public bool Remove(ComponentType component)
        {
            // TODO: Error Handling

            if (component == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Register a custom component to <see cref="AllCustomComponents"/>.
        /// </summary>
        /// <param name="component">The component to register.</param>
        public bool RegisterCustomComponent(Component component)
        {
            if (component == null)
            {
                Debug.LogError("Cannot register null component.");
                return false;
            }

            MonoScript script = MonoScript.FromMonoBehaviour(component as MonoBehaviour);

            // Invalid type, component being added that shouldn't

            if (script == null)
            {
                return false;
            }

            ComponentType type = new ComponentType(component.GetType(), false);
            type.UpdateType(script);

            // Invalid type, ignore 

            if (!type.IsValid())
            {
                return false;
            }

            return RegisterCustomComponent(type);
        }

        /// <summary>
        /// Register a custom component to <see cref="AllCustomComponents"/>.
        /// </summary>
        /// <param name="component">The component to register.</param>
        public bool RegisterCustomComponent(ComponentType component)
        {
            if (customCache.Contains(component.Type))
            {
                return false;
            }

            customCache.Register(component);

            return true;
        }

        /// <summary>
        /// Add a custom group.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        public void AddCustomGroup(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Attempted to add a custom group with no name.");
                return;
            }

            customGroups.Add(new ComponentGroup(name));
        }

        /// <summary>
        /// Delete a custom group.
        /// </summary>
        /// <param name="index">The index of the group.</param>
        public void DeleteCustomGroup(int index)
        {
            // Make sure index is in range before attempting to remove

            if (index < 0 || index >= customGroups.Count)
            {
                Debug.LogError($"Index out of range of custom groups.");
                return;
            }

            customGroups.RemoveAt(index);
        }

        public void MoveCustomGroup(int index, int newIndex)
        {
            if (index == newIndex)
            {
                return;
            }

            if (index < 0 || index >= customGroups.Count)
            {
                Debug.LogError("Invalid");
                return;
            }

            if (newIndex < 0 || newIndex >= customGroups.Count)
            {
                Debug.LogError("Invalid");
                return;
            }

            ComponentGroup previousGroup = customGroups[newIndex];
            customGroups[newIndex] = customGroups[index];
            customGroups[index] = previousGroup;
        }

        // - Queries

        public bool TryGetComponent(string id, out ComponentType component)
        {
            return unityCache.TryGet(id, out component);
        }

        public bool TryGetComponent(Type type, out ComponentType component) 
        {
            return unityCache.TryGet(type, out component);
        }

        public bool TryGetCustomComponent(string id, out ComponentType component)
        {
            return customCache.TryGet(id, out component);
        }

        public bool TryGetCustomComponent(Type type, out ComponentType component)
        {
            return customCache.TryGet(type, out component);
        }

        /// <summary>
        /// Get the category the given type belongs to.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the category for the type provided.</returns>
        private string GetTypeCategory(Type type)
        {
            // Cannot categorize null type.

            if (type == null)
            {
                return null;
            }

            foreach (var filter in ComponentFilters)
            {
                // Return the filter name if the type is valid

                if (filter.IsValidFilter(type))
                {
                    return filter.Name;
                }
            }

            // Return the default filter, so the type can still be categorized

            return DefaultFilter.Name;
        }
    }
}