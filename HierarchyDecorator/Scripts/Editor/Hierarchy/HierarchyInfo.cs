using UnityEngine;

namespace HierarchyDecorator
{
    internal abstract class HierarchyInfo
    {
        protected readonly Settings settings;

        public HierarchyInfo(Settings settings)
        {
            this.settings = settings;
        }

        public void Draw(Rect rect, int startRow, GameObject instance, Settings settings)
        {
            if (!CanDisplayInfo ())
            {
                return;
            }

            int rowSize = GetRowSize ();
            int rowOffset = 16 * (startRow + rowSize);

            // Get intial position - at the end of the rect
            rect.x += rect.width - rowOffset;

            //Calculate size based on rows
            rect.width = 16f * rowSize;

            DrawInternal (rect, instance);
        }

        /// <summary>
        /// Internal method to draw and setup the GUI to display in the Hierarchy
        /// </summary>
        /// <param name="rect">Hierarchy rectfor the instance</param>
        /// <param name="instance">The current hierarchy instance</param>
        protected abstract void DrawInternal(Rect rect, GameObject instance);

        /// <summary>
        /// Current grid size of the module. This tells the hierarchy how much space is taken up
        /// </summary>
        public abstract int GetRowSize();

        /// <summary>
        /// Condition to validate the display of the info
        /// </summary>
        public abstract bool CanDisplayInfo();
    }
}