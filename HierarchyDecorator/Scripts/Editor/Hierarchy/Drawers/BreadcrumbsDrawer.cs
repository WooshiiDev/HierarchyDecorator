using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HierarchyDecorator
{
    public class BreadcrumbsDrawer : HierarchyDrawer
    {
        const float DOT_LENGTH = 1f;

        const float HORIZONTAL_OFFSET = 7f;
        const float HORIZONTAL_WIDTH = 5f;

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
            int end = data.displayForFullDepth ? depth : 0;

            for (int i = start; i <= end; i++)
            {
                Rect verticalRect = GetVerticalLineRect(rect, i, scene, current);

                if (i == 0)
                {
                    Rect horizontalRect = GetHorizontalLineRect(rect, i, scene, current);

                    Handles.color = data.breadcrumbColor;
                    DrawLine(verticalRect, data.breadcrumbStyle);
                    DrawLine(horizontalRect, BreadcrumbStyle.Solid);
                }
                else
                {
                    Handles.color = data.fullDepthColor;

                    Rect dottedRect = verticalRect;
                    dottedRect.y = rect.y;
                    dottedRect.height = rect.height;

                    DrawLine(dottedRect, data.depthStyle);
                }
            }

            Handles.color = Color.white;
        }

        private void DrawLine(Rect rect, BreadcrumbStyle style)
        {
            if (style == BreadcrumbStyle.Solid)
            {
                DrawRectSolid(rect);
            }
            else
            {
                DrawDottedLine(rect);
            }
        }

        private void DrawRectSolid(Rect rect)
        {
            Handles.DrawLine(rect.min, rect.max);
        }

        private void DrawDottedLine(Rect rect)
        {
            Vector2 len = rect.max - rect.min;
            Vector2 offset = len.normalized;

            int count = offset.y == 0 ? 2 : 4;

            offset *= DOT_LENGTH;

            Vector2 seg = len * 0.25f;

            for (int i = 0; i < count; i++)
            {
                Vector2 a = rect.min + (seg * i);
                Vector2 b = a;
                b += offset * 2f;

                Handles.DrawLine(a, b);
            }
        }

      
        private static Rect GetVerticalLineRect(Rect rect, int depth, HierarchyCache.SceneCache scene, HierarchyCache.HierarchyData data)
        {
            rect.width = 0f;
            rect.x -= GetDepthOffset(depth);

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
                    rect.height = Mathf.Floor(rect.height / 2);
                }
            }

            return rect;
        }

        private static Rect GetHorizontalLineRect(Rect rect, int depth, HierarchyCache.SceneCache scene, HierarchyCache.HierarchyData data)
        {
            rect.y += rect.height / 2;
            rect.height = 0;

            rect.x -= GetDepthOffset(depth);
            rect.width = HORIZONTAL_WIDTH;

            return rect;
        }

        private static Rect GetVerticalChildRect(Rect rect)
        {
            rect.width = 0f;
            rect.x += 22f;

            return rect;
        }

        private static float GetDepthOffset(int depth)
        {
            return HORIZONTAL_OFFSET * (depth + 1) + (7f * depth);
        }
    }
}