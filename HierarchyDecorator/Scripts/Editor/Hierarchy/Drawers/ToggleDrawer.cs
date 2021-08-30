using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ToggleDrawer : HierarchyDrawer
    {
        protected override void DrawInternal(Rect rect, GameObject instance, Settings _settings)
        {
            rect.width = 16f;

#if UNITY_2019_1_OR_NEWER
            rect.x = 32;

            bool isActive = instance.activeInHierarchy;
            GUIStyle toggleStyle = isActive ? Style.Toggle : Style.ToggleMixed;

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
            rect.x = 1f;
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

        protected override bool DrawerIsEnabled(Settings _settings, GameObject instace)
        {
           return _settings.globalData.showActiveToggles && PrefabStageUtility.GetCurrentPrefabStage() == null;
        }
    }
}
