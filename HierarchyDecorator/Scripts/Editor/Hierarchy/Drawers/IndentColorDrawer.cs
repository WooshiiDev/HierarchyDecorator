using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
{
    /// <summary>
    /// Draws a hue shifted tint per indentation depth to help visualise hierarchy levels.
    /// </summary>
    public class IndentColorDrawer : HierarchyDrawer
    {
        const float BAR_WIDTH = 2f;

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            return settings.globalData.indentColors.showIndentationColors;
        }

        protected override void DrawInternal(Rect rect, HierarchyItem item, Settings settings)
        {
            // Do not draw over styled instances, they already have a distinct look
            // e.g. by default `= HEADER`, `- subheader`, `+ mini header`
            if (settings.styleData.TryGetStyleFromPrefix(item.DisplayName, out HierarchyStyle prefix))
            {
                return;
            }

            IndentColorSettings data = settings.globalData.indentColors;

            int depth = Mathf.Min(item.CalculateDepth(), data.maxDepth);

            if (data.cycleLength > 0)
            {
                depth %= data.cycleLength;
            }

            Color color = Color.HSVToRGB(
                Mathf.Repeat(data.hueStart + depth * data.hueStep, 1f),
                data.saturation,
                data.value);

            color.a = data.alpha;

            EditorGUI.DrawRect(GetModeRect(rect, data.mode), color);
        }

        // --- Rects

        private Rect GetModeRect(Rect rect, IndentColorMode mode)
        {
            switch (mode)
            {
                case IndentColorMode.Underline:
                    rect.y = rect.yMax - BAR_WIDTH;
                    rect.height = BAR_WIDTH;
                    return rect;

                case IndentColorMode.LeftBar:
                    rect.width = BAR_WIDTH;
                    rect.x -= BAR_WIDTH; // left of icon, so it doesn't get covered by it
                    return rect;
                case IndentColorMode.Fill:
                default:
                    return rect;
            }
        }
    }
}
