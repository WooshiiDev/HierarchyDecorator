using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;

namespace HierarchyDecorator
{
    internal class ToggleDrawer : HierarchyDrawer
    {
        private const string TOGGLE_ON = "OL Toggle";
        private const string TOGGLE_MIXED = "OL ToggleMixed";

        protected override void DrawInternal(Rect rect, GameObject instance, Settings _settings)
        {
            rect.x = 32;
            rect.width = 16f;

            bool isActive = instance.activeInHierarchy;
            GUIStyle toggleStyle = isActive ? TOGGLE_ON : TOGGLE_MIXED;

            EditorGUI.BeginChangeCheck ();
            {
                isActive = EditorGUI.Toggle (rect, instance.activeSelf, toggleStyle);
            }
            if (EditorGUI.EndChangeCheck ())
            {
                instance.SetActive (isActive);
            }

        }

        protected override bool DrawerIsEnabled(Settings _settings)
        {
           return _settings.globalSettings.showActiveToggles && PrefabStageUtility.GetCurrentPrefabStage() == null;
        }
    }
}
