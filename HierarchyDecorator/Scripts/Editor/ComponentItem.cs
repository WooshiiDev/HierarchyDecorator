using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentItem
    {
        // --- Properties

        public Component Component { get; private set; }
        public ComponentType Type { get; private set; }
        public bool IsNullComponent { get; private set; }

        // - Visual

        public GUIContent Content => Type.Content;

        public string DisplayName => Type.DisplayName;
        public bool IsBuiltIn => Type.IsBuiltIn;
        public bool CanToggle => Type.HasToggle;

        public bool Active { get; private set; }

        // --- Constructor 

        public ComponentItem(Component component)
        {
            Component = component;
            IsNullComponent = component == null;

            if (IsNullComponent)
            {
                return;
            }

            Type = GetComponentInfo(HierarchyDecorator.Settings);
            Active = GetActiveState();
        }

        // --- Methods

        public bool IsValid()
        {
            return Component == null == IsNullComponent;
        }

        private ComponentType GetComponentInfo(Settings settings)
        {
            var type = Component.GetType();
            if (settings.Components.TryGetComponent(type, out ComponentType c))
            {
                return c;
            }

            if (!settings.Components.TryGetCustomComponent(type, out c))
            {
                settings.Components.RegisterCustomComponent(Component);
                settings.Components.TryGetCustomComponent(type, out c);
            }

            return c;
        }
    
        private bool GetActiveState()
        {
            // Default as enabled 

            if (!Type.HasToggle)
            {
                return true;
            }

            if (Component is Behaviour behaviour)
            {
                return behaviour.enabled;
            }

            return (bool)Type.ToggleProperty.GetValue(Component);
        }

        public void ToggleActive()
        {
            SetActive(!Active);
        }

        public void SetActive(bool active)
        {
            if (!CanToggle)
            {
                return;
            }

            Active = active;
            Type.ToggleProperty.SetValue(Component, active);
        }
    }
}