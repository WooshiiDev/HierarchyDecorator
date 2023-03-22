using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class LayerInfo : HierarchyInfo
    {
        protected override void DrawInfo(Rect rect, GameObject instance, Settings settings)
        {
            if (rect.x < (LabelRect.x + LabelRect.width))
            {
                return;
            }

            EditorGUI.LabelField (rect, LayerMask.LayerToName (instance.layer), Style.SmallDropdown);

            if (settings.globalData.clickToSelectLayer)
            {
                Event e = Event.current;
                bool hasClicked = rect.Contains (e.mousePosition) && e.type == EventType.MouseDown;

                if (!hasClicked)
                {
                    return;
                }

                GameObject[] selection = Selection.gameObjects;

                if (selection.Length < 2)
                {
                    Selection.SetActiveObjectWithContext (instance, null);
                }

                GenericMenu menu = new GenericMenu ();
                bool setChildLayers = settings.globalData.applyChildLayers;

                foreach (System.String layer in Constants.LayerMasks)
                {
                    int index = LayerMask.NameToLayer (layer);

                    menu.AddItem (new GUIContent (layer), false, () =>
                        {
                            Undo.RecordObjects (Selection.gameObjects, "Layer Updated");

                            foreach (GameObject go in Selection.gameObjects)
                            {
                                go.layer = index;

                                if (setChildLayers)
                                {
                                    Undo.RecordObjects (Selection.gameObjects, "Layer Updated");

                                    foreach (Transform child in go.transform)
                                    {
                                        child.gameObject.layer = index;
                                    }
                                }

                                if (Selection.gameObjects.Length == 1)
                                {
                                    Selection.SetActiveObjectWithContext (null, null);
                                }
                            }
                        });
                }

                menu.ShowAsContext ();
                e.Use ();
            }
        }

        protected override int GetGridCount()
        {
            return 3;
        }

        protected override bool DrawerIsEnabled(Settings settings, GameObject instance)
        {
            if (settings.styleData.HasStyle (instance.name) && !settings.styleData.displayLayers)
            {
                return false;
            }

            return settings.globalData.showLayers;
        }
    }
}