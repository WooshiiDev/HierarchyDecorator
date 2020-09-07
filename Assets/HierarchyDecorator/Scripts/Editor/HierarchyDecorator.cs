using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
    {
    [InitializeOnLoad]
    internal static class HierarchyDecorator
        {
        //GameObject ID
        private static int currentID;
        private static GameObject currentObject;
        private static bool isPrefab;

        //Component Info
        private static List<Type> returnedComponents;

        //Drawing GUI
        private static int indentIndex = 0;
        private static int currentLayer;

        //Data
        private static HierarchyDecoratorSettings settings;
        private static HierarchyStyle[] styles;

        //// ==== Constructor ====
        static HierarchyDecorator()
            {
            //Initalize
            currentID = 0;
            returnedComponents = new List<Type> ();

            //Add to delegateO h 
            EditorApplication.hierarchyWindowItemOnGUI -= HandleObject;
            EditorApplication.hierarchyWindowItemOnGUI += HandleObject;
            }

        /// <summary>
        /// Main method called for each GameObject element
        /// </summary>
        /// <param name="instanceID">GameObject ID</param>
        /// <param name="selectionRect">Rect area on the inspector</param>
        private static void HandleObject(int instanceID, Rect selectionRect)
            {
            //Call it here to allow editor scripts to load
            //A lot of pain to work around
            if (settings == null)
                GetSettings ();

            currentID = instanceID;
            currentObject = EditorUtility.InstanceIDToObject (currentID) as GameObject;

            //Make sure the object isn't null for whatever reason
            if (currentObject != null)
                {
                //Style
                DrawElementStyle (currentObject, selectionRect);

                if (settings.showActiveToggles)
                    DrawToggles (currentObject, selectionRect);
                    
                }
            }

        #region Unity Prequisites

        private static void ShowChildFoldout(Rect selectionRect, GameObject obj, bool showBack)
            {
            Transform t = obj.transform;

            //Draw child potential first...
            if (t.childCount > 0)
                {
                Rect toggleRect = selectionRect;
                toggleRect.x -= 14;

                EditorGUI.Foldout (toggleRect, false, "");
                }

            //Draw override from child if expanded
            if (t.parent != null)
                {
                if (t == t.parent.GetChild (0))
                    {
                    Rect toggleRect = selectionRect;

                    toggleRect.x -= 28;
                    toggleRect.y -= toggleRect.height;

                    toggleRect.width = toggleRect.height - 1;
                    toggleRect.height--;


                    if (showBack)
                        EditorGUI.DrawRect (toggleRect, settings.globalStyle.GetTwoToneColour (toggleRect));

                    EditorGUI.Foldout (toggleRect, true, "");
                    }
                }
            }

        private static void DisplayGameObjectStatus(Rect selectionRect, GameObject obj)
            {
            GUIContent content = new GUIContent ()
                {
                text = obj.name
                };

            Object prefabObj = PrefabUtility.GetPrefabInstanceHandle (obj);
            isPrefab = PrefabUtility.GetPrefabInstanceStatus (obj) != PrefabInstanceStatus.NotAPrefab;

            if (isPrefab)
                {
                if (PrefabUtility.GetNearestPrefabInstanceRoot(obj) == obj)
                    {
                    content.image = EditorGUIUtility.IconContent ("Prefab Icon").image;

                    Rect selectorRect = selectionRect;
                    selectorRect.x = selectionRect.width + selectionRect.x;
                    selectorRect.width = selectionRect.height;

                    GUI.DrawTexture (selectorRect, EditorGUIUtility.IconContent ("tab_next").image, ScaleMode.ScaleToFit);
                    }
                else
                    {
                    content.image = EditorGUIUtility.IconContent ("GameObject Icon").image;
                    }
                }
            else
                content.image = EditorGUIUtility.IconContent ("GameObject Icon").image;

            GUIStyle style = new GUIStyle (EditorStyles.label);
            
            if (isPrefab)
                {
                if (EditorGUIUtility.isProSkin)
                    style.normal.textColor = new Color (0.48f, 0.67f, 0.95f, 1f);
                else
                    style.normal.textColor = new Color (0.1f, 0.3f, 0.7f, 1f);
                }

            if (!obj.activeInHierarchy)
                style.normal.textColor = Constants.UnactivePrefabColor;

            EditorGUI.LabelField (selectionRect, content, style);
            }

        #endregion

        #region Draw

        private static void DrawElementStyle(GameObject obj, Rect selectionRect)
            {
            //Draw background of default first
            if (settings.globalStyle.twoToneBackground)
                {
                Rect backRect = GetActualHierarchyWidth (selectionRect);
                EditorGUI.DrawRect (backRect, settings.globalStyle.GetTwoToneColour (selectionRect));

                //Apply draw of geneal unity icons
                DisplayGameObjectStatus (selectionRect, obj);
                }


            //Draw prefix styling

            //Final Touches/Fixes etc

            //Everything else as an overlay on top
            bool hasStyle = styles.Any (p => obj.name.StartsWith (p.prefix));

            if (hasStyle)
                {
                for (int i = 0; i < styles.Length; i++)
                    {
                    var style = styles[i];

                    if (obj.name.StartsWith (style.prefix))
                        {
                        Rect elementRect = selectionRect;
                        elementRect.x += 28f;
                        elementRect.width -= 28f * 1.73f;

                        ApplyElementStyle (elementRect, obj.name, style);
                        ShowChildFoldout (selectionRect, obj, false);
                        break;
                        }
                    }
                }

            if (obj.transform.parent != null)
                ShowChildFoldout (selectionRect, obj, styles.Any (p => obj.transform.parent.name.StartsWith (p.prefix)));


            if (!hasStyle)
                {
                if (selectionRect.width < 160f)
                    return;

                if (settings.showComponents)
                    DrawComponentData (currentObject, selectionRect);

                if (currentLayer >= 0 && settings.showLayers)
                    {
                    currentLayer = obj.layer;

                    indentIndex = 2;
                    Rect rect = GetRightLayerMaskRect (selectionRect, indentIndex);
                    EditorGUI.LabelField (rect, LayerMask.LayerToName (currentLayer), EditorStyles.centeredGreyMiniLabel);
                    }
                }

            //EditorGUI.DrawRect (selectionRect, obj.activeSelf ? Color.clear : Constants.UnactiveColor);
            }

        private static void ApplyElementStyle(Rect selectionRect, string name, HierarchyStyle prefix)
            {
            var styleSetting = prefix.GetCurrentSettings ();

            name = RemovePrefix (name, prefix.prefix);

            //Setup style
            Color backgroundColor = styleSetting.backgroundColor;
            Color fontCol = styleSetting.fontColor;

            //Create style to draw
            GUIStyle style = new GUIStyle (settings.GetGUIStyle(prefix.guiStyle))
                {
                alignment = styleSetting.fontAlignment,
                fontSize  = styleSetting.fontSize,
                fontStyle = styleSetting.fontStyle,

                font = styleSetting.font ?? EditorStyles.standardFont,
                };

            style.normal.textColor = fontCol;

            Rect backgroundRect = GetActualHierarchyWidth (selectionRect);
            Rect labelRect = selectionRect = GetHierarchyStyleSize (selectionRect);

            if (currentObject.transform.parent != null)
                {
                backgroundRect = selectionRect;
                labelRect = selectionRect;
                }

            //Draw background and label
            EditorGUI.DrawRect(backgroundRect, backgroundColor);
            EditorGUI.LabelField (labelRect, name.ToUpper (), style);

            //Apply overlay line
            DrawLineStyle (backgroundRect, prefix);
            }

        private static void DrawLineStyle(Rect selectionRect, HierarchyStyle style)
            {
            var setting = style.GetCurrentSettings ();

            switch (setting.displayedLine)
                {
                case LineStyle.NONE:
                    break;

                case LineStyle.TOP:
                    CreateLineSpacer (selectionRect, setting.lineColor, setting.lineHeight);
                    break;

                case LineStyle.BOTTOM:
                    selectionRect.y += EditorGUIUtility.singleLineHeight * 0.85f;
                    CreateLineSpacer (selectionRect, setting.lineColor, setting.lineHeight);
                    break;

                case LineStyle.BOTH:
                    CreateLineSpacer (selectionRect, setting.lineColor, setting.lineHeight);
                    selectionRect.y += EditorGUIUtility.singleLineHeight * 0.85f;
                    CreateLineSpacer (selectionRect, setting.lineColor, setting.lineHeight);
                    break;
                }
            }

        private static void DrawComponentData(GameObject obj, Rect selectionRect)
            {
            //Clear cached components
            returnedComponents.Clear ();

            //Space for layer
            indentIndex = 3;


            //Iterate over all components that exist on the current instance
            Component[] components = obj.GetComponents<Component> ();
            for (int i = 0; i < components.Length; i++)
                {
                var component = components[i].GetType ();

                //Make sure it's allowed to be displayed
                if (component == null || !IsAllowedType(component))
                    continue;


                //Do not need duplicates
                if (returnedComponents.Contains (component))
                    continue;

                returnedComponents.Add (component);

                //Get correct positioning and icon
                Rect drawRect = GetRightRectWithOffset (selectionRect, indentIndex);
                GUIContent content = EditorGUIUtility.ObjectContent (null, component);
                GUI.DrawTexture (drawRect, content.image, ScaleMode.ScaleToFit, true, 0, obj.activeInHierarchy ? Color.white : Constants.UnactiveColor, 0, 0);

                //Increment indent for correct display
                indentIndex++;
                }
            }

        /// <summary>
        /// Draw toggles for the GameObject's active state
        /// </summary>
        private static void DrawToggles(GameObject obj, Rect selectionRect)
            {
            selectionRect.x = 32;
            selectionRect.width = 16f;

            GUIStyle style = new GUIStyle ();
            style.normal.background = (obj.activeSelf)
                ? Textures.Checked
                : Textures.Checkbox;

            //obj.SetActive (EditorGUI.Toggle (selectionRect, obj.activeSelf, style));
            bool active = obj.activeInHierarchy;

            GUIStyle toggleStyle = active
              ? "OL Toggle"
              : "OL ToggleMixed";

            obj.SetActive (EditorGUI.Toggle (selectionRect, obj.activeSelf, toggleStyle));
            }

        #endregion

        #region Rect Helpers

        private static Rect GetActualHierarchyWidth(Rect rect)
            {
            rect.width += rect.x + 4f;
            rect.x = 32f;

            return rect;
            }

        //private static Rect GetActualParentWidth(Rect rect)
        //    {
            
        //    }

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

        private static Rect GetRightLayerMaskRect(Rect rect, int offset)
            {
            rect = GetRightRectWithOffset (rect, offset);

            rect.x -= 4;
            rect.y -= 2;
            rect.width += 48;

            return rect;
            }

        #endregion

        #region Prefix Helpers

        private static string RemoveAllPrefixesFromString(string originalString)
            {
            for (int i = 0; i < styles.Length; i ++)
                originalString = originalString.Trim (styles[i].prefix.ToCharArray ()).Trim ();

            return originalString;
            }

        private static void TogglePrefix(GameObject gameObject, HierarchyStyle style)
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

        private static bool IsAllowedType(Type type)
            {
            for (int i = 0; i < settings.components.Count; i++)
                {
                var component = settings.components[i];

                if (component.type == type && component.shown)
                    return true;
                }

            return false;
            }

        private static void CreateLineSpacer(Rect rect, Color color, float height = 2)
            {
            rect.height = height;

            Color c = GUI.color;

            GUI.color = color;
            EditorGUI.DrawRect (rect, color);
            GUI.color = c;
            }

        private static int GetParentRecursiveCount(GameObject obj)
            {
            Transform t = obj.transform;
            int count = 0;

            while (t != null)
                {
                count++;
                t = t.parent;
                }

            return count;
            }

        #endregion

        #region Menu Options

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

        public static void GetSettings()
            {
            //Cache Styles
            settings = Constants.Settings;
            styles = Constants.Settings.prefixes.ToArray ();

            //Call to make sure it updates without requiring the SO to be opened
            settings.UpdateSettings ();
            }

        }
    }