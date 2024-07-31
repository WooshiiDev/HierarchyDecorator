using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public abstract class HierarchyInfo : HierarchyDrawer
    {
        protected const int INDENT_SIZE = 16;
        protected static int IndentIndex;

        /// <summary>
        /// Full width rect for the current hierarchy instance
        /// </summary>
        protected static Rect FullRect { get; private set; }

        /// <summary>
        /// The rect for the label
        /// </summary>
        protected static Rect LabelRect { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected int GridCount { get; private set; }

        /// <summary>
        /// Apply the drawer GUI to the instance given
        /// </summary>
        /// <param name="rect">The drawn instance rect</param>
        /// <param name="instance">The instance to draw for</param>
        /// <param name="settings">Hierarchy settings</param>
        protected override void DrawInternal(Rect rect, HierarchyItem item, Settings settings)
        {
            // Full rect calculation 

            FullRect = GetFullRect (rect);
            LabelRect = GetLabelRect(rect, item.DisplayName, settings);

            // Setup any data before drawing

            OnDrawInit (item, settings);

            // Calculate initial rect

            GridCount = CalculateGridCount ();
            Rect drawRect = CalculateInfoRect(rect);

            // Draw Info

            if (!ValidateGrid())
            {
                return;
            }

            DrawInfo (drawRect, item, settings);

            // Calculate the next initial index

            IndentIndex += GridCount;
        }

        /// <summary>
        /// Draw the GUI info.
        /// </summary>
        /// <param name="rect">The drawn instance rect</param>
        /// <param name="instance">The instance to draw for</param>
        /// <param name="settings">Hierarchy settings</param>
        protected abstract void DrawInfo(Rect rect, HierarchyItem item, Settings settings);

        /// <summary>
        /// Method that provides any initialisation before drawing to the Hierarchy.
        /// </summary>
        /// <param name="instance">The target instance.</param>
        /// <param name="settings">The settings.</param>
        protected virtual void OnDrawInit(HierarchyItem item, Settings settings)
        {

        }

        /// <summary>
        /// The grid count this info requires when drawing. Use <see cref="GridCount"/> if you need the current value.
        /// </summary>
        /// <returns>Returns the grid usage count.</returns>
        protected abstract int CalculateGridCount();

        protected virtual bool ValidateGrid() { return GridCount > 0; }

        /// <summary>
        /// Get the initial draw position on the grid.
        /// </summary>
        /// <returns>Returns the position this drawer begins at.</returns>
        protected int GetGridPosition()
        {
            return INDENT_SIZE * (IndentIndex + GridCount);
        }

        /// <summary>
        /// Calculate the width of the grid.
        /// </summary>
        /// <returns></returns>
        protected int GetGridWidth()
        {
            return INDENT_SIZE * GridCount;
        }

        /// <summary>
        /// Reset the indent.
        /// </summary>
        public static void ResetIndent()
        {
            IndentIndex = 0;
        }
        
        // Helpers

        private Rect GetFullRect(Rect rect)
        {
            float widthOffset = rect.x - 16f;
            rect.x = 32f;
            rect.width += widthOffset;

            return rect;
        }

        private Rect GetLabelRect(Rect rect, string name, Settings settings)
        {
            GUIStyle labelStyle = null;
            GUIContent labelGUI = new GUIContent();

            bool hasStyle = settings.styleData.TryGetStyleFromPrefix (name, out HierarchyStyle style);

            if (hasStyle)
            {
                labelStyle = style.style;

                int len = style.prefix.Length;
                name = style.FormatString(name.Substring (len + 1, name.Length - len - 1));
            }
            else
            {
                labelStyle = EditorStyles.label;
            }

            labelGUI.text = name;
            rect.width = labelStyle.CalcSize (labelGUI).x + 2f;

            if (!hasStyle)
            {
                rect.width += 16f;
            }
            else
            {
                switch (labelStyle.alignment)
                {
                    case TextAnchor.UpperCenter:
                    case TextAnchor.MiddleCenter:
                    case TextAnchor.LowerCenter:

                        // Calculations:
                        // - Get the center of the full width
                        // - Add the original offset
                        // - Subtract half the content size
                        // - Add 16f to shift over due to the toggle box area

                        float width = (FullRect.width - 16f) * 0.5f;
                        width += rect.x * 0.5f;
                        width -= rect.width * 0.5f;
                        width += 16f;

                        rect.x = width;

                        break;

                    case TextAnchor.UpperRight:
                    case TextAnchor.MiddleRight:
                    case TextAnchor.LowerRight:

                        rect.x = FullRect.width + 17f;
                        rect.x -= rect.width;

                        break;
                }
            }

            return rect;
        }
        
        /// <summary>
        /// Calculate the rect for the info that will be drawn.
        /// </summary>
        /// <param name="rect">The rect to be drawn.</param>
        /// <returns></returns>
        private Rect CalculateInfoRect(Rect rect)
        {
            // Calculate the minimum point and any label overflow

            float xmin = rect.xMax - GetGridPosition();
            float overflow = LabelRect.xMax - xmin;

            // Handle overflow to avoid overlapping labels

            if (overflow > 0)
            {
                GridCount -= Mathf.CeilToInt(overflow / INDENT_SIZE);
                xmin = rect.xMax - GetGridPosition();
            }

            // Calculate the final rect

            rect.x = xmin;
            rect.width = GetGridWidth();

            return rect;
        }
    }
}