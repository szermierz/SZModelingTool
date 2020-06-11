using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class Vertex : ModelingToolBehaviour
    {
        public Model Model => GetComponentInParent<Model>();

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawEditorGizmos(Vertex vertex, GizmoType gizmoType)
        {
            vertex.Model.DrawModelGizmos(vertex);
        }
    }
}