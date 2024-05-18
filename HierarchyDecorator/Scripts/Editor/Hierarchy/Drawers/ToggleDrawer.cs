using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif

namespace HierarchyDecorator
{
    public class ToggleDrawer : HierarchyDrawer
    {
        // --- State 

        private bool isHoldingMouse = false;
        private bool isPressingShift = false;

        // Target data for settings based off the first instance selected
        
        private GameObject targetInstance = null;

        private bool targetActiveState = false;
        private int targetDepth = 0;

        // --- Selection

        private Event CurrentEvent => Event.current;
        private List<Object> swipedInstances = new List<Object> ();

        // Properties

        private GameObject[] SelectedInstances => Selection.gameObjects;

        // Methods

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            return settings.globalData.showActiveToggles && PrefabStageUtility.GetCurrentPrefabStage () == null;
        }

        protected override void DrawInternal(Rect rect, HierarchyItem item, Settings settings)
        {
            rect.width = 16f;

#if UNITY_2019_1_OR_NEWER
            rect.x = 32;
#else
            rect.x = 1f;
            rect.y--;
#endif

            // Handle events if they're valid

            GameObject instance = item.GameObject;
            HandleEvent (rect, instance, settings);

            if (isPressingShift)
            {
                HandleMultiToggle (rect, instance, settings);
            }
            else
            if (isHoldingMouse)
            {
                HandleSwiping (rect, instance, settings);
            }

            // Draw toggles

            DrawToggles (rect, instance, !settings.globalData.activeSwiping);
        }

        private void DrawToggles(Rect rect, GameObject instance, bool canUpdate = true)
        {
            bool isActive = instance.activeSelf;

            // Draw toggle

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

        // --- Behaviour 

        private void HandleSwiping(Rect rect, GameObject instance, Settings settings)
        {
            // Ignore instance if the mouse is not interacting with it

            if (!rect.Contains (CurrentEvent.mousePosition))
            {
                return;
            }

            // Do not reselect the instance

            if (swipedInstances.Contains (instance))
            {
                return;
            }

            // If swiping is allowed on the current instance, toggle and register the toggle

            if (IsSwipingValid (settings, instance))
            {
                Undo.RecordObject (instance, string.Format ("Changed the active state of an {0}", instance.name));

                swipedInstances.Add (instance);
                instance.SetActive (!instance.activeSelf);
            }
        }

        private void HandleMultiToggle(Rect rect, GameObject instance, Settings settings)
        {
            int instanceLen = SelectedInstances.Length;

            // If there's only one or no instances selected we don't need to check selected

            if (instanceLen <= 1f)
            {
                return;
            }

            if (rect.Contains (CurrentEvent.mousePosition))
            {
                for (int i = 0; i < instanceLen; i++)
                {
                    instance.SetActive (targetActiveState);
                }
            }
        }

        private void HandleEvent(Rect rect, GameObject instance, Settings settings)
        {
            EventType eventType = CurrentEvent.type;

            if (targetInstance == null && rect.Contains(CurrentEvent.mousePosition))
            {
                if (swipedInstances.Count > 0)
                {
                    swipedInstances.Clear ();
                }

                // Check event conditions

                isPressingShift = CurrentEvent.keyCode == KeyCode.LeftShift;
                isHoldingMouse = (eventType == EventType.MouseDown) && (CurrentEvent.button == 0);

                if (isPressingShift || isHoldingMouse)
                {
                    targetInstance = instance;
                    targetActiveState = targetInstance.activeSelf;
                    targetDepth = GetInstanceDepth (targetInstance.transform);
                }
            }

            bool hasChanged = false;
            if (isPressingShift)
            {
                hasChanged = !(isPressingShift = eventType != EventType.KeyUp);
            }

            if (isHoldingMouse)
            {
                hasChanged = !(isHoldingMouse = eventType != EventType.MouseUp);
            }

            // Reset variables

            if (hasChanged)
            {
                isPressingShift = false;
                isHoldingMouse = false;

                targetInstance = null;
                swipedInstances.Clear ();
            }
        }

        // 

        private bool IsSwipingValid(Settings settings, GameObject instance)
        {
            // --- Active Swiping

            if (settings.globalData.activeSwiping)
            {
                // Cancel out early if we've not prepared and to allow normal functionality

                if (targetInstance == null)
                {
                    return true;
                }

                // If selectionOnly is enabled, check if the instance has been selected

                if (settings.globalData.swipeSelectionOnly)
                {
                    if (SelectedInstances.Length > 1 && !Selection.Contains (instance))
                    {
                        return false;
                    }
                }

                // If states have to be the same, check for the same state as the first

                if (settings.globalData.swipeSameState && targetActiveState != instance.activeSelf)
                {                
                    return false;
                }

                // If depth has to be valid, calculate depth and check

                if (settings.globalData.depthMode != DepthMode.All)
                {
                    int depth = GetInstanceDepth (instance.transform);

                    switch (settings.globalData.depthMode)
                    {
                        case DepthMode.SameDepth:
                            return depth == targetDepth;

                        case DepthMode.SameDepthOrLower:
                            return depth >= targetDepth;

                        case DepthMode.SameDepthOrHigher:
                            return depth <= targetDepth;
                    }
                }
            }

            return true;
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
