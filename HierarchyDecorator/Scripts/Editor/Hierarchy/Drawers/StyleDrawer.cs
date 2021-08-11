using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
{
    // TODO: [Wooshii] Need to check for use-cases, handle prefabs correctly

    internal class StyleDrawer : HierarchyDrawer
    {
        private Transform firstTransform;

        private bool previousNeedsFoldout;
        private Transform previousTransform;
        private Rect previousRect;

        private Transform currentTransform;
        private Transform finalTransform;

        protected override void DrawInternal(Rect rect, GameObject instance, Settings _settings)
        {
            // ==========================================================
            // ======================= Assignment =======================
            // ==========================================================

            currentTransform = instance.transform;

            // Draw previous transform
            if (previousNeedsFoldout && previousTransform)
            {
                DrawFoldout (previousRect, currentTransform.parent == previousTransform);
            }

            // We've went back to the start
            if (firstTransform == currentTransform)
            {
                finalTransform = previousTransform;
            }

            // First transform will have the lowest index and no parent
            if (currentTransform.parent == null && currentTransform.GetSiblingIndex () == 0)
            {
                firstTransform = currentTransform;
            }

            // ==========================================================
            // ======================= Background =======================
            // ==========================================================

            // Draw style, and then drawn selection on top
            int instanceID = instance.GetInstanceID ();
            bool hasChildren = currentTransform.childCount > 0;

            bool hasStyle = _settings.globalSettings.twoToneBackground;

            // Only draw the two tone background if there's no style override
            if (_settings.globalSettings.twoToneBackground)
            {
                DrawTwoToneContent (rect, instance, _settings);
                DrawSelection (rect, instanceID);

                hasStyle = true;
            }

            // Draw the style if one is to be applied
            // Have to make sure selection colours are drawn on top when required too
            if (_settings.prefixes.Count > 0 && TryGetStyle (instance, _settings, out PrefixSettings prefix))
            {
                ApplyStyle (rect, instance, prefix, _settings);
                DrawSelection (rect, instanceID);

                hasStyle = true;
            }

            // ==========================================================
            // =================== Previous Transform ===================
            // ==========================================================

            // Final Transform Draw
            if (hasStyle && hasChildren && currentTransform == finalTransform)
            {
                DrawFoldout (rect, false);
            }

            // ==========================================================
            // ========================== Data ==========================
            // ==========================================================

            previousRect = rect;
            previousTransform = currentTransform;
            previousNeedsFoldout = hasStyle && hasChildren;
        }

        protected override bool DrawerIsEnabled(Settings _settings)
        {
            return _settings.prefixes.Count > 0 || _settings.globalSettings.twoToneBackground;
        }

        // Applying custom style

        private void ApplyStyle(Rect rect, GameObject instance, PrefixSettings prefix, Settings settings)
        {
            int len = prefix.prefix.Length;
            string name = instance.name.Trim ().Remove (0, len);

            ModeOptions styleSetting = prefix.GetCurrentMode (EditorGUIUtility.isProSkin);

            // =======================
            // ===== Setup style =====
            // =======================

            Color backgroundColor = styleSetting.backgroundColour;
            Color fontCol = styleSetting.fontColour;

            //Create style to draw
            Rect backgroundRect = GetActualHierarchyWidth (rect);
            Rect labelRect = backgroundRect;

            if (instance.transform.parent != null)
            {
                backgroundRect = rect;
                labelRect = rect;
            }

            // ======================
            // Draw header background
            // ======================

            EditorGUI.DrawRect (backgroundRect, backgroundColor);

            //Draw twice to take into account full width draw
            EditorGUI.LabelField (rect, name.ToUpper (), prefix.style);
        }

        private void DrawFoldout(Rect rect, bool foldout)
        {
            EditorGUI.Foldout (GetToggleRect (rect), foldout, GUIContent.none);
        }

        private void DrawSelection(Rect rect, int instanceID)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            rect = GetActualHierarchyWidth (rect);

            if (Selection.Contains (instanceID))
            {
                EditorGUI.DrawRect (rect, new Color (0.214f, 0.42f, 0.76f, 0.2f));

            }
            else
            if (rect.Contains (mousePosition))
            {
                EditorGUI.DrawRect (rect, new Color (0.3f, 0.3f, 0.3f, 0.2f));
            }
        }

        private void DrawTwoToneContent(Rect rect, GameObject instance, Settings _settings)
        {
            Rect twoToneRect = GetActualHierarchyWidth (rect);

            Handles.BeginGUI ();
            Handles.DrawSolidRectangleWithOutline (twoToneRect, Constants.GetTwoToneColour (rect), Color.clear);
            Handles.EndGUI ();

            // Draw standard content on top of drawn background

            GUIContent content = new GUIContent ();

            Object prefabObj = PrefabUtility.GetPrefabInstanceHandle (instance);

            if (prefabObj != null)
            {
                if (PrefabUtility.GetNearestPrefabInstanceRoot (instance) == instance)
                {
                    content.image = EditorGUIUtility.IconContent ("Prefab Icon").image;

                    Rect iconRect = rect;
                    iconRect.x = rect.width + rect.x;
                    iconRect.width = rect.height;

                    GUI.DrawTexture (iconRect, EditorGUIUtility.IconContent ("tab_next").image, ScaleMode.ScaleToFit);
                }
                else
                {
                    content.image = EditorGUIUtility.IconContent ("GameObject Icon").image;
                }
            }
            else
            {
                content.image = EditorGUIUtility.IconContent ("GameObject Icon").image;
            }

            GUIStyle style = new GUIStyle (Style.ComponentIconStyle);

            if (prefabObj != null)
            {
                style.normal.textColor = (EditorGUIUtility.isProSkin)
                    ? new Color (0.48f, 0.67f, 0.95f, 1f)
                    : new Color (0.1f, 0.3f, 0.7f, 1f);
            }

            if (Selection.Contains (instance))
            {
                style.normal.textColor = Color.white;
            }

            if (!instance.activeInHierarchy)
            {
                style.normal.textColor = (prefabObj != null) ? Constants.InactivePrefabColour : Color.gray;
            }

            Vector2 originalIconSize = EditorGUIUtility.GetIconSize ();
            EditorGUIUtility.SetIconSize (Vector2.one * rect.height);

            EditorGUI.LabelField (rect, content, style);

            rect.x += 18f;
            rect.y--;

            EditorGUI.LabelField (rect, instance.name, style);
            EditorGUIUtility.SetIconSize (originalIconSize);
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

        private bool TryGetStyle(GameObject instance, Settings settings, out PrefixSettings prefix)
        {
            string nameStart = instance.name.TrimStart ().Split (' ')[0];

            for (int i = 0; i < settings.prefixes.Count; i++)
            {
                prefix = settings.prefixes[i];

                if (nameStart.Equals (prefix.prefix))
                {
                    return true;
                }
            }

            prefix = null;
            return false;
        }

    }
}