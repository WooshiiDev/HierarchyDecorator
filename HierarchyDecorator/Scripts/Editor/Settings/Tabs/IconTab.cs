using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
    {
    internal class IconTab : SettingsTab
        {
        private static Dictionary<string, List<ComponentType>> componentCategories;

        private static bool[] categoryFoldout;

        // References
        private readonly SerializedProperty customComponents;
        private readonly SerializedProperty unityComponents;

        public IconTab() : base ("Icons", "d_FilterByType")
            {
            componentCategories = new Dictionary<string, List<ComponentType>> ();

            //Setup catergories 
            string[] keywords = Constants.componentKeywords;
            for (int i = 0; i < keywords.Length; i++)
                {
                string keyword = keywords[i];

                if (!componentCategories.ContainsKey (keyword))
                    {
                    componentCategories.Add (keyword, new List<ComponentType> ());
                    }
                }

            componentCategories.Add ("General", new List<ComponentType> ());

            categoryFoldout = new bool[componentCategories.Count];

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
                        componentCategories[keyword].Add (component);
                        isContained = true;
                        break;
                        }
                    }

                if (!isContained)
                    {
                    componentCategories["General"].Add (component);
                    }
                }

            // Sort to have larget catergories at the start, just for some form of row equality
            // Not the best, but as it's getting called once, it should be fine
            componentCategories = componentCategories.OrderByDescending (c => c.Value.Count).ToDictionary(t => t.Key, t => t.Value);

            //Sort just for nicer viewing
            foreach (List<ComponentType> item in componentCategories.Values)
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
            bool canHaveColumns = EditorGUIUtility.currentViewWidth > 500f;
  
            for (int i = 0; i < componentCategories.Count; i++)
                {
                KeyValuePair<string, List<ComponentType>> category = componentCategories.ElementAt (i);
                var components = category.Value;

                EditorGUILayout.BeginVertical (GUI.skin.box);

                DrawCategoryFoldout (ref categoryFoldout[i], category.Key, components);
              

                if (categoryFoldout[i])
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

                        GUIHelper.BeginConditionalHorizontal (hasColumn && canHaveColumns);
                            {
                            content.text = component.name;
                            component.shown = EditorGUILayout.ToggleLeft (content, component.shown);
                            }
                        GUIHelper.EndConditionHorizontal (!hasColumn && canHaveColumns);

                        validIndex++;
                        }

                    //Got to cancel the horizontal group
                    GUIHelper.EndConditionHorizontal (hasColumn && canHaveColumns);
                    }

                EditorGUILayout.EndVertical ();
                }

            DrawCustomComponents ();
            }

        private void DrawCustomComponents()
            {
            EditorGUILayout.BeginVertical (GUI.skin.box);

            DrawCategoryFoldout ("Custom", customComponents);

            if (customComponents.isExpanded)
                {
                SerializedProperty customType = null;
                SerializedProperty script = null;
                Object scriptValue = null;

                for (int i = 0; i < customComponents.arraySize; i++)
                    {
                    customType = customComponents.GetArrayElementAtIndex (i);
                    script = customType.FindPropertyRelative ("script");
                    scriptValue = script.objectReferenceValue;

                    string displayName = scriptValue == null ? "Empty Custom Element" : scriptValue.name;

                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal ();

                    EditorGUILayout.BeginVertical ();

                    //Drwa all children afterwards
                    SerializedProperty shown = customType.FindPropertyRelative ("shown");

                    EditorGUI.BeginChangeCheck ();
                        {
                        EditorGUILayout.BeginHorizontal ();
                        shown.boolValue = EditorGUILayout.Toggle (shown.boolValue, GUILayout.Width(64f));
                        EditorGUILayout.ObjectField (script, new GUIContent());


                        EditorGUILayout.EndHorizontal ();
                        }
                    if (EditorGUI.EndChangeCheck ())
                        {
                        serializedSettings.ApplyModifiedProperties ();
                        settings.customComponents[i].UpdateScriptType ();
                        serializedSettings.UpdateIfRequiredOrScript ();
                        }
                        

                    EditorGUILayout.EndVertical ();

                    if (GUILayout.Button ("Delete Icon", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandHeight(true)))
                        {
                        customComponents.DeleteArrayElementAtIndex (i);
                        serializedSettings.ApplyModifiedProperties ();
                        }

                    EditorGUILayout.EndHorizontal ();

                    EditorGUI.indentLevel--;
                    }

                if (GUILayout.Button("+", Style.listControlStyle))
                    {
                    Undo.RecordObject (settings, "Added new custom component icon");

                    customComponents.InsertArrayElementAtIndex (customComponents.arraySize);
                    serializedSettings.ApplyModifiedProperties ();
                    }
                }

            EditorGUILayout.EndVertical ();
            }

        private void DrawCategoryFoldout(ref bool foldout, string name, List<ComponentType> components)
            {
            EditorGUILayout.BeginHorizontal ();
                {
                foldout = EditorGUILayout.Foldout (foldout, name, true, Style.foldoutHeaderStyle);

                if (GUILayout.Button ("Enable All", EditorStyles.centeredGreyMiniLabel))
                    SetVisibilityForAll (components, true);

                EditorGUILayout.Space ();

                if (GUILayout.Button ("Disable All", EditorStyles.centeredGreyMiniLabel))
                    SetVisibilityForAll (components, false);
                }
            EditorGUILayout.EndHorizontal ();
            }

        private void DrawCategoryFoldout(string name, SerializedProperty componentParent)
            {
            EditorGUILayout.BeginHorizontal ();
                {
                componentParent.isExpanded = EditorGUILayout.Foldout (componentParent.isExpanded, name, true, Style.foldoutHeaderStyle);

                if (GUILayout.Button ("Enable All", EditorStyles.centeredGreyMiniLabel))
                    SetVisibilityForAll (componentParent, true);

                EditorGUILayout.Space ();

                if (GUILayout.Button ("Disable All", EditorStyles.centeredGreyMiniLabel))
                    SetVisibilityForAll (componentParent, false);
                }
            EditorGUILayout.EndHorizontal ();
            }

        private void DrawCustomFoldout(int index, string name, SerializedProperty customProperty)
            {
            EditorGUILayout.BeginHorizontal ();
                {
                GUIHelper.GetSerializedFoldout (customProperty, name);

              
                }
            EditorGUILayout.EndHorizontal ();
            }

        private void SetVisibilityForAll(List<ComponentType> components, bool visibility)
            {
            Undo.RecordObject (settings, $"Toggled components to {visibility}.");

            foreach (var component in components)
                component.shown = visibility;
            }

        private void SetVisibilityForAll(SerializedProperty componentParent, bool visibility)
            {
            Undo.RecordObject (settings, $"Toggled components to {visibility}.");

            for (int i = 0; i < componentParent.arraySize; i++)
                {
                SerializedProperty component = componentParent.GetArrayElementAtIndex (i);
                component.FindPropertyRelative ("shown").boolValue = visibility;
                }

            serializedSettings.ApplyModifiedProperties ();
            }
        }
    }
