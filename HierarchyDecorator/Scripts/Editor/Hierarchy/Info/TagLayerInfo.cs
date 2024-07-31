using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace HierarchyDecorator
{
    public class TagLayerInfo : HierarchyInfo
    {
        private const int LABEL_GRID_SIZE = 3;

        // --- Settings

        private bool tagEnabled;
        private bool layerEnabled;

        private bool setChildLayers;

        private bool isVertical;
        private bool tagFirst;

        private bool bothShown => tagEnabled && layerEnabled;

        // --- Methods

        protected override void OnDrawInit(HierarchyItem item, Settings settings)
        {
            setChildLayers = settings.globalData.applyChildLayers;

            TagLayerLayout layout = settings.globalData.tagLayerLayout;
            isVertical = layout == TagLayerLayout.TagAbove || layout == TagLayerLayout.LayerAbove;
            tagFirst = layout == TagLayerLayout.TagInFront;
        }

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            tagEnabled = settings.globalData.showTags;
            layerEnabled = settings.globalData.showLayers;

            if (settings.styleData.HasStyle(item.DisplayName))
            {
                tagEnabled &= settings.styleData.displayTags;
                layerEnabled &= settings.styleData.displayLayers;
            }

            return tagEnabled || layerEnabled;
        }

        protected override void DrawInfo(Rect rect, HierarchyItem item, Settings settings)
        {
            if (tagEnabled)
            {
                DrawTag(rect, item.GameObject, settings.globalData.tagLayerLayout);
            }

            if (layerEnabled)
            {
                DrawLayer(rect, item.GameObject, settings.globalData.tagLayerLayout);
            }
        }

        protected override int CalculateGridCount()
        {
            if (isVertical || !bothShown)
            {
                return LABEL_GRID_SIZE;
            }

            return LABEL_GRID_SIZE * 2;
        }

        protected override bool ValidateGrid()
        {
            if (GridCount < LABEL_GRID_SIZE) // Not big enough for either element
            {
                return false;
            }

            if (GridCount < LABEL_GRID_SIZE * 2 && !isVertical && bothShown)
            {
                tagEnabled = !tagFirst;
                layerEnabled = tagFirst;
            }

            return true;
        }

        // - Drawing elements

        private void DrawTag(Rect rect, GameObject instance, TagLayerLayout layout)
        {
            rect = GetInfoAreaRect(rect, true, layout);
            DrawInstanceInfo(rect, instance.tag, instance, true);
        }

        private void DrawLayer(Rect rect, GameObject instance, TagLayerLayout layout)
        {
            rect = GetInfoAreaRect(rect, false, layout);
            DrawInstanceInfo(rect, LayerMask.LayerToName(instance.layer), instance, false);
        }

        private void DrawInstanceInfo(Rect rect, string label, GameObject instance, bool isTag)
        {
            EditorGUI.LabelField(rect, label, (isVertical && bothShown) ? Style.TinyText : Style.SmallDropdown);

            Event e = Event.current;
            bool hasClicked = rect.Contains(e.mousePosition) && e.type == EventType.MouseDown;

            if (!hasClicked)
            {
                return;
            }

            // Create menu here 

            GenericMenu menu = isTag
                ? CreateMenu(InternalEditorUtility.tags, AssignTag)
                : CreateMenu(InternalEditorUtility.layers, AssignLayer);

            GameObject[] selection = Selection.gameObjects;
            if (selection.Length < 2)
            {
                Selection.SetActiveObjectWithContext(instance, null);
            }

            menu.ShowAsContext();
            e.Use();
        }

        // - Assignment

        private void AssignTag(string tag)
        {
            Undo.RecordObjects(Selection.gameObjects, "Tag Updated");

            foreach (GameObject go in Selection.gameObjects)
            {
                go.tag = tag;

                if (Selection.gameObjects.Length == 1)
                {
                    Selection.SetActiveObjectWithContext(Selection.gameObjects[0], null);
                }
            }
        }

        private void AssignLayer(string layer)
        {
            Undo.RecordObjects(Selection.gameObjects, "Layer Updated");

            foreach (GameObject go in Selection.gameObjects)
            {
                int layerIndex = LayerMask.NameToLayer(layer);
                go.layer = layerIndex;

                if (setChildLayers)
                {
                    Undo.RecordObjects(Selection.gameObjects, "Layer Updated");

                    foreach (Transform child in go.transform)
                    {
                        child.gameObject.layer = layerIndex;
                    }
                }

                if (Selection.gameObjects.Length == 1)
                {
                    Selection.SetActiveObjectWithContext(Selection.gameObjects[0], null);
                }
            }
        }

        // - Helpers

        private static GenericMenu CreateMenu(string[] items, Action<string> onSelect)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < items.Length; i++)
            {
                string item = items[i];
                menu.AddItem(new GUIContent(item), false, () => onSelect.Invoke(item));
            }

            return menu;
        }

        private Rect GetInfoAreaRect(Rect rect, bool isTag, TagLayerLayout layout)
        {
            if (!bothShown)
            {
                return rect;
            }

            switch (layout)
            {
                // Horizontal

                // - Front Element

                case TagLayerLayout.TagInFront when isTag:
                case TagLayerLayout.LayerInFront when !isTag:

                    float halfWidth = rect.width * 0.5f;
                    rect.width = halfWidth;

                    break;

                // - Back Element

                case TagLayerLayout.TagInFront when !isTag:
                case TagLayerLayout.LayerInFront when isTag:

                    halfWidth = rect.width * 0.5f;
                    rect.width = halfWidth;
                    rect.x += halfWidth;

                    break;

                // Vertical

                // - Top element

                case TagLayerLayout.TagAbove when isTag:
                case TagLayerLayout.LayerAbove when !isTag:

                    float halfHeight = rect.height * 0.5f;
                    rect.height = halfHeight;

                    break;

                // - Lower element

                case TagLayerLayout.LayerAbove when isTag:
                case TagLayerLayout.TagAbove when !isTag:

                    halfHeight = rect.height * 0.5f;
                    rect.height = halfHeight;
                    rect.y += halfHeight;

                    break;
            }

            return rect;
        }
    }
}
