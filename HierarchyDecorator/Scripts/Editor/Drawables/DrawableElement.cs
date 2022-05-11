using UnityEditor;

namespace HierarchyDecorator
{
    /// <summary>
    /// An abstract class for drawing simple GUI elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DrawableElement<T> : IDrawable<T>
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
        public DrawableElement(T target)
        {
            Target = target;
        }

        /// <summary>
        /// Draw this element.
        /// </summary>
        public void OnDraw()
        {
            EditorGUI.BeginDisabledGroup (!IsEnabled ());
            
            OnElementDraw ();

            EditorGUI.EndDisabledGroup ();
        }

        /// <summary>
        /// Is this element currently enabled? 
        /// Called in <see cref="OnDraw"/>. Will disable the element otherwise.
        /// </summary>
        /// <returns>Returns a bool stating if this element is enabled or not.</returns>
        public virtual bool IsEnabled()
        {
            return true;
        }

        /// <summary>
        /// Draw method used to setup how the element is displayed.
        /// </summary>
        protected abstract void OnElementDraw();
    }
}
