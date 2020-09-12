using UnityEngine;
using UnityEditor;

namespace HierarchyDecorator
    {
    internal class GeneralTab : SettingsTab
        {
        private GlobalSettings global;

        public GeneralTab() : base()
            {
            global = settings.globalStyle;
            }

        public override void OnTitleHeaderGUI()
            {

            }

        public override void OnTitleContentGUI()
            {

            }

        public override void OnBodyHeaderGUI()
            {
            
            }

        public override void OnBodyContentGUI()
            {
            global.OnDraw ();
            }
        }
    }
