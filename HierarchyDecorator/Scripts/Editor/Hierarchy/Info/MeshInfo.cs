using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    public class MeshInfo : HierarchyInfo
    {
        private const int LABEL_GRID_SIZE = 2;

        // --- Settings

        private bool triangleCountEnabled;

        // --- Cached Data

        private int triangleCount;

        // --- Methods

        protected override void OnDrawInit(HierarchyItem item, Settings settings)
        {
            triangleCount = GetTriangleCount(item.GameObject);
        }

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            triangleCountEnabled = settings.globalData.showTriangleCounts;

            if (settings.styleData.HasStyle(item.DisplayName))
            {
                triangleCountEnabled &= settings.styleData.displayTriangleCounts;
            }

            return triangleCountEnabled;
        }

        protected override void DrawInfo(Rect rect, HierarchyItem item, Settings settings)
        {
            DrawTriangleCount(rect);
        }

        protected override int CalculateGridCount()
        {
            return triangleCount > 0 ? LABEL_GRID_SIZE : 0;
        }

        protected override bool ValidateGrid()
        {
            if (GridCount < LABEL_GRID_SIZE)
            {
                return false;
            }

            return true;
        }

        // - Drawing elements

        private void DrawTriangleCount(Rect rect)
        {
            rect.x = rect.xMax - LABEL_GRID_SIZE * INDENT_SIZE;
            rect.width = LABEL_GRID_SIZE * INDENT_SIZE;

            GUIStyle SmallDropdownRightAligned = new GUIStyle(Style.SmallDropdown)
            {
                alignment = TextAnchor.MiddleRight
            };

            EditorGUI.LabelField(rect, triangleCount.ToString(), SmallDropdownRightAligned);
        }

        // - Helpers

        private int GetTriangleCount(GameObject obj)
        {
            int total = 0;

            MeshFilter mf = obj.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                total += mf.sharedMesh.triangles.Length / 3;
            }

            SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
            if (smr != null && smr.sharedMesh != null)
            {
                total += smr.sharedMesh.triangles.Length / 3;
            }

            return total;
        }
    }
}
