using UnityEditor;

namespace HierarchyDecorator
{
    public class SpaceDrawer : IDrawable
    {
        private readonly float height;

        public SpaceDrawer(float height)
        {
            this.height = height;
        }

        public void OnDraw()
        {
            HierarchyGUI.Space(height);
        }
    }
}
