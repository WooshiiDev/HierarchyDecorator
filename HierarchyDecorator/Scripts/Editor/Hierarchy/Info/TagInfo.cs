using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

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

            EditorGUI.LabelField(rect, instance.tag, Style.SmallDropdown);

            if (settings.globalData.clickToSelectTags)
            {
                Event e = Event.current;
                bool hasClicked = rect.Contains(e.mousePosition) && e.type == EventType.MouseDown;

                if (!hasClicked)
                {
                    return;
                }

                GameObject[] selection = Selection.gameObjects;

                if (selection.Length < 2)
                {
                    Selection.SetActiveObjectWithContext(instance, null);
                }

                GenericMenu menu = new GenericMenu();
                bool setChildLayers = settings.globalData.applyChildLayers;

                foreach (string tag in InternalEditorUtility.tags)
                {
                    menu.AddItem(new GUIContent(tag), false, () =>
                    {
                        Undo.RecordObjects(Selection.gameObjects, "Tag Updated");

                        foreach (GameObject go in Selection.gameObjects)
                        {
                            go.tag = tag;

                            if (Selection.gameObjects.Length == 1)
                            {
                                Selection.SetActiveObjectWithContext(null, null);
                            }
                        }
                    });
                }

                menu.ShowAsContext();
                e.Use();
            }
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