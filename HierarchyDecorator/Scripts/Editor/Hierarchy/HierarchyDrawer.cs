using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
{
    /// <summary>
    /// Base class for drawing custom GUI to the Hierarchy
    /// </summary>
    public abstract class HierarchyDrawer
    {
        /// <summary>
        /// Display the drawer for the given instance.
        /// </summary>
        /// <param name="rect">The drawn instance rect.</param>
        /// <param name="instance">The instance to draw for.</param>
        /// <param name="settings">Hierarchy Settings.</param>
        public void Draw(Rect rect, GameObject instance, Settings settings)
        {
            if (DrawerIsEnabled (settings, instance))
            {
                DrawInternal (rect, instance, settings);
            }
        }

        /// <summary>
        /// Apply the drawer GUI to the instance given
        /// </summary>
        /// <param name="rect">The drawn instance rect.</param>
        /// <param name="instance">The instance to draw for.</param>
        /// <param name="settings">Hierarchy settings.</param>
        protected abstract void DrawInternal(Rect rect, GameObject instance,  Settings _settings);

        /// <summary>
        /// Is this drawer enabled?
        /// </summary>
        /// <param name="_settings">Hierarchy settings</param>
        /// <returns>Returns true/false depending on if the drawer can be used.</returns>
        protected abstract bool DrawerIsEnabled(Settings _settings, GameObject instance);
    }
}
