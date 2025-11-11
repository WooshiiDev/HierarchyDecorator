using System;
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
        // --- Editor Settings

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

        // --- Settings

        [SerializeField] private bool enableIcons = true;
        [SerializeField, Tooltip("Will clicking the icon toggle the component")] private bool clickToToggleComponent = true;

        [SerializeField] private bool showMissingScriptWarning = true;
        [SerializeField] private DisplayMode showAll = DisplayMode.Unity | DisplayMode.Custom;
        [SerializeField] private bool stackDuplicateIcons;

        [SerializeField] private ComponentGroup[] unityGroups = new ComponentGroup[0];

        [SerializeField] private List<ComponentGroup> customGroups = new List<ComponentGroup>();
        [SerializeField] private ComponentGroup allCustomComponents = new ComponentGroup("All");

        // --- Validation

        [SerializeField] private string unityVersion;
        [SerializeField] private int unityCount; //  The number of Unity Components that have been found.

        // --- Reflection

        private Type[] allTypes = new Type[0];

        // --- Properties

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

        /// <summary>
        /// A component group containing all custom components.
        /// </summary>
        public ComponentGroup AllCustomComponents => allCustomComponents;

        // --- Methods

        // --- Initialization

        /// <summary>
        /// Initialize component data.
        /// </summary>
        public void OnInitialize()
        {
            if (unityVersion != Application.unityVersion)
            {
                unityVersion = Application.unityVersion;
                UpdateData(true);
            }
        }

        /// <summary>
        /// Update Component Data
        /// </summary>
        public void UpdateData(bool forceDirty = false)
        {
            if (allTypes.Length == 0)
            {
                allTypes = TypeCache.GetTypesDerivedFrom(typeof(Component))
                    .Where(t => t.Assembly.FullName.StartsWith("Unity") && !t.IsAbstract)
                    .OrderBy(t => t.Name)
                    .ToArray();
            }

            if (!IsDirty() && !forceDirty)
            {
                return;
            }

            // Update dirty data

            unityCount = allTypes.Length;
            unityVersion = Application.unityVersion;

            Dictionary<string, ComponentGroup> cachedGroups = new Dictionary<string, ComponentGroup>();

            for (int i = 0; i < unityCount; i++)
            {
                Type type = allTypes[i];
                string category = GetTypeCategory(type);

                // If the component does not already exist in a group, create one

                if (!TryGetComponent(type, out ComponentType component))
                {
                    component = new ComponentType(type, true);
                }

                // Create a category group if one does not exist

                if (!cachedGroups.TryGetValue(category, out ComponentGroup group))
                {
                    group = new ComponentGroup(category);
                    cachedGroups.Add(category, group);
                }

                // Add the created component

                group.Add(component);
            }

            unityGroups = cachedGroups.Values.ToArray();

            Debug.LogWarning("HierarchyDecorator components updated due to changes detected.");

            bool IsDirty()
            {
                return
                    allTypes.Length != unityCount ||    // Internal types changed
                    unityGroups.Length == 0;            // No initialization so far
            }
        }

        /// <summary>
        /// Update the components type and content.
        /// </summary>
        /// <param name="updateContent">Should the gui content be updated too?</param>
        public void UpdateComponents(bool updateContent = true)
        {

            for (int i = 0; i < unityGroups.Length; i++)
            {
                unityGroups[i].UpdateCache(updateContent);
            }

            // - Complete Groups

            allCustomComponents.UpdateCache(updateContent);

            // - Custom Groups

            for (int i = 0; i < customGroups.Count; i++)
            {
                customGroups[i].UpdateCache(updateContent);
            }
        }

        // --- Custom Components

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
            // Cannot register null components

            if (component == null)
            {
                Debug.LogError($"Attempted to register null component.");
                return false;
            }

            // Do not register the component if it already exists

            if (allCustomComponents.Contains(component))
            {
                Debug.LogError($"Attempted to register a component that already exists. ({component})");
                return false;
            }

            allCustomComponents.Add(component);
            return true;
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

        // --- Queries

        /// <summary>
        /// Find a component with the given type.
        /// </summary>
        /// <param name="type">The type to find</param>
        /// <param name="component">The component returned if one is found. Otherwise will be null.</param>
        /// <returns>Returns true if a component was found, otherwise will return false.</returns>
        public bool TryGetComponent(Type type, out ComponentType component)
        {
            // If the given type is null, there is nothing to look for

            if (type == null)
            {
                component = null;
                return false;
            }

            for (int i = 0; i < unityGroups.Length; i++)
            {
                ComponentGroup group = unityGroups[i];

                if (group.TryGetComponent(type, out component))
                {
                    return true;
                }
            }

            // Did not find component type, return false

            component = null;
            return false;
        }

        /// <summary>
        /// Find a custom component with the given type.
        /// </summary>
        /// <param name="type">The type to find.</param>
        /// <param name="component">The component returned if one is found. Otherwise this will be null.</param>
        /// <returns>Returns true if a component was found, otherwise will return false.</returns>
        public bool TryGetCustomComponent(Type type, out ComponentType component)
        {
            // If the given type is null, there is nothing to look for

            if (type == null)
            {
                component = null;
                return false;
            }

            // Check all components if showAll is on, otherwise check groups

            if (!DisplayMonoScripts)
            {
                for (int i = 0; i < customGroups.Count; i++)
                {
                    ComponentGroup group = customGroups[i];

                    if (group.TryGetComponent(type, out component))
                    {
                        return true;
                    }
                }
            }

            return allCustomComponents.TryGetComponent(type, out component);
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