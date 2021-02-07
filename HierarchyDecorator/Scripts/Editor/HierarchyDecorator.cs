using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
    {

    internal class InstanceInfo
        {
        public readonly int instanceID;
        public readonly GameObject gameObject;
        public readonly bool isPrefab;
        public bool isExpanded;
        public bool hasStyle;

        public Rect selectionRect;

        public InstanceInfo(int ID, GameObject gameObject, Rect selectionRect, bool isPrefab)
            {
            this.instanceID = ID;
            this.gameObject = gameObject;
            this.selectionRect = selectionRect;
            this.isPrefab = isPrefab;
            }
        }

    [InitializeOnLoad]
    internal static class HierarchyDecorator
        {
        //Instance references
        private static InstanceInfo currentInstance;
        private static InstanceInfo previousInstance;

        private static Transform finalInstance;

        //Component Info
        private static List<Type> returnedComponents;
        private static HierarchyInfo[] info;

        //Drawing GUI
        private static int indentIndex = 0;

        //Data
        private static Settings settings;
        private static PrefixSettings[] styles;

        private static bool IsTwoTone => (settings != null) ? settings.globalSettings.twoToneBackground : false;

        // ==== Constructor ====
        static HierarchyDecorator()
            {
            //Initalize
            returnedComponents = new List<Type> ();

            //Add to delegateO h 
            EditorApplication.hierarchyWindowItemOnGUI -= HandleObject;
            EditorApplication.hierarchyWindowItemOnGUI += HandleObject;
            }

        static void HandleObject(int instanceID, Rect selectionRect)
            {
            //Call it here to allow editor scripts to load
            //A lot of pain to work around
            if (settings == null)
                {
                GetSettings ();

                if (settings == null)
                    {
                    Debug.LogError ("Cannot find settings");
                    return;
                    }

                info = new HierarchyInfo[]
                    {
                    new LayerInfo (settings),
                    new ComponentIconInfo (settings),
                    };

                settings.UpdateCustomComponentData ();
                }

            GameObject gameObject = EditorUtility.InstanceIDToObject (instanceID) as GameObject;

            if (gameObject == null)
                return;

            currentInstance = new InstanceInfo (
                instanceID,
                gameObject,
                selectionRect,
                PrefabUtility.GetPrefabInstanceStatus (gameObject) != PrefabInstanceStatus.NotAPrefab
                );

            if (previousInstance != null && previousInstance.gameObject != null)
                {
                Transform previousTransform = previousInstance.gameObject.transform;
                Transform currentTransform = gameObject.transform;

                previousInstance.isExpanded = previousTransform.GetSiblingIndex () >= currentTransform.GetSiblingIndex ();

                bool isSibling = previousTransform.parent == currentTransform.parent;

                if (previousTransform.root.GetSiblingIndex () > currentTransform.root.GetSiblingIndex ())
                    finalInstance = previousTransform;
                }

            //Draw custom styling
            DrawElementStyle (gameObject, selectionRect);
            }
   
        #region Draw Style

        private static void DrawElementStyle(GameObject obj, Rect selectionRect)
            {
            indentIndex = 0;

            // ========= BACK LAYER =========
            if (IsTwoTone)
                {
                Rect twoToneRect = (settings.globalSettings.stretchWidth) ? GetActualHierarchyWidth (selectionRect) : selectionRect;
                EditorGUI.DrawRect (twoToneRect, settings.globalSettings.GetTwoToneColour (selectionRect));
                }

            // ======== CONTENT LAYER ========
            // Draw selection backgrounds first to make sure no icons or content get into conflict with it
            bool hasStyle = styles.Any (p => obj.name.TrimStart().StartsWith (p.prefix));

            if (hasStyle || !hasStyle && IsTwoTone)
                DrawStandardSelection (selectionRect, obj);

            // ======== OVERLAY LAYER ========
            if (IsTwoTone && !hasStyle)
                DrawStandardContent (selectionRect, obj);


            //Draw element style
            if (hasStyle)
                {
                var style = styles.FirstOrDefault (s => obj.name.TrimStart ().StartsWith (s.prefix));
                ApplyElementStyle (selectionRect, obj.name, style);
                }

            if (PrefabStageUtility.GetCurrentPrefabStage () == null)
                {
                if (settings.globalSettings.showActiveToggles)
                    DrawToggles (obj, selectionRect);
                }

            bool requiresFoldout = (previousInstance != null && previousInstance.hasStyle) || (settings.globalSettings.twoToneBackground && settings.globalSettings.stretchWidth);

            if (requiresFoldout)
                ShowChildFoldout (selectionRect, obj, false);       
               
            //Everything else, which is generally custom stuffs 
            if (!hasStyle && selectionRect.width > 160f)
                {
                foreach (var infoGUI in info)
                    {
                    infoGUI.Draw (selectionRect, indentIndex, obj, settings);

                    if (infoGUI.CanDisplayInfo ())
                        indentIndex += infoGUI.GetRowSize ();
                    }
                }

         
            previousInstance = currentInstance;
            previousInstance.hasStyle = hasStyle;
            }

        private static void ApplyElementStyle(Rect selectionRect, string name, PrefixSettings prefix)
            {
            var styleSetting = prefix.GetCurrentSettings ();


            name = RemovePrefix (name, prefix.prefix);

            //Setup style
            Color backgroundColor = styleSetting.backgroundColor;
            Color fontCol = styleSetting.fontColor;

            //Create style to draw
            GUIStyle style = new GUIStyle (settings.GetGUIStyle (prefix.guiStyle))
                {
                alignment = styleSetting.fontAlignment,
                fontSize = styleSetting.fontSize,
                fontStyle = styleSetting.fontStyle,

                font = styleSetting.font ?? EditorStyles.standardFont,
                };

            style.normal.textColor = fontCol;

            Rect backgroundRect = (settings.globalSettings.stretchWidth) ? GetActualHierarchyWidth (selectionRect) : selectionRect;
            Rect labelRect = backgroundRect;

            if (settings.globalSettings.stretchWidth)
                {
                labelRect.x += 24f;
                labelRect.width -= 48f;
                }

            if (currentInstance.gameObject.transform.parent != null)
                {
                backgroundRect = selectionRect;
                labelRect = selectionRect;
                }
      
            //Draw header background
            //backgroundRect = GetAlignedRect (backgroundRect, styleSetting.widthScale, style.alignment);
            EditorGUI.DrawRect (backgroundRect, backgroundColor);

            if (styleSetting.hasOutline)
                {
                Handles.BeginGUI ();
                Handles.DrawSolidRectangleWithOutline (backgroundRect, Color.clear, styleSetting.outlineColor);
                Handles.EndGUI ();
                }

            //Draw twice to take into account full width draw
            EditorGUI.LabelField (GetActualHierarchyWidth (selectionRect), "", style);
            EditorGUI.LabelField (labelRect, name.ToUpper (), style);

         

            }

        /// <summary>
        /// Draw toggles for the GameObject's active state
        /// </summary>
        private static void DrawToggles(GameObject obj, Rect rect)
            {
            rect.x = 32;
            rect.width = 16f;

            bool isActive = obj.activeInHierarchy;
            GUIStyle toggleStyle = isActive ? "OL Toggle" : "OL ToggleMixed";

            EditorGUI.BeginChangeCheck ();
            isActive = EditorGUI.Toggle (rect, obj.activeSelf, toggleStyle);
            if (EditorGUI.EndChangeCheck())
                obj.SetActive (isActive);
            }

        #endregion

        #region Unity Prequisites

        private static void ShowChildFoldout(Rect selectionRect, GameObject obj, bool showBack)
            {
            Transform currentTransform = obj.transform;

            // Usecase for the final instance in the hierarchy
            if (currentTransform == finalInstance && currentTransform.childCount > 0)
                DrawFoldout (currentInstance.selectionRect, false);

            // No previous instance if it's the first object
            if (previousInstance == null || previousInstance.gameObject == null)
                return;

            Transform previousTransform = previousInstance.gameObject.transform;

            if (previousTransform.childCount == 0 || previousTransform == finalInstance)
                return;

            DrawFoldout (previousInstance.selectionRect, previousInstance.isExpanded);
            }

        private static void DrawStandardContent(Rect selectionRect, GameObject obj)
            {
            GUIContent content = new GUIContent ()
                {
                text = obj.name
                };

            Object prefabObj = PrefabUtility.GetPrefabInstanceHandle (obj);

            if (currentInstance.isPrefab)
                {
                if (PrefabUtility.GetNearestPrefabInstanceRoot (obj) == obj)
                    {
                    content.image = EditorGUIUtility.IconContent ("Prefab Icon").image;

                    Rect iconRect = selectionRect;
                    iconRect.x = selectionRect.width + selectionRect.x;
                    iconRect.width = selectionRect.height;

                    GUI.DrawTexture (iconRect, EditorGUIUtility.IconContent ("tab_next").image, ScaleMode.ScaleToFit);
                    }
                else
                    {
                    content.image = EditorGUIUtility.IconContent ("GameObject Icon").image;
                    }
                }
            else
                content.image = EditorGUIUtility.IconContent ("GameObject Icon").image;

            GUIStyle style = new GUIStyle (Style.componentIconStyle);

            if (currentInstance.isPrefab)
                {
                if (EditorGUIUtility.isProSkin)
                    style.normal.textColor = new Color (0.48f, 0.67f, 0.95f, 1f);
                else
                    style.normal.textColor = new Color (0.1f, 0.3f, 0.7f, 1f);
                }

            if (Selection.Contains (obj))
                style.normal.textColor = Color.white;

            if (!obj.activeInHierarchy)
                style.normal.textColor = (currentInstance.isPrefab) ? Constants.UnactivePrefabColor : Color.gray;

            Vector2 originalIconSize = EditorGUIUtility.GetIconSize ();
            EditorGUIUtility.SetIconSize (Vector2.one * selectionRect.height);
            EditorGUI.LabelField (selectionRect, content, style);

            EditorGUIUtility.SetIconSize(originalIconSize);
            }

        private static void DrawStandardSelection(Rect selectionRect, GameObject obj)
            {
            Vector2 mousePos = Event.current.mousePosition;
             
            if (Selection.Contains (obj))
                EditorGUI.DrawRect (GetActualHierarchyWidth (selectionRect), new Color (0.214f, 0.42f, 0.76f, 1f));
            else
            if (selectionRect.Contains(mousePos))
                EditorGUI.DrawRect (GetActualHierarchyWidth (selectionRect), new Color (0.3f, 0.3f, 0.3f, 0.2f));
            }
   
        #endregion

        #region Component Draw

        private static void DrawMonoBehaviour(Rect rect, Component component)
            {
            Type type = component.GetType ();
            MonoScript script = GetCustomType (type);

            if (script == null)
                return;


            string path = null;
            Texture icon = null;

            if (settings.globalSettings.showAllComponents || script != null)
                {
                path = AssetDatabase.GetAssetPath (MonoScript.FromMonoBehaviour (component as MonoBehaviour));
                icon = AssetDatabase.GetCachedIcon (path);

                returnedComponents.Add (type);
                returnedComponents.Add (typeof(MonoBehaviour));
                }
            else
                {
                return;
                }
            
            GUIContent content = new GUIContent (icon);

            //Draw and increment icon placement
            GUI.DrawTexture (rect, content.image, ScaleMode.ScaleToFit, true, 0, currentInstance.gameObject.activeInHierarchy ? Color.white : Constants.UnactiveColor, 0, 0);
            indentIndex++;

            }

        private static void DrawComponent(Rect rect, Component component)
            {
            Type type = component.GetType ();

            //Make sure it's allowed to be displayed
            if (type == null || !IsAllowedType (type))
                return;

            returnedComponents.Add (type);

            GUIContent content = EditorGUIUtility.ObjectContent (null, type);

            //Draw and increment icon placement
            GUI.DrawTexture (rect, content.image, ScaleMode.ScaleToFit, true, 0, currentInstance.gameObject.activeInHierarchy ? Color.white : Constants.UnactiveColor, 0, 0);
            indentIndex++;

            //DrawOutline (rect);
            }

        private static bool DrawFoldout(Rect selectionRect, bool foldout)
            {
            selectionRect.width = selectionRect.height;
            selectionRect.x -= 14f;

            foldout = EditorGUI.Foldout (selectionRect, foldout, "");

            return foldout;
            }

        #endregion

        #region Rect Helpers

        private static Rect GetActualHierarchyWidth(Rect rect)
            {
            rect.width += rect.x + 4f;
            rect.x = 32f;

            return rect;
            }

        private static Rect GetHierarchyStyleSize(Rect rect)
            {
            rect.x -= 28f;
            rect.width += 44f;

            return rect;
            }

        private static Rect GetRightRectWithOffset(Rect rect, int offset)
            {
            var newRect = new Rect (rect);
            newRect.width = newRect.height;
            newRect.x = rect.x + rect.width - (rect.height * offset) - 16;

            return newRect;
            }

        #endregion

        #region Prefix Helpers

        private static string RemoveAllPrefixesFromString(string originalString)
            {
            for (int i = 0; i < styles.Length; i ++)
                originalString = originalString.Trim (styles[i].prefix.ToCharArray ()).Trim ();

            return originalString;
            }

        private static void TogglePrefix(GameObject gameObject, PrefixSettings style)
            {
            string str = gameObject.name;

            if (str.StartsWith (style.prefix))
                str = RemoveAllPrefixesFromString (str);
            else
                str = $"{style.prefix} {RemoveAllPrefixesFromString (str)}";

            gameObject.name = str;
            }

        private static string RemovePrefix(string str, string prefix)
            {
            return str.Remove (str.IndexOf (prefix), prefix.Length);
            }

        #endregion

        #region Util

        //Is the type we're looking for in either the global components and shown
        //Or is it in the custom defined
        private static bool IsAllowedType(Type type)
            {
            //Check global
            for (int i = 0; i < settings.unityComponents.Count; i++)
                {
                var component = settings.unityComponents[i];

                if (component.type == type)
                    return component.shown;
                }

            //Check custom types
            for (int i = 0; i < settings.customComponents.Count; i++)
                {
                var component = settings.customComponents[i];

                if (component == null)
                    continue;

                if (component.name == type.Name)
                    return component.shown;
                }


            return false;
            }

        // Find the correct custom type in the custom list
        private static MonoScript GetCustomType(Type type)
            {
            for (int i = 0; i < settings.customComponents.Count; i++)
                {
                var customType = settings.customComponents[i];

                if (type.Name == customType.name && customType.shown)
                    return customType.script;
                }

            return null;
            }

        private static void GetSettings()
            {
            //Cache Styles
            settings = Settings.GetOrCreateSettings();

            if (settings == null)
                {
                Debug.LogError ("Cannot find settings");
                return;
                }

            styles = settings.prefixes.ToArray ();

            //Call to make sure it updates without requiring the SO to be opened
            settings.UpdateSettings ();
            }

        #endregion

        #region Menu Items

        [MenuItem ("GameObject/Designer/Toggle Header %h", priority = 100)]
        private static void CreateHeader()
            {
            TogglePrefix (Selection.gameObjects[0], styles[0]);
            }

        [MenuItem ("GameObject/Designer/Toggle Subheader %j", priority = 100)]
        private static void CreateSubheader()
            {
            TogglePrefix (Selection.gameObjects[0], styles[1]);
            }
  
        #endregion
        }
    }