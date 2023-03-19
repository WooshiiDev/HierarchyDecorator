using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class StyleDrawer : HierarchyDrawer
    {
        protected override void DrawInternal(Rect rect, GameObject instance, Settings settings)
        {
            bool hasStyle = settings.styleData.twoToneBackground;

            // Only draw the two tone background if there's no style override

            if (hasStyle)
            {
                DrawTwoToneContent(rect, instance, settings);
                hasStyle = true;
            }

            // Draw the style if one is to be applied
            // Have to make sure selection colours are drawn on top when required too

            if (settings.styleData.TryGetStyleFromPrefix(instance.name, out HierarchyStyle prefix))
            {
                Rect styleRect = (instance.transform.parent != null)
                    ? rect
                    : GetActualHierarchyWidth(rect);

                HierarchyGUI.DrawHierarchyStyle(prefix, styleRect, rect, instance.name);
                hasStyle = true;
            }

            var target = HierarchyCache.Target;
            var current = target.Current;

            if (current.HasChildren && hasStyle)
            {
                DrawFoldout(rect, current.Foldout);
            }
        }

        protected override bool DrawerIsEnabled(Settings _settings, GameObject instance)
        {
            return _settings.styleData.Count > 0 || _settings.styleData.twoToneBackground;
        }

        // Standards

        private void DrawFoldout(Rect rect, bool foldout)
        {
            GUI.Toggle (GetToggleRect (rect), foldout, GUIContent.none, EditorStyles.foldout);
        }

        // Rect GUI

        private void DrawTwoToneContent(Rect rect, GameObject instance, Settings _settings)
        {
            Rect twoToneRect = GetActualHierarchyWidth (rect);
            var color = _settings.styleData.GetColorMode(EditorGUIUtility.isProSkin);

            Handles.DrawSolidRectangleWithOutline (twoToneRect, color.GetColor(twoToneRect), Color.clear);
            HierarchyGUI.DrawStandardContent (rect, instance);
        }

        // Helpers

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

        private Rect GetToggleRect(Rect rect)
        {
            rect.width = 0f;
            rect.height -= 0f;

            rect.x -= 14f;

            return rect;
        }
    }
}