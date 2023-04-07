using System;
using System.Collections.Generic;
using UnityEditor;

namespace SZ.ModelingTool
{
    public static class EdgeExtensions
    {
        public static Dictionary<Vertex, List<Vertex>> BuildConnectionsMap(this IEnumerable<Edge> edges, Func<Vertex, Vertex> transform = null)
        {
            var connectedVertices = new Dictionary<Vertex, List<Vertex>>();
            foreach (var edge in edges)
            {
                var v1 = transform?.Invoke(edge.V1) ?? edge.V1;
                var v2 = transform?.Invoke(edge.V2) ?? edge.V2;
                if (!v1 || !v2)
                    continue;

                AddVertex(v1, v2);
                AddVertex(v2, v1);
            }

            return connectedVertices;

            void AddVertex(Vertex from, Vertex to)
            {
                if (!connectedVertices.ContainsKey(from))
                    connectedVertices.Add(from, new List<Vertex>());

                var list = connectedVertices[from];
                list.Add(to);
            }
        }
    }
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