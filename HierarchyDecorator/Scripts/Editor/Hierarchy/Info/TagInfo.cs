using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class TagInfo : HierarchyInfo
    {
        protected override void DrawInfo(Rect rect, GameObject instance, Settings settings)
        {
            if (rect.x < (LabelRect.x + LabelRect.width))
            {
                return;
            }

            instance.tag = EditorGUI.TagField(rect, instance.tag, Style.SmallDropdown);
        }

        protected override int GetGridCount()
        {
            return 3;
        }

        protected override bool DrawerIsEnabled(Settings settings, GameObject instance)
        {
            return settings.globalData.showTags;
        }
    }
}