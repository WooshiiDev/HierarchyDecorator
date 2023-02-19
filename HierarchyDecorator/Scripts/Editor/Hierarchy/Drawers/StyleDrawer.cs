using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
{
    public class StyleDrawer : HierarchyDrawer
    {
        private Transform firstTransform;

        private bool previousNeedsFoldout;
        private Transform previousTransform;
        private Transform currentTransform;

        private int instanceIndex;

        private List<int> validFoldoutCache = new List<int>();
        private Dictionary<int, bool> foldoutCache = new Dictionary<int, bool> ();

        protected override void DrawInternal(Rect rect, GameObject instance, Settings settings)
        {
            currentTransform = instance.transform;

            // We've went back to the start

            if (firstTransform == currentTransform)
            {
                // Find all invalid keys and remove

                List<int> invalidKeys = new List<int> ();
                foreach (var key in foldoutCache.Keys)
                {
                    if (!validFoldoutCache.Contains(key))
                    {
                        invalidKeys.Add (key);
                    }
                }

                for (int i = 0; i < invalidKeys.Count; i++)
                {
                    foldoutCache.Remove (invalidKeys[i]);
                }

                // Clear the valid cache for the next check

                validFoldoutCache.Clear ();
            }

            // First transform will have the lowest index and no parent

            if (currentTransform.parent == null && currentTransform.GetSiblingIndex () == 0)
            {
                firstTransform = currentTransform;
                instanceIndex = 0;
            }

            // Draw previous transform

            if (previousTransform != null)
            {
                int previousInstanceID = previousTransform.GetInstanceID ();

                if (previousNeedsFoldout)
                {
                    foldoutCache[previousInstanceID] = currentTransform.parent == previousTransform;
                }
                else
                if (foldoutCache.ContainsKey (previousInstanceID))
                {
                    foldoutCache.Remove (previousInstanceID);
                }
            }

            // Draw style, and then drawn selection on top

            int instanceID = currentTransform.GetInstanceID();

            bool hasChildren = currentTransform.childCount > 0;
            bool hasStyle = settings.globalData.twoToneBackground;

            // Only draw the two tone background if there's no style override

            if (settings.globalData.twoToneBackground)
            {
                DrawTwoToneContent (rect, instance, settings);
                hasStyle = true;
            }

            // Draw the style if one is to be applied
            // Have to make sure selection colours are drawn on top when required too

            if (settings.styleData.TryGetStyleFromPrefix (instance.name, out HierarchyStyle prefix))
            {
                Rect styleRect = (instance.transform.parent != null)
                    ? rect
                    : GetActualHierarchyWidth (rect);

                HierarchyGUI.DrawHierarchyStyle (prefix, styleRect, rect, instance.name);
                hasStyle = true;
            }

            // Cache values for next instance

            previousTransform = currentTransform;
            previousNeedsFoldout = hasStyle && hasChildren;

            // Draw foldout on top if required

            if (previousNeedsFoldout)
            {
                if (foldoutCache.ContainsKey (instanceID))
                {
                    DrawFoldout (rect, foldoutCache[instanceID]);
                }
                else
                {
                    foldoutCache.Add (instanceID, false);
                }

                validFoldoutCache.Add (instanceID);
            }

            instanceIndex++;
        }

        protected override bool DrawerIsEnabled(Settings _settings, GameObject instance)
        {
            return _settings.styleData.Count > 0 || _settings.globalData.twoToneBackground;
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

            Handles.DrawSolidRectangleWithOutline (twoToneRect, HierarchyGUI.GetTwoToneColour (instanceIndex), Color.clear);
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