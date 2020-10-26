using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal abstract class SettingsTab
        {
        protected HierarchyDecoratorSettings settings;
        protected SerializedObject serializedSettings;

        /// <summary>
        /// Constructor used to cache the data required
        /// </summary>
        /// <param name="settings">Current settings used for the hierarchy</param>
        public SettingsTab()
            {
            this.settings = HierarchyDecoratorSettings.GetOrCreateSettings ();
            this.serializedSettings = HierarchyDecoratorSettings.GetSerializedSettings ();

            if (this.settings == null)
                Debug.LogError ("Cannot find settings in project!");
            }

        /// <summary>
        /// The header drawn under the tab selection
        /// </summary>
        public abstract void OnTitleHeaderGUI();

        /// <summary>
        /// The content drawn under the header GUI
        /// Use this when information is required under the HeaderGUI
        /// </summary>
        public abstract void OnTitleContentGUI();

        /// <summary>
        /// The header for the body, if it requires any content
        /// </summary>
        public abstract void OnBodyHeaderGUI();

        /// <summary>
        /// The main content area for the settings tab,
        /// drawn under the body header
        /// </summary>
        public abstract void OnBodyContentGUI();
        }
    }
