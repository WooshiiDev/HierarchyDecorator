using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class PrefabOverrideDrawer : HierarchyDrawer
    {
        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings) => settings.styleData.twoToneBackground && PrefabStageUtility.GetCurrentPrefabStage() == null;

        protected override void DrawInternal(Rect rect, HierarchyItem item, Settings _settings)
        {
            rect.width = 2;

#if UNITY_2019_1_OR_NEWER
            rect.x = 32;
#else
            rect.x = 1f;
            rect.y--;
#endif

            GameObject instance = item.GameObject;

            //Draw the little bar that indicates there's modifications in your prefab instance.
            DrawPrefabOverrideBar(rect, instance);
        }

        private void DrawPrefabOverrideBar(Rect rect, GameObject instance)
        {
            PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(instance);
            if (status == PrefabInstanceStatus.Connected)
            {
                bool condition = false;
#if UNITY_2018_3_OR_NEWER
                condition = PrefabUtility.GetObjectOverrides(instance).Count > 0;
#else
                condition = PrefabUtility.GetPropertyModifications(instance).Count > 0;
#endif
                if (condition) EditorGUI.DrawRect(rect, Color.cyan);
            }
        }
    }
}