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

            if (_settings.styleData.TryGetStyleFromPrefix(instance.name, out HierarchyStyle prefix))
            {
                return;
            }

            Transform transform = current.Transform;

            int depth = current.CalculateDepth();
            int startingDepth = (transform.childCount == 0) ? 0 : 1;

            for (int i = startingDepth; i <= depth; i++)
            {
                Rect verticalRect = GetVerticalLineRect(rect, i, scene, current);

                if (i == 0)
                {
                    Rect horizontalRect = GetHorizontalLineRect(rect, i, scene, current);
                  
                    Handles.color = new Color(0.7f, 0.7f, 0.7f);
                    Handles.DrawLine(verticalRect.min, verticalRect.max);
                    Handles.DrawLine(horizontalRect.min, horizontalRect.max);
                }
                else
                {
                    Handles.color = Color.gray;

                    Rect dottedRect = verticalRect;
                    dottedRect.y = rect.y;
                    dottedRect.height = rect.height;

                    DrawDottedLine(dottedRect);
                }
            }

            Handles.color = Color.white;
        }

        const float DOT_LENGTH = 1f;

        private void DrawDottedLine(Rect rect)
        {
            rect.y += DOT_LENGTH;

            Vector2 len = rect.max - rect.min;
            Vector2 seg = len / 4;

            for (int i = 0; i < 4; i++)
            {
                Vector2 a = rect.min + (seg * i);
                a.y -= DOT_LENGTH;

                Vector2 b = rect.min + (seg * i);
                b.y += DOT_LENGTH;

                Handles.DrawLine(a, b);
            }
        }

        const float HORIZONTAL_OFFSET = 7f;
        const float HORIZONTAL_WIDTH = 5f;

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