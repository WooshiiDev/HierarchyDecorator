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

        private List<string> styleNames = new List<string> ();
        private int styleSelection;

        private string[] tabNames = { "Global Settings", "Prefixes", "Styles", "Shown Components" };
        private int tabSelection;

        private Vector2 scrollView;

        // --- Others ---
        private GUIStyle verticalStyle;

        // --- Component View ---
        private int catSelection;
        private Dictionary<string, List<ComponentType>> componentCatergories = new Dictionary<string, List<ComponentType>> ();
        private List<ComponentType> currentComponents;

        private readonly string[] componentKeywords =
            {
            "2D",

            "Anim",
            "Audio",

            "Collider",

            "Nav",
            "Mesh",

            "Renderer",

            };

        private void OnEnable()
            {
            t = target as HierarchyDecoratorSettings;
            }

        public override void OnInspectorGUI()
            {
            if (prefixes == null)
                {
                prefixes = serializedObject.FindProperty ("prefixes");
                styles = serializedObject.FindProperty ("styles");

                GetSettingNames ();
                }

            if (serializedObject == null)
                return;

            serializedObject.UpdateIfRequiredOrScript ();

            verticalStyle = new GUIStyle (GUI.skin.window)
                {
                padding = new RectOffset (0, 0, 10, 10),

                fontSize = 10
                };

            GUIStyle greyMid = new GUIStyle (EditorStyles.centeredGreyMiniLabel)
                {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                };

            EditorGUILayout.BeginVertical (verticalStyle);
                {
                tabSelection = GUILayout.SelectionGrid (tabSelection, tabNames, tabNames.Length, greyMid);

                if (tabSelection > 0)
                    {
                    EditorGUILayout.Space ();
                    HierarchyDecoratorGUI.LineSpacer ();
                    EditorGUILayout.Space ();
                    }

                switch (tabSelection)
                    {
                    case 0:

                        break;

                    case 1:
                        prefixSelection = GUILayout.SelectionGrid (prefixSelection, prefixNames.ToArray (), 3, EditorStyles.centeredGreyMiniLabel);
                        break;

                    case 2:
                        styleSelection = GUILayout.SelectionGrid (styleSelection, styleNames.ToArray (), 3, EditorStyles.centeredGreyMiniLabel);
                        break;

                    case 3:
                        String[] catergories = componentCatergories.Keys.ToArray ();
                        catSelection = GUILayout.SelectionGrid (catSelection, catergories, 4, EditorStyles.centeredGreyMiniLabel);
                        currentComponents = componentCatergories[catergories[catSelection]];
                        break;
                    }
                }
            EditorGUILayout.EndVertical ();

            EditorGUILayout.Space ();

            EditorGUILayout.BeginVertical (GUI.skin.box);
                {
                EditorGUI.indentLevel++;
                switch (tabSelection)
                    {
                    case 0:
                        {
                        EditorGUI.BeginChangeCheck ();
                            {
                            HierarchyDecoratorGUI.ToggleAuto (ref t.showActiveToggles, "Show GameObject Toggles");
                            HierarchyDecoratorGUI.ToggleAuto (ref t.showComponents, "Show Common Components");
                            HierarchyDecoratorGUI.ToggleAuto (ref t.showLayers, "Show Current Layer");
                            }
                        if (EditorGUI.EndChangeCheck ())
                            {
                            EditorApplication.RepaintHierarchyWindow ();
                            }
                        }
                    break;

                    case 1:
                        DrawSetting ("Prefix Selection", ref prefixSelection, prefixNames.ToArray (), prefixes);
                        break;

                    case 2:
                        DrawSetting ("Style Selection", ref styleSelection, styleNames.ToArray (), styles);
                        break;

                    case 3:
                        DrawComponentSelection ();
                        break;

                    default:
                        break;
                    }
                EditorGUI.indentLevel--;
                }
            EditorGUILayout.EndVertical ();

            }

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

                            serializedObject.ApplyModifiedProperties ();
                            serializedObject.Update ();

                            GetSettingNames ();
                            HierarchyDecorator.GetSettings ();

                            localSelection = property.arraySize - 1;
                        });

                    HierarchyDecoratorGUI.ButtonAction ("Remove Current", EditorStyles.miniButton, () =>
                        {
                            property.DeleteArrayElementAtIndex (localSelection);

                            serializedObject.ApplyModifiedProperties ();
                            serializedObject.Update ();

                            GetSettingNames ();
                            HierarchyDecorator.GetSettings ();

                            localSelection--;
                        });
                    }
                if (EditorGUI.EndChangeCheck ())
                    selection = localSelection;
                }
            EditorGUILayout.EndHorizontal ();

            //Draw current setting, and update when changed
            EditorGUI.BeginChangeCheck ();
                {
                EditorGUILayout.PropertyField (property.GetArrayElementAtIndex (selection));
                }
            if (EditorGUI.EndChangeCheck ())
                {
                EditorApplication.RepaintHierarchyWindow ();
                property.serializedObject.ApplyModifiedProperties ();
                }
            }

        private void DrawComponentSelection()
            {

            EditorGUILayout.BeginVertical ();
                {
                //Draw all components for the catergory
                foreach (ComponentType component in currentComponents)
                    {
                    GUIContent content = new GUIContent ()
                        {
                        text = component.name,
                        image = EditorGUIUtility.ObjectContent (null, component.type).image
                        };

                    //Do not display if it's irrelevant in the first place
                    if (content.image == null || content.image.name == "d_DefaultAsset Icon")
                        {
                        continue;
                        }

                    EditorGUILayout.BeginHorizontal ();
                        {
                        EditorGUI.BeginChangeCheck ();
                            {
                            component.shown = EditorGUILayout.Toggle (component.shown);
                            EditorGUILayout.LabelField (content);
                            }
                        if (EditorGUI.EndChangeCheck ())
                            {
                            EditorApplication.RepaintHierarchyWindow (); //Froce to make sure that the changes apply to the hierarchy instantly
                            }
                        }
                    EditorGUILayout.EndHorizontal ();
                    }
    
                }
            EditorGUILayout.EndVertical ();
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

            //foreach (var item in t.shownComponents)
            foreach (ComponentType item in t.components)
                {
                bool isContained = false;
                for (int i = 0; i < componentKeywords.Length; i++)
                    {
                    string keyword = componentKeywords[i];

                    if (item.type == null)
                        continue;

                    if (item.type.FullName.Contains (keyword))
                        {
                        if (componentCatergories.ContainsKey (keyword))
                            {
                            componentCatergories[keyword].Add (item);
                            }
                        else
                            {
                            componentCatergories.Add (keyword, new List<ComponentType> ()
                                {
                                item
                                });
                            }

                        isContained = true;
                        break;
                        }
                    }

                if (!isContained)
                    {
                    if (componentCatergories.ContainsKey ("General"))
                        {
                        componentCatergories["General"].Add (item);
                        }
                    else
                        {
                        componentCatergories.Add ("General", new List<ComponentType> ()
                            {
                            item
                            });
                        }
                    }
                }

            //Sort just for nicer viewing
            foreach (List<ComponentType> item in componentCatergories.Values)
                {
                item.Sort ();
                }
            }
        }
    }
