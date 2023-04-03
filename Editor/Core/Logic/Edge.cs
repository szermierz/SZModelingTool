using UnityEditor;

namespace SZ.ModelingTool
{
    public class Edge : ModelingToolBehaviour
    {
        public Model Model => GetComponentInParent<Model>();

        public Vertex V1;
        public Vertex V2;

        public bool Valid => V1 && V2 && V1 != V2;

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawEditorGizmos(Edge edge, GizmoType gizmoType)
        {
            edge.Model.DrawModelGizmos(edge);
        }
    }
}