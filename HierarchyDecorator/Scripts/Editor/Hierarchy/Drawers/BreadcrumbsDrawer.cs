using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HierarchyDecorator
{
    public class BreadcrumbsDrawer : HierarchyDrawer
    {
        const float DEPTH_WIDTH = 14f;
        const float DEPTH_OFFSET = 7f;

        const float HORIZONTAL_WIDTH = 6f;

        const float MIN_RECT_X = 60f;

        private static readonly HideFlags IgnoreFoldoutFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

        private static HierarchyItem Item => HierarchyManager.Current;

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            return settings.globalData.showBreadcrumbs;
        }

        protected override void DrawInternal(Rect rect, HierarchyItem item, Settings _settings)
        {
            if (rect.x < MIN_RECT_X)
            {
                return;
            }

            HierarchyItem current = HierarchyManager.Current;

            if (_settings.styleData.TryGetStyleFromPrefix(item.DisplayName, out HierarchyStyle prefix))
            {
                return;
            }

            GlobalData data = _settings.globalData;
            Transform transform = current.Transform;

            int depth = current.CalculateDepth();
            int start = 0;
           
            bool hasVisibleChild = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if ((child.hideFlags & IgnoreFoldoutFlags) == 0)
                {
                    hasVisibleChild = true;
                    break;
                }
            }

            if (hasVisibleChild)
            {
                start = 1;
            }

            int end = data.fullDepthBreadcrumbs.show ? depth : 0;
            for (int i = start; i <= end; i++)
            {
                BreadcrumbSettings breadcrumbs = (i == 0) ? data.instanceBreadcrumbs : data.fullDepthBreadcrumbs;
                Handles.color = breadcrumbs.color;
                DrawTrail(rect, i, breadcrumbs);
            }

            Handles.color = Color.white;
        }

        private void DrawTrail(Rect rect, int depth, BreadcrumbSettings settings)
        {
            if (!settings.show)
            {
                return;
            }

            Rect verticalRect = GetVerticalLineRect(rect, depth);
            DrawLine(verticalRect, settings.style);

            if (settings.displayHorizontal)
            {
                Rect horizontalRect = GetHorizontalLineRect(rect, depth);
                DrawLine(horizontalRect, settings.style);
            }
        }

        private void DrawLine(Rect rect, BreadcrumbStyle style)
        {
            if (rect.x % 2 != 0)
            {
                rect.x--;
            }

            switch (style)
            {
                case BreadcrumbStyle.Solid:
                    DrawRectSolid(rect);
                    break;

                case BreadcrumbStyle.Dash:
                    DrawDashedLine(rect);
                    break;

                case BreadcrumbStyle.Dotted:
                    DrawDottedLine(rect);
                    break;
            }
        }

        private void DrawRectSolid(Rect rect)
        {
            Handles.DrawLine(rect.min, rect.max);
        }

        private void DrawDashedLine(Rect rect)
        {
            SpacedLine(rect, false);
        }

        private void DrawDottedLine(Rect rect)
        {
            SpacedLine(rect, true);
        }

        private void SpacedLine(Rect rect, bool small)
        {
            Vector2 start = rect.min;
            Vector2 end = rect.max;
            Vector2 len = end - start;

            float totalDistance = Vector2.Distance(start, end);

            // Calculate segment sizing

            float segmentLength = small ? 1f : 2f;
            int lengthCount = Mathf.FloorToInt(totalDistance / (segmentLength * 2));
            int count = Mathf.Max(lengthCount, 7);

            // Draw the line using Handles.DrawLine with alternating segments and gaps

            float distanceTraveled = 0;
            for (int i = 0; i < count; i++)
            {
                Vector3 segmentStart = Vector2.Lerp(start, end, distanceTraveled / totalDistance);
                distanceTraveled += segmentLength;
                Vector3 segmentEnd = Vector2.Lerp(start, end, distanceTraveled / totalDistance);
                Handles.DrawLine(segmentStart, segmentEnd);

                distanceTraveled += segmentLength;
            }
        }

        private static Rect GetVerticalLineRect(Rect rect, int depth)
        {
            rect.width = 0f;
            rect.x -= GetDepthX(depth) + 1;

            if (depth == 0 && Item.IsLastSibling())
            {
                rect.height = Mathf.Ceil(rect.height * 0.5f);
            }

            return rect;
        }

        private static Rect GetHorizontalLineRect(Rect rect, int depth)
        {
            if (depth == 0)
            {
                rect.width = HORIZONTAL_WIDTH;
            }
            else
            {
                rect.width = rect.height;

                if (depth == 1 && Item.HasChildren)
                {
                    rect.width -= HORIZONTAL_WIDTH;
                }
            }

            rect.y += rect.height / 2;
            rect.x -= GetDepthX(depth) + 1;

            rect.height = 0;

            return rect;
        }

        private static float GetDepthX(int depth)
        {
            return depth * DEPTH_WIDTH + DEPTH_OFFSET;
        }
    }
}