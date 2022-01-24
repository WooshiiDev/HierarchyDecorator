using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ToggleDrawer : HierarchyDrawer
    {
        private bool isHolding = false;

        private Event CurrentEvent => Event.current;
        private List<Object> swipedInstances = new List<Object> ();

        protected override void DrawInternal(Rect rect, GameObject instance, Settings settings)
        {
            rect.width = 16f;

#if UNITY_2019_1_OR_NEWER
            rect.x = 32;
#else
            rect.x = 1f;
            rect.y--;
#endif

            int button = CurrentEvent.button;
            EventType eventType = CurrentEvent.type;

            if (!isHolding)
            {
                isHolding = eventType == EventType.MouseDown && button == 0;
            }
            else
            if (eventType == EventType.MouseUp)
            {
                isHolding = false;
                swipedInstances.Clear ();
            }

            if (isHolding)
            {
                HandleSwiping (rect, instance);
            }

            DrawToggles (rect, instance);
        }

        protected override bool DrawerIsEnabled(Settings settings, GameObject instance)
        {
            return settings.globalData.showActiveToggles && PrefabStageUtility.GetCurrentPrefabStage () == null;
        }

        private void HandleSwiping(Rect rect, GameObject instance)
        {
            int instanceID = instance.GetInstanceID ();

            if (swipedInstances.Contains(instance))
            {
                return;
            }

            if (rect.Contains(CurrentEvent.mousePosition))
            {
                Undo.RecordObject (instance, string.Format ("Changed the active state of an {0}", instance.name));

                swipedInstances.Add (instance);
                instance.SetActive (!instance.activeSelf);
            }
        }

        private void DrawToggles(Rect rect, GameObject instance)
        {
            bool isActive = instance.activeSelf;

#if UNITY_2019_1_OR_NEWER
            GUIStyle toggleStyle = isActive ? Style.Toggle : Style.ToggleMixed;

            EditorGUI.BeginChangeCheck ();
            {
                //isActive = 
                    EditorGUI.Toggle (rect, isActive, toggleStyle);
            }
            if (EditorGUI.EndChangeCheck ())
            {
                Undo.RecordObject (instance, string.Format ("Changed the active state of an {0}", instance.name));
                instance.SetActive (isActive);
            }
#else
            EditorGUI.BeginChangeCheck ();
            {
                isActive = EditorGUI.Toggle (rect, isActive);
            }
            if (EditorGUI.EndChangeCheck ())
            {
                instance.SetActive (isActive);
            }
#endif
        }
    }
}
