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
        /// Has this info drawer initalized for the current instance
        /// </summary>
        protected static bool HasInitialized { get; private set; }

        /// <summary>
        /// Apply the drawer GUI to the instance given
        /// </summary>
        /// <param name="rect">The drawn instance rect</param>
        /// <param name="instance">The instance to draw for</param>
        /// <param name="settings">Hierarchy settings</param>
        protected override void DrawInternal(Rect rect, GameObject instance, Settings settings)
        {
            // Full rect calculation 
            FullRect = GetFullRect (rect);
            LabelRect = GetLabelRect(rect, instance.name, settings);

            // Setup any data before drawing
            HasInitialized = false;
            OnDrawInit (instance, settings);

            // Calculate initial rect
            int gridCount = GetGridCount ();

            rect.x += rect.width - GetGridPosition();
            rect.width = INDENT_SIZE * gridCount;
            HasInitialized = true;

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
    }
}