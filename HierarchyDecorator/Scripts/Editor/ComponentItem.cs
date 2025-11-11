using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ComponentItem
    {
        // --- Properties
        public Settings HDSettings => HierarchyDecorator.Settings;

        public Component Component { get; private set; }
        public ComponentType Type => GetComponentInfo(HDSettings);
        public bool IsNullComponent { get; private set; }

        public readonly bool IsBehaviour;
        private Behaviour behaviour;

        // - Visual

        public GUIContent Content => Type.Content;

        public string DisplayName => Type.DisplayName;
        public bool IsBuiltIn => Type.IsBuiltIn;
        public bool HasToggle => Type.HasToggle;

        public bool Active { get; private set; }

        // --- Constructor 

        public ComponentItem(Component component)
        {
            Component = component;
            IsNullComponent = component == null;

            IsBehaviour = Component is Behaviour;
            if (IsBehaviour)
            {
                behaviour = Component as Behaviour;
            }

            if (IsNullComponent)
            {
                return;
            }

            IsNullComponent = Type == null;
            Active = GetActiveState();
        }

        // --- Methods

        public bool IsValid()
        {
            return Component == null == IsNullComponent;
        }

        private ComponentType GetComponentInfo(Settings settings)
        {
            if (IsNullComponent)
                return null;

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

            if (IsNullComponent || !HasToggle)
            {
                return true;
            }

            if (IsBehaviour)
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
            if (Active != active)
            {
                Active = active;
                Type.ToggleProperty.SetValue(Component, active);
                EditorUtility.SetDirty(Component.gameObject);
            }
        }

        public void UpdateActiveState()
        {
            if (IsNullComponent || !Type.HasToggle)
            {
                return;
            }

            Active = GetActiveState();
        }
    }
}