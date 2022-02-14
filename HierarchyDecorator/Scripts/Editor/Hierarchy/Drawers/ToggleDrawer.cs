using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace HierarchyDecorator
{
    public class ToggleDrawer : HierarchyDrawer
    {
        private bool isHolding = false;
        private bool defaultState = false;
        private int defaultDepth = 0;

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

            if (settings.globalData.activeSwiping)
            {
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
                    HandleSwiping (rect, instance, settings);
                }
            }

            DrawToggles (rect, instance, !settings.globalData.activeSwiping);
        }

        protected override bool DrawerIsEnabled(Settings settings, GameObject instance)
        {
            return settings.globalData.showActiveToggles && PrefabStageUtility.GetCurrentPrefabStage () == null;
        }

        private void DrawToggles(Rect rect, GameObject instance, bool canUpdate = true)
        {
            bool isActive = instance.activeSelf;

            EditorGUI.BeginChangeCheck ();
            {
#if UNITY_2019_1_OR_NEWER
                isActive = EditorGUI.Toggle (rect, isActive, isActive ? Style.Toggle : Style.ToggleMixed);
#else
                isActive = EditorGUI.Toggle (rect, isActive);
#endif
            }
            if (EditorGUI.EndChangeCheck ())
            {
                if (canUpdate)
                {
                    instance.SetActive (isActive);
                }
            }
        }

        private bool IsInteractable(Settings settings, GameObject instance)
        {
            // Just return early as there is no reason to check the other conditions

            if (!settings.globalData.activeSwiping)
            {
                return true;
            }

            // If selectionOnly is enabled, check if the instance has been selected

            if (settings.globalData.swipeSelectionOnly && Selection.gameObjects.Length > 1 && !Selection.Contains (instance))
            {
                return false;
            }

            // Setup defaults if instance

            if (swipedInstances.Count == 0)
            {
                defaultState = instance.activeSelf;
                defaultDepth = GetInstanceDepth (instance.transform);
            }

            // If states have to be the same, check for the same state as the first

            if (settings.globalData.swipeSameState && defaultState != instance.activeSelf)
            {
                return false;
            }
            
            // If depth has to be valid, calculate depth and check

            if (settings.globalData.depthMode != DepthMode.All)
            {    
                bool isValid = false;
                int depth = GetInstanceDepth (instance.transform);

                switch (settings.globalData.depthMode)
                {
                    case DepthMode.SameDepth:
                        isValid = depth == defaultDepth;
                        break;

                    case DepthMode.SameDepthOrLower:
                        isValid = depth >= defaultDepth;
                        break;
                            
                    case DepthMode.SameDepthOrHigher:
                        isValid = depth <= defaultDepth;
                        break;
                }

                if (!isValid)
                {
                    return false;
                }
            }

            return true;
        }

        private void HandleSwiping(Rect rect, GameObject instance, Settings settings)
        {
            if (swipedInstances.Contains (instance))
            {
                return;
            }

            if (rect.Contains (CurrentEvent.mousePosition))
            {
                if (!IsInteractable (settings, instance))
                {
                    return;
                }

                Undo.RecordObject (instance, string.Format ("Changed the active state of an {0}", instance.name));

                swipedInstances.Add (instance);
                instance.SetActive (!instance.activeSelf);
            }
        }

        private int GetInstanceDepth(Transform transform)
        {
            int depth = 0;

            while (transform.parent != null)
            {
                transform = transform.parent;
                depth++;
            }

            return depth;
        }
    }
}
