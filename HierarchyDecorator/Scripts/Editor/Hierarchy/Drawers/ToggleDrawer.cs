using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;

namespace HierarchyDecorator
{
    internal class ToggleDrawer : HierarchyDrawer
    {
        private const string TOGGLE_ON = "OL Toggle";

#if UNITY_2019_1_OR_NEWER
        private const string TOGGLE_MIXED = "OL ToggleMixed";
#else
        private const string TOGGLE_MIXED = "OL Toggle Mixed";
#endif

        protected override void DrawInternal(Rect rect, GameObject instance, Settings _settings)
        {
            rect.width = 16f;

#if UNITY_2019_1_OR_NEWER
            rect.x = 32;

            bool isActive = instance.activeInHierarchy;
            GUIStyle toggleStyle = isActive ? TOGGLE_ON : TOGGLE_MIXED;

            EditorGUI.BeginChangeCheck ();
            {
                isActive = EditorGUI.Toggle (rect, instance.activeSelf, toggleStyle);
            }
            if (EditorGUI.EndChangeCheck ())
            {
                Undo.RecordObject (instance, string.Format("Changed the active state of an {0}", instance.name));
                instance.SetActive (isActive);
            }
#else
            rect.x = 1;
            rect.y--;

            bool isActive = instance.activeInHierarchy;

            EditorGUI.BeginChangeCheck ();
            {
                isActive = EditorGUI.Toggle (rect, instance.activeSelf);
            }
            if (EditorGUI.EndChangeCheck ())
            {
                instance.SetActive (isActive);
            }
#endif
        }

        protected override bool DrawerIsEnabled(Settings _settings)
        {
           return _settings.globalSettings.showActiveToggles && PrefabStageUtility.GetCurrentPrefabStage() == null;
        }
    }
}
