using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
    {
    //Seeing this makes me realize I should have made a smaller name for this.
    [CustomEditor (typeof (HierarchyDecoratorSettings))]
    internal class HierarchyDecoratorSettingsEditor : Editor
        {
        private HierarchyDecoratorSettings t;

        private SerializedProperty prefixes;
        private SerializedProperty styles;

        private List<string> components = new List<string> ();

        // --- GUI ---
        private List<string> prefixNames = new List<string> ();
        private int prefixSelection;
        private int modeSelection;

        private List<string> styleNames = new List<string> ();
        private int styleSelection;

        private string[] tabNames = { "Global", "Prefixes", "Styles", "Icons", "About"};
        private int tabSelection;

        private Vector2 scrollView;

        // --- Others ---
        private GUIStyle verticalStyle;
        private GUIStyle greyMidStyle;

        // --- Component View ---
        private int catSelection;
        private Dictionary<string, List<ComponentType>> componentCatergories = new Dictionary<string, List<ComponentType>> ()
            {
                {"General", new List<ComponentType>() }
            };

        private List<ComponentType> currentComponents;

        private void OnEnable()
            {
            t = target as HierarchyDecoratorSettings;

            //Setup catergories 
            string[] keywords = Constants.componentKeywords;
            for (int i = 0; i < keywords.Length; i++)
                {
                string keyword = keywords[i];

                if (!componentCatergories.ContainsKey (keyword))
                    componentCatergories.Add (keyword, new List<ComponentType> ());
                }
            }

        public override void OnInspectorGUI()
            {
            if (serializedObject == null)
                return;

            serializedObject.UpdateIfRequiredOrScript ();

            if (prefixes == null)
                {
                prefixes = serializedObject.FindProperty ("prefixes");
                styles = serializedObject.FindProperty ("styles");

                GetSettingNames ();

                verticalStyle = new GUIStyle (GUI.skin.window)
                    {
                    padding = new RectOffset (0, 0, 10, 10),

                    fontSize = 10
                    };

                greyMidStyle = new GUIStyle (EditorStyles.centeredGreyMiniLabel)
                    {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    };
                }

            EditorGUILayout.BeginVertical (verticalStyle);
                {
                tabSelection = GUILayout.SelectionGrid (tabSelection, tabNames, tabNames.Length, greyMidStyle);

                EditorGUILayout.Space ();
                HierarchyDecoratorGUI.LineSpacer ();
                EditorGUILayout.Space ();
                    
                switch (tabSelection)
                    {
                    case 0:
                        DrawGlobalSettings ();
                        break;

                    case 1:
                        prefixSelection = GUILayout.SelectionGrid (prefixSelection, prefixNames.ToArray (), 3, EditorStyles.centeredGreyMiniLabel);
                        break;

                    case 2:
                        styleSelection = GUILayout.SelectionGrid (styleSelection, styleNames.ToArray (), 3, EditorStyles.centeredGreyMiniLabel);
                        break;

                    case 3:
                        string[] catergories = componentCatergories.Keys.ToArray ();
                        catSelection = GUILayout.SelectionGrid (catSelection, catergories, 4, EditorStyles.centeredGreyMiniLabel);
                        currentComponents = componentCatergories[catergories[catSelection]];
                        break;
                    }
                }
            EditorGUILayout.EndVertical ();

            EditorGUILayout.Space ();

            EditorGUI.BeginChangeCheck ();
                {
                EditorGUI.indentLevel++;
                    {
                    switch (tabSelection)
                        {
                        //PREFIX SELECTION
                        case 1:
                            EditorGUI.indentLevel++;
                            DrawPrefixBody ();
                            EditorGUI.indentLevel--;
                            break;

                        //STYLES
                        case 2:
                            DrawStyleBody ();
                            break;

                        case 3:
                            DrawComponentBody ();
                            break;

                        case 4:
                            DrawAboutBody ();
                            break;
                        }
                    }
                EditorGUI.indentLevel--;
                }
            if (EditorGUI.EndChangeCheck())
                {
                EditorApplication.RepaintHierarchyWindow ();
                }
            }

        #region Settings Body 

        private void DrawGlobalSettings()
            {
            EditorGUI.BeginChangeCheck ();
                {
                t.globalStyle.OnDraw ();
                }
            if (EditorGUI.EndChangeCheck())
                EditorApplication.RepaintHierarchyWindow ();
            }

        private void DrawPrefixBody()
            {
            EditorGUILayout.BeginVertical (verticalStyle);
                {
                DrawSetting ("Prefix Selection", ref prefixSelection, prefixNames.ToArray (), prefixes);

                if (prefixes.arraySize == 0)
                    {
                    EditorGUILayout.EndVertical ();
                    return;
                    }

                var prefix = prefixes.GetArrayElementAtIndex (prefixSelection);
                var mode = (modeSelection == 0) ? prefix.FindPropertyRelative ("lightMode") : prefix.FindPropertyRelative ("darkMode");
                mode.isExpanded = true;

                var prefixString = prefix.FindPropertyRelative ("prefix");
                var guiStyle = prefix.FindPropertyRelative ("guiStyle");

                EditorGUI.BeginChangeCheck ();
                    {
                    EditorGUILayout.PropertyField (prefixString);
                    EditorGUILayout.PropertyField (guiStyle);

                    modeSelection = GUILayout.SelectionGrid (modeSelection, new[] { "Light", "Dark" }, 2, greyMidStyle);
                    EditorGUILayout.PropertyField (mode);
                    }
                if (EditorGUI.EndChangeCheck ())
                    {
                    GetSettingNames ();
                    prefixes.serializedObject.ApplyModifiedProperties ();
                    }
                }
            EditorGUILayout.EndVertical ();
            }

        private void DrawStyleBody()
            {
            EditorGUILayout.BeginVertical (verticalStyle);
                {
                DrawSetting ("Style Selection", ref styleSelection, styleNames.ToArray (), styles);

                if (styles.arraySize == 0)
                    {
                    EditorGUILayout.EndVertical ();
                    return;
                    }

                var styleSelected = styles.GetArrayElementAtIndex (styleSelection);
                styleSelected.isExpanded = true;

                EditorGUI.BeginChangeCheck ();
                    {
                    EditorGUILayout.PropertyField (styleSelected);
                    }
                if (EditorGUI.EndChangeCheck ())
                    {
                    GetSettingNames ();
                    prefixes.serializedObject.ApplyModifiedProperties ();
                    }
                }
            EditorGUILayout.EndVertical ();
            }

        private void DrawAboutBody()
            {
            Texture banner = Textures.Banner;
            GUIStyle style = new GUIStyle ();

            if (GUILayout.Button ("GitHub Repository", EditorStyles.miniButtonMid))
                Application.OpenURL ("https://github.com/WooshiiDev/HierarchyDecorator/");

            if (GUILayout.Button ("Twitter", EditorStyles.miniButtonMid))
                Application.OpenURL ("https://twitter.com/DaamiaanS");

            GUILayout.Box (banner, style);
            }

        private void DrawComponentBody()
            {
            EditorGUILayout.BeginVertical (verticalStyle);
                {
                //Draw all components for the catergory
                foreach (ComponentType component in currentComponents)
                    {
                    GUIContent content = EditorGUIUtility.ObjectContent (null, component.type);

                    //Do not display if it's irrelevant in the first place
                    if (content.image == null || content.image.name == "d_DefaultAsset Icon")
                        continue;

                    content.text = component.name;
  
                    EditorGUILayout.BeginHorizontal ();
                        {
                        component.shown = EditorGUILayout.Toggle (component.shown);
                        EditorGUILayout.LabelField (content);
                        }
                    EditorGUILayout.EndHorizontal ();
                    }

                }
            EditorGUILayout.EndVertical ();
            }

        #endregion

        private void DrawSetting(string label, ref int selection, string[] selectionArray, SerializedProperty property)
            {
            int localSelection = selection;

            if (property.arraySize == 0)
                {
                HierarchyDecoratorGUI.ButtonAction ("Add New", EditorStyles.toolbarButton, () =>
                    {
                    property.InsertArrayElementAtIndex (property.arraySize);

                    serializedObject.ApplyModifiedProperties ();
                    serializedObject.Update ();

                    GetSettingNames ();
                    HierarchyDecorator.GetSettings ();

                    localSelection = property.arraySize - 1;
                    });

                selection = localSelection;
                return;
                }

            //Draw buttons
            EditorGUILayout.BeginHorizontal ();
                {
                EditorGUILayout.LabelField (label, EditorStyles.largeLabel);

                EditorGUI.BeginChangeCheck ();
                    {
                    HierarchyDecoratorGUI.ButtonAction ("Add New", EditorStyles.miniButton, () =>
                        {
                        property.InsertArrayElementAtIndex (property.arraySize);
                        localSelection = property.arraySize - 1;
                        });

                    HierarchyDecoratorGUI.ButtonAction ("Remove Current", EditorStyles.miniButton, () =>
                        {
                        property.DeleteArrayElementAtIndex (localSelection);
                        localSelection--;

                        if (localSelection < 0)
                            localSelection = 0;
                        });
                    }
                if (EditorGUI.EndChangeCheck ())
                    {
                    selection = localSelection;

                    serializedObject.ApplyModifiedProperties ();
                    serializedObject.Update ();

                    GetSettingNames ();
                    HierarchyDecorator.GetSettings ();
                    }

                }
            EditorGUILayout.EndHorizontal ();
            }

        private void GetSettingNames()
            {
            if (t.components == null)
                {
                Debug.Log ("Component collection is null!");
                return;
                }

            prefixNames.Clear ();
            styleNames.Clear ();

            for (int i = 0; i < prefixes.arraySize; i++)
                {
                prefixNames.Add ("Prefix " + prefixes.GetArrayElementAtIndex (i).displayName);
                }

            for (int i = 0; i < styles.arraySize; i++)
                {
                styleNames.Add (styles.GetArrayElementAtIndex (i).displayName);
                }

            List<string> catergories = new List<string> ();
            string[] keywords = Constants.componentKeywords;

            for (int i = 0; i < t.components.Count; i++)
                {
                ComponentType component = t.components[i];

                bool isContained = false;
                for (int j = 0; j < keywords.Length; j++)
                    {
                    string keyword = keywords[j];

                    if (component.type == null)
                        continue;

                    if (component.type.FullName.Contains (keyword))
                        {
                        componentCatergories[keyword].Add (component);
                        isContained = true;
                        break;
                        }
                    }

                if (!isContained)
                    componentCatergories["General"].Add (component);
                }

            //Sort just for nicer viewing
            foreach (List<ComponentType> item in componentCatergories.Values)
                {
                item.Sort ();
                }
            }
        }
    }
