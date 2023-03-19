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

        private HierarchyCache.SceneCache Scene => HierarchyCache.Target;
        private HierarchyCache.HierarchyData HierarchyInstance => Scene.Current;

        protected override bool DrawerIsEnabled(Settings _settings, GameObject instance)
        {
            return _settings.globalData.showBreadcrumbs;
        }
        
        protected override void DrawInternal(Rect rect, GameObject instance, Settings _settings)
        {
            var scene = HierarchyCache.Target;
            var current = scene.Current;

            if (_settings.styleData.TryGetStyleFromPrefix(instance.name, out HierarchyStyle prefix))
            {
                return;
            }

            GlobalData data = _settings.globalData;

            Transform transform = current.Transform;

            int depth = current.CalculateDepth();

            int start = (transform.childCount == 0) ? 0 : 1;
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

            Rect verticalRect = GetVerticalLineRect(rect, depth, Scene, HierarchyInstance);
            DrawLine(verticalRect, settings.breadcrumbStyle);

            if (settings.showHorizontal)
            {
                Rect horizontalRect = GetHorizontalLineRect(rect, depth, Scene, HierarchyInstance);
                DrawLine(horizontalRect, settings.breadcrumbStyle);
            }
        }

        private void DrawLine(Rect rect, BreadcrumbStyle style)
        {
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
            Vector2 min = rect.min;

            Vector2 len = rect.max - min;
            Vector2 dir = len.normalized;
            Vector2 dot = dir * 2;

            float sqrSize = 64f;
            int count = 4;

            if (small)
            {
                sqrSize = 32;
                count = 8;
                dot = dir;
            }

            count = Mathf.Min(Mathf.CeilToInt(len.sqrMagnitude / sqrSize) + 1, count);
            for (int i = 0; i < count; i++)
            {
                Vector2 a = min + (2 * i * dot);
                Vector2 b = a;
                b += dot;

                Handles.DrawLine(a, b);
            }
        }

        private static Rect GetVerticalLineRect(Rect rect, int depth, HierarchyCache.SceneCache scene, HierarchyCache.HierarchyData data)
        {
            rect.width = 0f;
            rect.x -= GetDepthX(depth);

            int index = data.Transform.GetSiblingIndex();

            if (depth == 0)
            {
                float offset = 1f;
                if (index == 0)
                {
                    rect.y += offset;
                    rect.height -= offset;
                }

                if (depth == 0 && data.IsLastSibling(scene))
                {
                    rect.height /= 2;// Mathf.Floor(rect.height / 2);
                }
            }

            return rect;
        }

        private static Rect GetHorizontalLineRect(Rect rect, int depth, HierarchyCache.SceneCache scene, HierarchyCache.HierarchyData data)
        {
            if (depth == 0)
            {
                rect.width = HORIZONTAL_WIDTH;
            }
            else
            {
                rect.width = rect.height;
            }

            rect.y += rect.height / 2;
            rect.x -= GetDepthX(depth);

            rect.height = 0;

            return rect;
        }

        private static float GetDepthX(int depth)
        {
            return depth * DEPTH_WIDTH + DEPTH_OFFSET;
        }
    }
}