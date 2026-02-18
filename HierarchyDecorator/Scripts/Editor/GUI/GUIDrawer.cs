using UnityEditor;

namespace HierarchyDecorator
{
    /// <summary>
    /// A class for drawing GUI elements.
    /// </summary>
    public abstract class GUIDrawer : IDrawable
    {
        protected System.Func<bool> optionEnableIf;
        protected System.Func<bool> optionShowIf;
        protected System.Action<SerializedProperty> optionOnChanged;
        public void SetOptionEnableIf(System.Func<bool> condition) => optionEnableIf = condition;
        public void SetOptionShowIf(System.Func<bool> condition) => optionShowIf = condition;
        public void SetOptionOnchanged(System.Action<SerializedProperty> callback) => optionOnChanged = callback;

        /// <summary>
        /// Draw this element.
        /// </summary>
        public void OnDraw()
        {
            if (optionShowIf?.Invoke() == false)
                return;
            EditorGUI.BeginDisabledGroup (!IsEnabled ());

            OnGUI ();

            EditorGUI.EndDisabledGroup ();
        }

        /// <summary>
        /// Is this element currently enabled? 
        /// Called in <see cref="OnDraw"/>. Will disable the element otherwise.
        /// </summary>
        /// <returns>Returns a bool stating if this element is enabled or not.</returns>
        public virtual bool IsEnabled()
        {
            return optionEnableIf?.Invoke() ?? true;
        }

        /// <summary>
        /// Draw method used to setup how the element is displayed.
        /// </summary>
        protected abstract void OnGUI();

        protected abstract float GetHeight();
    }

    /// <summary>
    /// A class for drawing GUI elements.
    /// </summary>
    /// <typeparam name="T">The target data type for this drawer to target.</typeparam>
    public abstract class GUIDrawer<T> : GUIDrawer
    {
        /// <summary>
        /// The target data used for the GUI.
        /// </summary>
        public T Target
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new DrawableElement.
        /// </summary>
        /// <param name="target">The target data object.</param>
        public GUIDrawer(T target)
        {
            Target = target;
        }
    }
}
