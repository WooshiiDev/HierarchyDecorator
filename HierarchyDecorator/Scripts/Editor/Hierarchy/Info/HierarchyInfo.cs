using UnityEngine;

namespace HierarchyDecorator
{
    public abstract class HierarchyInfo : HierarchyDrawer
    {
        protected const int INDENT_SIZE = 16;
        protected static int IndentIndex;

        /// <summary>
        /// Apply the drawer GUI to the instance given
        /// </summary>
        /// <param name="rect">The drawn instance rect</param>
        /// <param name="instance">The instance to draw for</param>
        /// <param name="settings">Hierarchy settings</param>
        protected override void DrawInternal(Rect rect, GameObject instance, Settings settings)
        {
            // Setup any data before drawing
            OnDrawInit (instance, settings);

            // Calculate initial rect
            int gridCount = GetGridCount ();

            rect.x += rect.width - GetGridPosition();
            rect.width = INDENT_SIZE * gridCount;

            // Draw Info
            DrawInfo (rect, instance, settings);

            // Calculate the next initial index
            IndentIndex += gridCount;
        }

        /// <summary>
        /// Draw the GUI info.
        /// </summary>
        /// <param name="rect">The drawn instance rect</param>
        /// <param name="instance">The instance to draw for</param>
        /// <param name="settings">Hierarchy settings</param>
        protected abstract void DrawInfo(Rect rect, GameObject instance, Settings settings);

        /// <summary>
        /// Method that provides any initialisation before drawing to the Hierarchy.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="settings">The settings.</param>
        protected virtual void OnDrawInit(GameObject instance, Settings settings)
        {

        }

        /// <summary>
        /// Get the number of grid elements this info will use when drawing
        /// </summary>
        /// <returns>Returns the grid usage count</returns>
        protected abstract int GetGridCount();

        /// <summary>
        /// Get the initial draw position on the grid
        /// </summary>
        /// <returns>Returns the position this drawer begins at</returns>
        protected int GetGridPosition()
        {
            return INDENT_SIZE * (IndentIndex + GetGridCount ());
        }

        /// <summary>
        /// Reset the indent
        /// </summary>
        public static void ResetIndent()
        {
            IndentIndex = 0;
        }
    }
}