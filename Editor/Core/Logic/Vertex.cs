using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    [ExecuteInEditMode]
    public class Vertex : ModelingToolBehaviour
    {
        public Model Model => GetComponentInParent<Model>();

        private void OnDestroy()
        {
            var faces = Model.GetComponentsInChildren<Face>();
            foreach(var face in faces)
            {
                if (face.Vertices.Contains(this))
                    DestroyImmediate(face.gameObject);
            }

            var edges = Model.GetComponentsInChildren<Edge>();
            foreach(var edge in edges)
            {
                if (edge.V1 == this || edge.V2 == this)
                    DestroyImmediate(edge.gameObject);
            }
        }

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