using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace HierarchyDecorator
    {
    [InitializeOnLoad]
    internal static class Constants
        {
        //Path
        internal const string SETTINGS_PATH = "Hierarchy Decorator";

        internal const string SETTINGS_ASSET_PATH = "Assets/HierarchyDecorator/Settings.asset";
        internal const string SETTINGS_ASSET_FOLDER = "Assets/HierarchyDecorator/";

        //Draw
        internal readonly static Color SelectionColor = new Color (58f / 255f, 178f / 255f, 178f / 255f, 1);
        internal readonly static Color HoverColor = new Color (150f / 255f, 150f / 255f, 150f / 255f, 1);
        internal readonly static Color UnactiveColor = new Color (0.9f, 0.9f, 0.9f, 0.4f);

        //Collections
        public readonly static List<Type> CommonComponents = new List<Type>
            {
            // -- Animation --
            typeof(Animation),
            typeof(Animator),

            // -- Audio --
            typeof(AudioListener),
            typeof(AudioSource),

            //-- Coll --
            typeof(BoxCollider),
            typeof(SphereCollider),
            typeof(CapsuleCollider),
            typeof(MeshCollider),

            // -- Camera --
            typeof(Camera),

            // -- Lighting --
            typeof(Light),

            // -- Mesh --
            typeof(MeshRenderer),

            // -- Particle --
            typeof(ParticleSystem),
            typeof(Projector),

            // -- Physics --
            typeof(Rigidbody),
                
            // --- Terrain ---
            typeof(Terrain),

            // --- UI ---
            typeof(RectTransform),

            typeof(Image),
            typeof(RawImage),
            };

        internal static HierarchyDecoratorSettings Settings => HierarchyDecoratorSettings.GetOrCreateSettings ();
        internal static List<HierarchyStyle> prefixes => Settings.prefixes;

        //Layer Masks
        internal static string[] LayerMasks => UnityEditorInternal.InternalEditorUtility.layers;
        internal readonly static LayerMask AllLayers = ~0;  

        }

    internal static class Icons
        {
        internal readonly static Texture2D CheckboxEmpty = Resources.Load<Texture2D> ("Icons/checkbox_filled");
        internal readonly static Texture2D CheckboxFilled = Resources.Load<Texture2D> ("Icons/checkbox_empty");

        internal readonly static Texture2D Checked = Resources.Load<Texture2D> ("Icons/checked");
        internal readonly static Texture2D Checkbox = Resources.Load<Texture2D> ("Icons/checkBox");

        internal readonly static Texture2D Checkmark = Resources.Load<Texture2D> ("Icons/checkmark");
        }
    }
