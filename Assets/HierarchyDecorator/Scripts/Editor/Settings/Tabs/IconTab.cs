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
        private int catergorySelection;
        private int customSelection;

        private static List<ComponentType> currentComponents;
        private static Dictionary<string, List<ComponentType>> componentCatergories;

        public IconTab() : base()
            {
            componentCatergories = new Dictionary<string, List<ComponentType>> ();

            //Setup catergories 
            string[] keywords = Constants.componentKeywords;
            for (int i = 0; i < keywords.Length; i++)
                {
                string keyword = keywords[i];

                if (!componentCatergories.ContainsKey (keyword))
                    componentCatergories.Add (keyword, new List<ComponentType> ());
                }

            componentCatergories.Add ("General", new List<ComponentType> ());
            componentCatergories.Add ("Custom", null);

            //Get all types and sort into catergories
            for (int i = 0; i < settings.components.Count; i++)
                {
                ComponentType component = settings.components[i];

                bool isContained = false;
                for (int j = 0; j < keywords.Length; j++)
                    {
                    string keyword = keywords[j];
                    Type type = component.type;

                    if (type == null)
                        continue;
                    
                    if (type.FullName.Contains (keyword))
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
            foreach (var item in componentCatergories.Values)
                {
                if (item != null)
                    item.Sort ();
                }
            }

        public override void OnTitleHeaderGUI()
            {

            }

        public override void OnTitleContentGUI()
            {
            EditorGUILayout.Space ();
            GUIHelper.LineSpacer ();
            EditorGUILayout.Space ();

            string[] catergories = componentCatergories.Keys.ToArray ();
            catergorySelection = GUILayout.SelectionGrid (catergorySelection, catergories, 4, EditorStyles.centeredGreyMiniLabel);
            currentComponents = componentCatergories[catergories[catergorySelection]];
            }

        public override void OnBodyHeaderGUI()
            {

            }

        public override void OnBodyContentGUI()
            {
            if (catergorySelection == componentCatergories.Count - 1)
                {
                DrawCustoms ();
                return;
                }

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

        private void DrawCustoms()
            {
            serializedSettings.UpdateIfRequiredOrScript ();

            var customTypes = serializedSettings.FindProperty ("customTypes");

            if (customTypes.arraySize == 0)
                {
                if (GUILayout.Button ("Add", EditorStyles.miniButtonRight))
                    {
                    customTypes.InsertArrayElementAtIndex (0);
                    serializedSettings.ApplyModifiedProperties ();
                    }
                }

            string[] customNames = new string[customTypes.arraySize];

            EditorGUI.indentLevel++;

            SerializedProperty customType = null;
            SerializedProperty script = null;
            Object scriptValue = null;


            for (int i = 0; i < customTypes.arraySize; i++)
                {                        
                customType = customTypes.GetArrayElementAtIndex (i);
                script = customType.FindPropertyRelative ("script");
                scriptValue = script.objectReferenceValue;

                string displayName = scriptValue == null ? "Empty Custom Element" : scriptValue.name;

                //Draw header for the custom type
                EditorGUILayout.BeginHorizontal ();
                    {
                    GUIHelper.GetSerializedFoldout (customType, displayName);

                    if (GUILayout.Button ("Delete", EditorStyles.miniButtonLeft))
                        settings.customTypes.RemoveAt (i);
                    else
                    if (GUILayout.Button ("Add", EditorStyles.miniButtonRight))
                        settings.customTypes.Insert (i + 1, null);
                    }
                EditorGUILayout.EndHorizontal ();

                //Drwa all children afterwards
                if (customType.isExpanded)
                    {
                    EditorGUI.BeginChangeCheck ();
                        {
                        SerializedPropertyUtility.DrawChildrenProperties (customType, false);
                        }
                    if (EditorGUI.EndChangeCheck ())
                        {
                        serializedSettings.ApplyModifiedProperties ();
                        settings.customTypes[i].UpdateScriptType ();
                        }
                    }
                }
            EditorGUI.indentLevel--;  
            }
            
        }
    }
