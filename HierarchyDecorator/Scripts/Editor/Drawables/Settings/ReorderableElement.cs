using UnityEditorInternal;

namespace HierarchyDecorator
{
    public class ReorderableElement : DrawableElement<ReorderableList>
    {
        public ReorderableElement(ReorderableList target) : base (target) { }

        protected override void OnElementDraw()
        {
            Target.DoLayoutList ();
        }
    }

}
