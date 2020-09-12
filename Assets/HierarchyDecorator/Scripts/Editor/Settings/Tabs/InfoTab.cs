using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal class InfoTab : SettingsTab
        {
        private static Texture banner = Textures.Banner;

        public override void OnTitleHeaderGUI()
            {

            }

        public override void OnTitleContentGUI()
            {

            }

        public override void OnBodyHeaderGUI()
            {
            if (GUILayout.Button ("GitHub Repository", EditorStyles.miniButtonMid))
                Application.OpenURL ("https://github.com/WooshiiDev/HierarchyDecorator/");

            if (GUILayout.Button ("Twitter", EditorStyles.miniButtonMid))
                Application.OpenURL ("https://twitter.com/DaamiaanS");
            }

        public override void OnBodyContentGUI()
            {
            GUILayout.Box (banner, new GUIStyle());
            }
        }
    }
