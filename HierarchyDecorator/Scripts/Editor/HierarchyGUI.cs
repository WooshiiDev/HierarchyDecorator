using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
{
    public static class HierarchyGUI
    {
        public static void DrawHierarchyStyle(HierarchyStyle style, Rect styleRect, Rect labelRect, string label, bool removePrefix = true)
        {
            if (removePrefix)
            {
                label = label.Substring (style.prefix.Length).Trim ();
            }

            ModeOptions styleSetting = style.GetCurrentMode (EditorGUIUtility.isProSkin);

            EditorGUI.DrawRect (styleRect, styleSetting.backgroundColour);
            EditorGUI.LabelField (labelRect, label.ToUpper (), style.style);
        }

        public static void DrawStandardContent(Rect rect, GameObject instance)
        {
            // Draw standard content on top of drawn background
            bool isPrefab = PrefabUtility.GetNearestPrefabInstanceRoot (instance) == instance;

            GUIContent content = GetStandardContent (rect, instance, isPrefab);
            GUIStyle style = new GUIStyle (Style.ComponentIconStyle);

            if (isPrefab)
            {
                DrawPrefabArrow (rect);

                style.normal.textColor = (EditorGUIUtility.isProSkin)
                    ? new Color (0.48f, 0.67f, 0.95f, 1f)
                    : new Color (0.1f, 0.3f, 0.7f, 1f);
            }

            if (Selection.Contains (instance))
            {
                style.normal.textColor = Color.white;
            }

            DrawStandardLabel (rect, content, instance.name, style);
        }

        private static void DrawStandardLabel(Rect rect, GUIContent icon, string label, GUIStyle style)
        {
            // Draw Label + Icon
            Vector2 originalIconSize = EditorGUIUtility.GetIconSize ();
            EditorGUIUtility.SetIconSize (Vector2.one * rect.height);
            {
                EditorGUI.LabelField (rect, icon, style);

                rect.x += 18f;
                rect.y--;

                EditorGUI.LabelField (rect, label, style);
            }
            EditorGUIUtility.SetIconSize (originalIconSize);
        }

        private static void DrawPrefabArrow(Rect rect)
        {
            Rect iconRect = rect;
            iconRect.x = rect.width + rect.x;
            iconRect.width = rect.height;

            GUI.DrawTexture (iconRect, EditorGUIUtility.IconContent ("tab_next").image, ScaleMode.ScaleToFit);
        }

        // Content Helpers

        public static GUIContent GetStandardContent(Rect rect, GameObject instance, bool isPrefab)
        {
            return EditorGUIUtility.IconContent (isPrefab ? "Prefab Icon" : "GameObject Icon");
        }

        public static Color GetTwoToneColour(Rect selectionRect)
        {
            bool isEvenRow = selectionRect.y % 32 != 0;

            if (EditorGUIUtility.isProSkin)
            {
                return isEvenRow ? Constants.DarkModeEvenColor : Constants.DarkModeOddColor;
            }
            else
            {
                return isEvenRow ? Constants.LightModeEvenColor : Constants.LightModeOddColor;
            }
        }

        public static Color GetTwoToneColour(int rowIndex)
        {
            bool isEvenRow = rowIndex % 2 != 0;

            if (EditorGUIUtility.isProSkin)
            {
                return isEvenRow ? Constants.DarkModeEvenColor : Constants.DarkModeOddColor;
            }
            else
            {
                return isEvenRow ? Constants.LightModeEvenColor : Constants.LightModeOddColor;
            }
        }
    }
}