using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal class InfoTab : SettingsTab
        {
        public override void OnTitleHeaderGUI()
            {

            }

        public override void OnTitleContentGUI()
            {

            }

        public override void OnBodyHeaderGUI()
            {
            GUIStyle style = new GUIStyle (EditorStyles.inspectorDefaultMargins)
                {
                padding = new RectOffset (4, 4, 0, 0)
                };

            EditorGUILayout.BeginVertical (style);
                {
                if (GUILayout.Button ("GitHub Repository", EditorStyles.miniButtonMid))
                    Application.OpenURL ("https://github.com/WooshiiDev/HierarchyDecorator/");

                if (GUILayout.Button ("Twitter", EditorStyles.miniButtonMid))
                    Application.OpenURL ("https://twitter.com/DaamiaanS");

                EditorGUILayout.Space ();

                GUILayout.Box (Textures.Banner, new GUIStyle ());
                }
            EditorGUILayout.EndVertical ();
            }

        public override void OnBodyContentGUI()
            {
            }
        }
    }
