using System;
using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
{
    public class StateDrawer : HierarchyDrawer
    {
        private static readonly Color SelectionColour = new Color(0.214f, 0.42f, 0.76f, 0.2f);
        private static readonly Color HoverColour = new Color(0.3f, 0.3f, 0.3f, 0.2f);

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            return settings.styleData.Count > 0 || settings.styleData.twoToneBackground;
        }

        protected override void DrawInternal(Rect rect, HierarchyItem item, Settings settings)
        {
            if (settings.styleData.twoToneBackground || settings.styleData.HasStyle(item.DisplayName))
            {
                DrawSelection(rect, item.Transform);
            }
        }

        // --- GUI

        private void DrawSelection(Rect rect, Transform transform)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            rect = GetActualHierarchyWidth(rect);

            bool hasIndex = Array.IndexOf(Selection.gameObjects, transform.gameObject) != -1;

            if (!transform.gameObject.activeInHierarchy)
            {
                Handles.DrawSolidRectangleWithOutline(rect, Constants.InactiveColour, Constants.InactiveColour);
            }

            if (hasIndex)
            {
                EditorGUI.DrawRect(rect, SelectionColour);
            }
            else
            if (rect.Contains(mousePosition))
            {
                EditorGUI.DrawRect(rect, HoverColour);
            }
        }

        // --- Rects

        private Rect GetActualHierarchyWidth(Rect rect)
        {
#if UNITY_2019_3_OR_NEWER
            rect.width += rect.x;
            rect.x = 32f;
#elif UNITY_2019_1_OR_NEWER
            rect.x = 16f;
            rect.width += 48f;
#else
            rect.x = 32f;
            rect.width += 32f;
#endif
            return rect;
        }
    }

}