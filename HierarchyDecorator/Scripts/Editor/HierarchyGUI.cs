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
            EditorGUI.LabelField (labelRect, label.ToUpper(), style.style);
        }
    }
}
