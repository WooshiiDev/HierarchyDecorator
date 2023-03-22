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

        [SerializeField] private bool showMissingScriptWarning = true;
        [SerializeField] private DisplayMode showAll = DisplayMode.Unity | DisplayMode.Custom;
        [SerializeField] private bool stackMonoBehaviours;

        [SerializeField] private ComponentGroup[] unityGroups = new ComponentGroup[0];

        [SerializeField] private List<ComponentGroup> customGroups = new List<ComponentGroup>();
        [SerializeField] private ComponentGroup allCustomComponents = new ComponentGroup("All");
        [SerializeField] private ComponentGroup excludedComponents = new ComponentGroup("Excluded");

        // --- Validation

        [SerializeField] private string unityVersion;
        [SerializeField] private bool isDirty;

        // --- Reflection

        private Type[] allTypes;

        // --- Properties

        /// <summary>
        /// The number of Unity Components that have been found.
        /// </summary>
        public int UnityCount;

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

        public bool StackScripts => stackMonoBehaviours;

        /// <summary>
        /// Are components enabled?
        /// </summary>
        public bool Enabled => enableIcons;

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

        /// <summary>
        /// 
        /// </summary>
        public ComponentGroup ExcludedComponents => excludedComponents;

        // --- Methods

        // --- Initialization

        /// <summary>
        /// Initialize component data.
        /// </summary>
        public void OnInitialize()
        {
            isDirty = unityVersion != Application.unityVersion;

            if (isDirty)
            {
                unityVersion = Application.unityVersion;
                Debug.LogWarning ("HierarchyDecorator version changed, updating cached components.");
            }

            UpdateData();
        }

        /// <summary>
        /// Update Component Data
        /// </summary>
        public void UpdateData()
        {
            // Generate all types found in the Unity project

            if (allTypes == null || isDirty)
            {
                allTypes = ReflectionUtility.GetSubTypesFromAssemblies(typeof(Component), 
                        t => t.Assembly.FullName.StartsWith("Unity"))
                    .OrderBy(t => t.Name)
                    .ToArray();
            }

            // Update the components if any are missing

            if (!isDirty)
            {
                isDirty = UnityCount != allTypes.Length || unityGroups.Length == 0;
            }

            if (isDirty)
            {
                // Get the count of unity types 

                UnityCount = allTypes.Length;
                Dictionary<string, ComponentGroup> cachedGroups = new Dictionary<string, ComponentGroup>();

                for (int i = 0; i < UnityCount; i++)
                {
                    // Find the category for the type 
                    // If the group doesn't exist, create one

                    Type type = allTypes[i];
                    string category = GetTypeCategory(type);

                    // If the component does not already exist in a group, create one

                    if (!TryGetComponent(type, out ComponentType component))
                    {
                        component = new ComponentType(type, true);
                    }

                    // Create a group if one does not exist already

                    if (!cachedGroups.TryGetValue(category, out ComponentGroup group))
                    {
                        group = new ComponentGroup(category);
                        cachedGroups.Add(category, group);
                    }

                    group.Add(component);
                    excludedComponents.Add(new ComponentType(type, true));
                }

                excludedComponents.Sort();

                // Assign the new groups and end dirty

                unityGroups = cachedGroups.Values.ToArray();
                isDirty = false;
            }
        }
        
        /// <summary>
        /// Update the components type and content.
        /// </summary>
        /// <param name="updateContent">Should the gui content be updated too?</param>
        public void UpdateComponents(bool updateContent = true)
        {
            // Update built in components first
            
            for (int i = 0; i < unityGroups.Length; i++)
            {
                ComponentGroup group = unityGroups[i];
                UpdateGroup(group);
            }

            UpdateGroup(excludedComponents);

            // Update custom components 

            for (int i = 0; i < allCustomComponents.Count; i++)
            {
                ComponentType component = allCustomComponents.Get(i);

                // Script does not exist, remove it

                if (component.Script == null)
                {
                    allCustomComponents.Remove(i);
                    i--;
                    continue;
                }

                component.UpdateType(component.Script.GetClass(), updateContent);
            }

            for (int i = 0; i < customGroups.Count; i++)
            {
                ComponentGroup group = customGroups[i];

                for (int j = 0; j < group.Count; j++)
                {
                    ComponentType component = group.Get(j);

                    // Do not update if the component already has a type

                    if (component == null)
                    {
                        group.Remove(j);
                        j--;

                        continue;
                    }

                    // No need to update custom components with no assignment
                    // Likewise, do not update components that have valid information

                    if (component.Script == null || component.IsValid())
                    {
                        continue;
                    }

                    // Update the type and the content (if required)

                    component.UpdateType(component.Script.GetClass(), updateContent);
                }
            }

            // Make sure excluded components have all 

            for (int i = 0; i < allTypes.Length; i++)
            {
                Type type = allTypes[i];
                if (!excludedComponents.TryGetComponent(type, out _))
                {
                    ComponentType component = new ComponentType(type, true);
                    excludedComponents.Add(component);
                    component.UpdateContent();

                }
            }

            void UpdateGroup(ComponentGroup group)
            {
                List<Type> types = new List<Type>();
                for (int i = 0; i < group.Count; i++)
                {
                    ComponentType component = group.Get(i);
                    if (component == null)
                    {
                        group.Remove(component);
                        i--;

                        continue;
                    }

                    if (component.IsValid())
                    {
                        continue;
                    }

                    int index = Array.FindIndex(allTypes, type => type.Name == component.Name);
                    if (index == -1)
                    {
                        group.Remove(component);
                        i--;
                        continue;
                    }

                    Type componentType = allTypes[index];

                    if (types.Contains(componentType))
                    {
                        group.Remove(component);
                        i--;
                        continue;
                    }

                    component.UpdateType(componentType, updateContent);
                    types.Add(componentType);
                }
                types.Clear();
            }
        }
        
        /// <summary>
        /// Set component data to dirty. 
        /// </summary>
        public void SetDirty()
        {
            isDirty = true;
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
        public void RegisterCustomComponent(Component component)
        {
            if (component == null)
            {
                Debug.LogError("Cannot register null component.");
                return;
            }

            MonoScript script = MonoScript.FromMonoBehaviour(component as MonoBehaviour);
            ComponentType type = new ComponentType(component.GetType(), false);
            type.UpdateType(script);
            
            RegisterCustomComponent(type);
        }

        /// <summary>
        /// Register a custom component to <see cref="AllCustomComponents"/>.
        /// </summary>
        /// <param name="component">The component to register.</param>
        public void RegisterCustomComponent(ComponentType component)
        {
            // Cannot register null components

            if (component == null)
            {
                Debug.LogError($"Attempted to register null component.");
                return;
            }

            // Do not register the component if it already exists

            if (allCustomComponents.Contains(component))
            {
                Debug.LogError($"Attempted to register a component that already exists.");
                return;
            }

            allCustomComponents.Add(component);
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

        public bool IsExcluded(Type type)
        {
            return excludedComponents.TryGetComponent(type, out ComponentType component) && component.Shown;
        }

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

                for (int j = 0; j < group.Count; j++)
                {
                    component = group.Get(j);
                    
                    // Found component with required type, return

                    if (component.Type == type)
                    {
                        return true;
                    }
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

            component = null;

            if (type == null)
            {
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
            else
            {
                return allCustomComponents.TryGetComponent(type, out component);
            }

            return false;
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