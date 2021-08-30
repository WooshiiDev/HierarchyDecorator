using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    //Seeing this makes me realize I should have made a smaller name for this.
    [CustomEditor (typeof (Settings))]
    internal class SettingsEditor : Editor
    {
        private Settings t;

        private List<SettingsTab> tabs;

        private void OnEnable()
        {
            t = target as Settings;

            tabs = new List<SettingsTab> ();

            RegisterTab (new GeneralTab (t, serializedObject));
            RegisterTab (new StyleTab (t, serializedObject));
            RegisterTab (new IconTab (t, serializedObject));
        }

        private void OnDisable()
        {
            if (serializedObject != null)
            {
                serializedObject.Dispose ();

            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update ();

            DrawTitle ();

#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.Space ();
#else
            GUILayout.Space(16f);
#endif
            foreach (SettingsTab tab in tabs)
            {
                EditorGUI.indentLevel++;
                tab.OnGUI ();
                EditorGUI.indentLevel--;
            }

            if (serializedObject.UpdateIfRequiredOrScript())
            {
                EditorApplication.RepaintHierarchyWindow ();
            }
        }

        private void RegisterTab(SettingsTab tab)
        {
            tabs.Add (tab);
        }

        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal ();
            {
                EditorGUILayout.LabelField ("Hierarchy Settings", Style.Title);

                GUILayout.FlexibleSpace ();

                if (GUILayout.Button ("GitHub Repository", EditorStyles.miniButtonMid))
                {
                    Application.OpenURL ("https://github.com/WooshiiDev/HierarchyDecorator/");
                }

                EditorGUILayout.Space ();

                if (GUILayout.Button ("Twitter", EditorStyles.miniButtonMid))
                {
                    Application.OpenURL ("https://twitter.com/WooshiiDev");
                }

                EditorGUILayout.Space ();
            }
            EditorGUILayout.EndHorizontal ();
        }
    }
}