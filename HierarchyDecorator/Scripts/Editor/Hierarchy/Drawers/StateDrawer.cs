using System;
using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
{
    public class StateDrawer : HierarchyDrawer
    {
        private static readonly Color SelectionColour = new Color(0.214f, 0.42f, 0.76f, 0.2f);
        private static readonly Color HoverColour = new Color(0.3f, 0.3f, 0.3f, 0.2f);

        protected override bool DrawerIsEnabled(Settings _settings, GameObject instance)
        {
            return _settings.styleData.Count > 0 || _settings.styleData.twoToneBackground;
        }

        protected override void DrawInternal(Rect rect, GameObject instance, Settings settings)
        {
            if (settings.styleData.twoToneBackground || settings.styleData.HasStyle(instance.name))
            {
                DrawSelection(rect, instance.transform);
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
            rect.width += rect.x;

#if UNITY_2019_1_OR_NEWER
            rect.x = 32f;
#else
            rect.x = 0;
            rect.width += 32f;
#endif

            return rect;
        }
    }

}