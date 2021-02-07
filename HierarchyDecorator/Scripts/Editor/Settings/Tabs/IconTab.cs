using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
    {
    internal class IconTab : SettingsTab
        {
        [System.Serializable]
        private class IconInfo : IComparable<IconInfo>
            {
            public readonly Type type;
            public SerializedProperty property;

            public IconInfo(Type type, SerializedProperty property)
                {
                this.type = type;
                this.property = property;
                }

            public Int32 CompareTo(IconInfo other)
                {
                return type.FullName.CompareTo (other.type.FullName);
                }
            }

        private static Dictionary<string, List<IconInfo>> componentCategories = new Dictionary<string, List<IconInfo>> ();

        private static bool[] categoryFoldout;            

        // References
        private readonly SerializedProperty customComponents;
        private readonly SerializedProperty unityComponents;

        public IconTab() : base ("Icons", "d_FilterByType")
            {
            // Setup References
            unityComponents = serializedSettings.FindProperty ("unityComponents");
            customComponents = serializedSettings.FindProperty ("customComponents");

            //Setup catergories 
            string[] keywords = Constants.componentKeywords;

            for (int i = 0; i < keywords.Length; i++)
                componentCategories.Add (keywords[i], new List<IconInfo> ());

            //Add other categories
            componentCategories.Add ("General", new List<IconInfo> ());

            //Get all types and sort into catergories
            for (int i = 0; i < settings.unityComponents.Count; i++)
                {
                ComponentType component = settings.unityComponents[i];
                Type type = component.type;

                IconInfo info = new IconInfo (type, unityComponents.GetArrayElementAtIndex (i).FindPropertyRelative("shown"));
                Texture componentImage = EditorGUIUtility.ObjectContent (null, type).image;

                if (componentImage == null || componentImage.name == "d_DefaultAsset Icon")
                    continue;

                string componentKeyword = keywords.FirstOrDefault (f => type.FullName.Contains (f));

                if (string.IsNullOrEmpty (componentKeyword))
                    componentKeyword = "General";

                componentCategories[componentKeyword].Add (info);
                }

            // Sort to have larget catergories at the start
            componentCategories = componentCategories.OrderByDescending (c => c.Value.Count).ToDictionary (t => t.Key, t => t.Value);

            foreach (List<IconInfo> item in componentCategories.Values)
                item.Sort ();

            // Bools for foldouts
            categoryFoldout = new bool[componentCategories.Count];
            }



        protected override void OnTitleGUI()
            {

            }

        protected override void OnContentGUI()
            {
            bool canHaveColumns = EditorGUIUtility.currentViewWidth > 500f;

            for (int i = 0; i < componentCategories.Count; i++)
                {
                var category = componentCategories.ElementAt (i);
                var icons = category.Value;

                EditorGUILayout.BeginVertical (GUI.skin.box);
                    {
                    if (categoryFoldout[i] = DrawCategoryFoldout (categoryFoldout[i], category.Key, category.Value))
                        {
                        bool hasColumn = false;
                        int validIndex = 0;

                        for (int j = 0; j < icons.Count; j++)
                            {
                            IconInfo icon = icons[j];

                            GUIContent content = EditorGUIUtility.ObjectContent (null, icon.type);
                            content.text = icon.type.Name;

                            // Setup group to display two column view of icons
                            hasColumn = validIndex % 2 == 0;

                            GUIHelper.BeginConditionalHorizontal (hasColumn && canHaveColumns);
                                {
                                icon.property.boolValue = EditorGUILayout.ToggleLeft (content, icon.property.boolValue);
                                }
                            GUIHelper.EndConditionHorizontal (!hasColumn && canHaveColumns);

                            validIndex++;
                            }

                        GUIHelper.EndConditionHorizontal (hasColumn && canHaveColumns);
                        }
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
                        {
                        EditorGUILayout.BeginVertical ();
                            {
                            //Draw all children afterwards
                            SerializedProperty shown = customType.FindPropertyRelative ("shown");

                            EditorGUI.BeginChangeCheck ();
                                {
                                EditorGUILayout.BeginHorizontal ();

                                shown.boolValue = EditorGUILayout.Toggle (shown.boolValue, GUILayout.Width (64f));
                                EditorGUILayout.ObjectField (script, new GUIContent ());

                                EditorGUILayout.EndHorizontal ();
                                }
                            if (EditorGUI.EndChangeCheck ())
                                {
                                serializedSettings.ApplyModifiedProperties ();
                                settings.customComponents[i].UpdateScriptType ();
                                serializedSettings.UpdateIfRequiredOrScript ();
                                }
                            }
                        EditorGUILayout.EndVertical ();

                        if (GUILayout.Button ("Delete Icon", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandHeight (true)))
                            {
                            customComponents.DeleteArrayElementAtIndex (i);
                            serializedSettings.ApplyModifiedProperties ();
                            }
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

        private bool DrawCategoryFoldout(bool foldout, string name, List<IconInfo> components)
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

            return foldout;
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
                GUIHelper.GetSerializedFoldout (customProperty, name);
            EditorGUILayout.EndHorizontal ();
            }

        private void SetVisibilityForAll(List<IconInfo> components, bool visibility)
            {
            Undo.RecordObject (settings, $"Toggled components to {visibility}.");

            foreach (var component in components)
                component.property.boolValue = visibility;

            serializedSettings.ApplyModifiedProperties ();
            }

        private void SetVisibilityForAll(SerializedProperty componentParent, bool visibility)
            {
            Undo.RecordObject (settings, $"Toggled components to {visibility}.");

            for (int i = 0; i < componentParent.arraySize; i++)
                {
                SerializedProperty component = componentParent.GetArrayElementAtIndex (i);
                component.FindPropertyRelative ("shown").boolValue = visibility;
                }
            }
        }
    }
