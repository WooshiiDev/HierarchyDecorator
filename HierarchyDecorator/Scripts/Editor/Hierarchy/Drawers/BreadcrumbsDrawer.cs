using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HierarchyDecorator
{
    public class BreadcrumbsDrawer : HierarchyDrawer
    {     
        protected override bool DrawerIsEnabled(Settings _settings, GameObject instance)
        {
            return true;
        }
        
        protected override void DrawInternal(Rect rect, GameObject instance, Settings _settings)
        {
            var scene = HierarchyCache.Target;
            var current = scene.Current;

            if (current.Parent == null)
            {
                return;
            }

            Transform transform = current.Transform;

            Handles.color = new Color(0.7f, 0.7f, 0.7f);

            int depth = current.CalculateDepth();
            int startingDepth = (transform.childCount == 0) ? 0 : 1;

            for (int i = startingDepth; i <= depth; i++)
            {
                Rect a = GetVerticalLineRect(rect, i, scene, current);
                Rect b = GetHorizontalLineRect(rect, i, scene, current);

                Handles.DrawLine(a.min, a.max);
                Handles.DrawLine(b.min, b.max);
            }

            Handles.color = Color.white;
        }

        const float HORIZONTAL_OFFSET = 7f;
        const float HORIZONTAL_WIDTH = 5f;

        private static Rect GetVerticalLineRect(Rect rect, int depth, HierarchyCache.SceneCache scene, HierarchyCache.HierarchyData data)
        {
            rect.width = 0f;
            rect.x -= GetDepthOffset(depth);

            int index = data.Transform.GetSiblingIndex();

            float offset = 1f;
            if (index == 0)
            {
                rect.y += offset;
                rect.height -= offset;
            }

            if (depth == 0 && data.IsLastSibling(scene))
            {
                rect.height *= 0.5f;
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