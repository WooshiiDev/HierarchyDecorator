using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HierarchyDecorator
    {
    internal class IconTab : SettingsTab
        {
        private static Dictionary<string, List<ComponentType>> componentCatergories;
        private static bool[] catergoryFoldout;

        // References
        private SerializedProperty customComponents;
        private SerializedProperty unityComponents;
        private ReorderableList componentList;

        public IconTab() : base ("Icons", "d_FilterByType")
            {
            componentCatergories = new Dictionary<string, List<ComponentType>> ();

            //Setup catergories 
            string[] keywords = Constants.componentKeywords;
            for (int i = 0; i < keywords.Length; i++)
                {
                string keyword = keywords[i];

                if (!componentCatergories.ContainsKey (keyword))
                    {
                    componentCatergories.Add (keyword, new List<ComponentType> ());
                    }
                }

            componentCatergories.Add ("General", new List<ComponentType> ());
            componentCatergories.Add ("Custom", new List<ComponentType> ());

            catergoryFoldout = new bool[componentCatergories.Count];

            //Get all types and sort into catergories
            for (int i = 0; i < settings.unityComponents.Count; i++)
                {
                ComponentType component = settings.unityComponents[i];

                bool isContained = false;
                for (int j = 0; j < keywords.Length; j++)
                    {
                    string keyword = keywords[j];
                    Type type = component.type;

                    if (type == null)
                        {
                        continue;
                        }

                    if (type.FullName.Contains (keyword))
                        {
                        componentCatergories[keyword].Add (component);
                        isContained = true;
                        break;
                        }
                    }

                if (!isContained)
                    {
                    componentCatergories["General"].Add (component);
                    }
                }

            // Sort to have larget catergories at the start, just for some form of row equality
            // Not the best, but as it's getting called once, it should be fine
            componentCatergories = componentCatergories.OrderByDescending (c => c.Value.Count).ToDictionary(t => t.Key, t => t.Value);

            //Sort just for nicer viewing
            foreach (List<ComponentType> item in componentCatergories.Values)
                {
                if (item != null)
                    item.Sort ();
                }

            unityComponents = serializedSettings.FindProperty ("unityComponents");
            customComponents = serializedSettings.FindProperty ("customComponents");


            }

        protected override void OnTitleGUI()
            {

            }

        protected override void OnContentGUI()
            {
            for (int i = 0; i < componentCatergories.Count; i++)
                {
                KeyValuePair<string, List<ComponentType>> catergory = componentCatergories.ElementAt (i);
                var components = catergory.Value;

                EditorGUILayout.BeginHorizontal ();
                    { 
                    catergoryFoldout[i] = EditorGUILayout.Foldout (catergoryFoldout[i], catergory.Key, true);

                    GUILayout.FlexibleSpace ();

                    if (GUILayout.Button ("Enable All", EditorStyles.centeredGreyMiniLabel))
                        SetVisibilityForAll (components, true);

                    EditorGUILayout.Space ();

                    if (GUILayout.Button("Disable All", EditorStyles.centeredGreyMiniLabel))
                        SetVisibilityForAll (components, false);
                    }
                EditorGUILayout.EndHorizontal ();

                if (catergoryFoldout[i])
                    {
                
                    bool hasColumn = false;
                    int validIndex = 0;
                    for (int j = 0; j < components.Count; j++)
                        {
                        ComponentType component = components[j];
                        GUIContent content = EditorGUIUtility.ObjectContent (null, component.type);

                        // Ignore irrelevant components, mostly base types or unused components
                        if (content.image == null || content.image.name == "d_DefaultAsset Icon")
                            continue;

                        // Setup group to display two column view of icons
                        hasColumn = validIndex % 2 == 0;

                        GUIHelper.BeginConditionalHorizontal (hasColumn);
                            {
                            content.text = component.name;
                            component.shown = EditorGUILayout.ToggleLeft (content, component.shown);
                            }
                        GUIHelper.EndConditionHorizontal (!hasColumn);

                        validIndex++;
                        }

                    //Got to cancel the horizontal group
                    GUIHelper.EndConditionHorizontal (hasColumn);
                    }
                }

            }

        private void SetVisibilityForAll(List<ComponentType> components, bool visibility)
            {
            Undo.RecordObject (settings, $"Toggled components to {visibility}.");

            foreach (var component in components)
                component.shown = visibility;
            }
        }
    }
