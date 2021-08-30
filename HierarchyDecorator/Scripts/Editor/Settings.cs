using System.Collections.Generic;
using UnityEngine;

namespace HierarchyDecorator
{
    /// <summary>
    /// ScriptableObject containing all settings and relevant data for the hierarchy
    /// </summary>
    public class Settings : ScriptableObject, ISerializationCallbackReceiver
    {
        // Settings
        public GlobalData globalData = new GlobalData ();
        public HierarchyStyleData styleData = new HierarchyStyleData ();
        public ComponentData componentData = new ComponentData ();

        // Settings Creation

        private void OnEnable()
        {
            componentData.OnInitialize ();
        }

        /// <summary>
        /// Setup defaults for the new settings asset
        /// </summary>
        internal void SetDefaults(bool isDarkMode)
        {
            componentData.UpdateData ();
            styleData.UpdateStyles (isDarkMode);
        }

        // Serialization

        public void OnBeforeSerialize()
        {
            componentData.UpdateData ();
        }

        public void OnAfterDeserialize()
        {
            componentData.UpdateData ();
        }
    }
}