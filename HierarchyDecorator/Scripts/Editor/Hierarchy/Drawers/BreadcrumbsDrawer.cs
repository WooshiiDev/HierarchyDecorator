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
            var current = HierarchyCache.Target.Current;

            if (current.Parent == null)
            {
                return;
            }

            Handles.color = Color.grey;

            Rect a = GetVerticalLineRect(rect);
            Rect b = GetHorizontalLineRect(rect, current.Transform);
            Handles.DrawLine(a.min, a.max);
            Handles.DrawLine(b.min, b.max);

            Handles.color = Color.white;
        }

        private static Rect GetVerticalLineRect(Rect rect)
        {
            rect.width = 0f;
            rect.x -= 22f;

            return rect;
        }

        private static Rect GetHorizontalLineRect(Rect rect, Transform transform)
        {
            rect = GetVerticalLineRect(rect);
            rect.y += rect.height/2;

            float w = rect.width;
            rect.width = (transform.childCount > 0) ? rect.height/2 : rect.height;
            rect.height = 0f;

            return rect;
        }
    }
}