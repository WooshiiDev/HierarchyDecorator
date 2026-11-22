using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class StyleDrawer : HierarchyDrawer
    {
        private static readonly HideFlags IgnoreFoldoutFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

        protected override void DrawInternal(Rect rect, HierarchyItem item, Settings settings)
        {
            bool hasStyle = settings.styleData.twoToneBackground;

            // Only draw the two tone background if there's no style override

            if (hasStyle)
            {
                DrawTwoToneContent(rect, item, settings);
                hasStyle = true;
            }

            // Draw the style if one is to be applied
            // Have to make sure selection colours are drawn on top when required too

            string displayName = item.DisplayName;
            if (settings.styleData.TryGetStyleFromPrefix(displayName, out HierarchyStyle prefix))
            {
                Rect styleRect = (item.HasParent)
                    ? rect
                    : GetActualHierarchyWidth(rect, item.IsPrefab);

                HierarchyGUI.DrawHierarchyStyle(prefix, styleRect, rect, displayName);
                hasStyle = true;
            }

            if (!item.HasChildren || !hasStyle)
            {
                return;
            }

            // - Need to validate children to check if some can be visible

            bool canShow = false;
            for (int i = 0; i < item.Transform.childCount; i++)
            {
                Transform child = item.Transform.GetChild(i);

                if ((child.hideFlags & IgnoreFoldoutFlags) == 0)
                {
                    canShow = true;
                    break;
                }
            }

            if (!canShow)
            {
                return;
            }

            DrawFoldout(rect, item.Foldout);
        }

        protected override bool DrawerIsEnabled(HierarchyItem instance, Settings settings)
        {
            return settings.styleData.Count > 0 || settings.styleData.twoToneBackground;
        }

        // Standards

        private void DrawFoldout(Rect rect, bool foldout)
        {
            GUI.Toggle (GetToggleRect (rect), foldout, GUIContent.none, EditorStyles.foldout);
        }

        // Rect GUI

        private void DrawTwoToneContent(Rect rect, HierarchyItem item, Settings _settings)
        {
            Rect twoToneRect = GetActualHierarchyWidth (rect, HasPrefabModifications(item));
            var color = _settings.styleData.GetColorMode(EditorGUIUtility.isProSkin);

            Handles.DrawSolidRectangleWithOutline (twoToneRect, color.GetColor(twoToneRect), Color.clear);
            HierarchyGUI.DrawStandardContent (rect, item.GameObject);
        }

        // Helpers

        private Rect GetActualHierarchyWidth(Rect rect, bool isModifiedPrefab)
        {
            rect.width += rect.x;

#if UNITY_2019_1_OR_NEWER
            rect.x = 32f;
#else
            rect.x = 0;
            rect.width += 32f;
#endif

            if (isModifiedPrefab)
            {
                rect.xMin += 2f;
            }

            return rect;
        }

        private Rect GetToggleRect(Rect rect)
        {
            rect.width = 0f;
            rect.height -= 0f;

            rect.x -= 14f;

            return rect;
        }

        private bool HasPrefabModifications(HierarchyItem item)
        {
            if (!item.IsPrefab)
            {
                return false;
            }

            if (PrefabUtility.HasPrefabInstanceAnyOverrides(item.GameObject, false))
            {
                return true;
            }

            return item.PrefabInfo == PrefabInfo.Root && PrefabUtility.GetPrefabInstanceStatus(item.GameObject) == PrefabInstanceStatus.MissingAsset;
        }
    }
}