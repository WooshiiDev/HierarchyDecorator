using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace HierarchyDecorator
{
    public class TagLayerInfo : HierarchyInfo
    {
        private bool tagEnabled;
        private bool layerEnabled;

        private bool isVertical;
        private bool bothShown => tagEnabled && layerEnabled;
        private bool hasStyle;

        protected override void OnDrawInit(GameObject instance, Settings settings)
        {
            tagEnabled = settings.globalData.showTags;
            layerEnabled = settings.globalData.showLayers;

            TagLayerLayout layout = settings.globalData.tagLayerLayout;
            isVertical = layout == TagLayerLayout.TagAbove || layout == TagLayerLayout.LayerAbove;
        }

        protected override bool DrawerIsEnabled(Settings settings, GameObject instance)
        {
            bool showLayers = settings.globalData.showLayers;
            bool showTags = settings.globalData.showTags;

            if (hasStyle = settings.styleData.HasStyle(instance.name))
            {
                bool showStyleTag = settings.styleData.displayTags;
                bool showStyleLayer = settings.styleData.displayLayers;

                return (showLayers == showStyleLayer) || (showTags == showStyleTag);
            }

            return showLayers || showTags;
        }

        protected override void DrawInfo(Rect rect, GameObject instance, Settings settings)
        {
            bool canDrawTag = hasStyle == (hasStyle && settings.styleData.displayTags);
            bool canDrawLayers = hasStyle == (hasStyle && settings.styleData.displayLayers);

            if (settings.globalData.showTags && canDrawTag)
            {
                DrawTag(rect, instance, settings.globalData.tagLayerLayout);
            }

            if (settings.globalData.showLayers && canDrawLayers)
            {
                DrawLayer(rect, instance, settings.globalData.tagLayerLayout);
            }
        }

        protected override int GetGridCount()
        {
            if (isVertical || tagEnabled != layerEnabled)
            {
                return 3;
            }

            return 6;
        }
        
        // - Drawing elements

        private void DrawTag(Rect rect, GameObject instance, TagLayerLayout layout)
        {
            GenericMenu menu = CreateMenu(InternalEditorUtility.tags, AssignTag);
            rect = GetInfoAreaRect(rect, true, layout);

            DrawInstanceInfo(rect, instance.tag, instance, menu);
        }
        private void DrawLayer(Rect rect, GameObject instance, TagLayerLayout layout)
        {
            GenericMenu menu = CreateMenu(InternalEditorUtility.layers, AssignLayer);
            rect = GetInfoAreaRect(rect, false, layout);

            DrawInstanceInfo(rect, LayerMask.LayerToName(instance.layer), instance, menu);
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
                go.layer = LayerMask.NameToLayer(layer);

                //if (setChildLayers)
                //{
                //    Undo.RecordObjects(Selection.gameObjects, "Layer Updated");

                //    foreach (Transform child in go.transform)
                //    {
                //        child.gameObject.layer = index;
                //    }
                //}

                if (Selection.gameObjects.Length == 1)
                {
                    Selection.SetActiveObjectWithContext(Selection.gameObjects[0], null);
                }
            }
        }

        private void DrawInstanceInfo(Rect rect, string label, GameObject instance, GenericMenu menu)
        {
            EditorGUI.LabelField(rect, label, Style.SmallDropdown);

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
            menu.ShowAsContext();
            e.Use();
        }

        // - Helpers

        private GenericMenu CreateMenu(string[] items, Action<string> onSelect)
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
                case TagLayerLayout.LayerInfront when !isTag:

                    float halfWidth = rect.width * 0.5f;
                    rect.width = halfWidth;

                    break;

                // - Back Element

                case TagLayerLayout.TagInFront when !isTag:
                case TagLayerLayout.LayerInfront when isTag:

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
