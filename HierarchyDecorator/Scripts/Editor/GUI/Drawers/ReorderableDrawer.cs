using UnityEditorInternal;

namespace HierarchyDecorator
{
    public class ReorderableDrawer : GUIDrawer<ReorderableList>
    {
        public ReorderableDrawer(ReorderableList target) : base (target) { }

        protected override void OnElementDraw()
        {
            Target.DoLayoutList ();
        }
    }
}