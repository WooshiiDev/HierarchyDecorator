using UnityEngine;

namespace HierarchyDecorator
{
    /// <summary>
    /// ScriptableObject containing all settings and relevant data for the hierarchy
    /// </summary>
    public class Settings : ScriptableObject, ISerializationCallbackReceiver
    {
        // Fields

        public GlobalData globalData = new GlobalData ();
        public HierarchyStyleData styleData = new HierarchyStyleData ();

        [SerializeField]
        private ComponentData components = new ComponentData ();

        // Properties

        public ComponentData Components
        {
            get
            {
                return components;
            }
        }

        // Settings Creation

        private void OnEnable()
        {
            components.OnInitialize ();
        }

        /// <summary>
        /// Setup defaults for the new settings asset
        /// </summary>
        internal void SetDefaults(bool isDarkMode)
        {
            components.UpdateData ();
            styleData.UpdateStyles (isDarkMode);
        }

        // Serialization

        public void OnBeforeSerialize()
        {
            components.UpdateData ();
        }

        public void OnAfterDeserialize()
        {
            components.UpdateData ();
        }
    }
}