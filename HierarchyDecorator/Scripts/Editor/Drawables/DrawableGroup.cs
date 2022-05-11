using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace HierarchyDecorator
{
    /// <summary>
    /// Collection of drawers drawn with a title.
    /// </summary>
    public class DrawableGroup
    {
        private readonly string title;
        private List<IDrawable> drawers = new List<IDrawable> ();

        /// <summary>
        /// Create a new DrawableGroup with a given title.
        /// </summary>
        /// <param name="title">The title of the group.</param>
        public DrawableGroup(string title)
        {
            this.title = title;
        }

        /// <summary>
        /// Draw settings for this group.
        /// </summary>
        /// <param name="property"></param>
        public virtual void OnGUI()
        {
            EditorGUILayout.LabelField (title, Style.SettingsTabHeader);

            for (int i = 0; i < drawers.Count; i++)
            {
                drawers[i].OnDraw ();
            }
        }

        /// <summary>
        /// Register a drawer.
        /// </summary>
        /// <param name="drawable">The drawer to register.</param>
        public DrawableGroup RegisterDrawable(IDrawable drawable)
        {
            // Make sure it's not null

            if (drawable == null)
            {
                Debug.LogWarning ("Cannot add setting drawer of null");
                return this;
            }

            drawers.Add (drawable);

            return this;
        }

        /// <summary>
        /// Register a <see cref="ReorderableList"/> drawer.
        /// </summary>
        /// <param name="list">The ReorderableList to register.</param>
        public DrawableGroup RegisterReorderable(ReorderableList list)
        {
            if (list == null)
            {
                Debug.LogError ("Passing null ReorderableList. Cannot create setting drawers.");
                return this;
            }

            drawers.Add (new ReorderableElement (list));
            return this;
        }

        /// <summary>
        /// Register setting drawers from a <see cref="SerializedProperty"/> with given child property names.
        /// </summary>
        /// <param name="property">The SerializedProperty.</param>
        /// <param name="names">The names of the child properties to find.</param>
        public DrawableGroup RegisterSerializedPropertu(SerializedProperty property, params string[] names)
        {
            // Check if names is null

            if (names == null)
            {
                Debug.LogError ("Passing null property names. Cannot create setting drawers.");
                return this;
            }

            // Loop each name and add a SerializedSettingDrawer if a property is found.

            for (int i = 0; i < names.Length; i++)
            {
                SerializedProperty prop = property.FindPropertyRelative (names[i]);

                // Call a warning in case of changes or incorrect names given

                if (prop == null)
                {
                    Debug.LogWarning ($"Cannot find child property {names[i]} in property {property.displayName}");
                    continue;
                }

                drawers.Add (new SerializedPropertyElement (prop));
            }

            return this;
        }
    }
}
