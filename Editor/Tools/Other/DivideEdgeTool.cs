using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class DivideEdgeTool : ToolBase
    {
        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> selectedVertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, selectedVertices, sceneView, mousePos);

            if (!wrapper.Consume())
                return;

            if (2 != selectedVertices.Count())
                return;

            var v1 = selectedVertices.ElementAt(0);
            var v2 = selectedVertices.ElementAt(1);

            var edges = Model.GetComponentsInChildren<Edge>();
            var existing = edges.FirstOrDefault(_edge => Matching(_edge));
            if (!existing)
                return;

            var verticesMap = GetEdgesByVertices(edges);

            var v12 = new GameObject(nameof(Vertex), typeof(Vertex)).GetComponent<Vertex>();
            v12.transform.SetParent(v1.transform.parent);
            v12.transform.position = GetCenter(existing);

            Selection.objects = new Object[] { v12.gameObject };

            if (verticesMap.ContainsKey(v1) && verticesMap.ContainsKey(v2))
            {
                var edges1 = verticesMap[v1];
                var edges2 = verticesMap[v2];

                foreach (var edge1 in edges1)
                {
                    if (edge1 == existing)
                        continue;

                    var outherVertex = edge1.V1 == v1
                        ? edge1.V2
                        : edge1.V1;

                    foreach (var edge2 in edges2)
                    {
                        if (edge2 == existing)
                            continue;

                        if (edge2.V1 != outherVertex && edge2.V2 != outherVertex)
                            continue;

                        var e = new GameObject(nameof(Edge), typeof(Edge)).GetComponent<Edge>();
                        e.transform.SetParent(existing.transform.parent);
                        e.V1 = v12;
                        e.V2 = outherVertex;
                    }
                }
            }

            var newEdge = new GameObject(nameof(Edge), typeof(Edge)).GetComponent<Edge>();
            newEdge.transform.SetParent(existing.transform.parent);
            newEdge.V1 = v1;
            newEdge.V2 = v12;

            existing.V1 = v12;
            existing.V2 = v2;

            Vector3 GetCenter(Edge edge)
            {
                return (edge.V1.Position + edge.V2.Position) / 2.0f;
            }
            bool Matching(Edge edge)
            {
                var matching = false;
                matching |= edge.V1 == v1 && edge.V2 == v2;
                matching |= edge.V1 == v2 && edge.V2 == v1;
                return matching;
            }
        }

        private Dictionary<Vertex, List<Edge>> GetEdgesByVertices(IEnumerable<Edge> edges)
        {
            var result = new Dictionary<Vertex, List<Edge>>();
            foreach(var edge in edges)
            {
                ProcessVertex(edge.V1, edge);
                ProcessVertex(edge.V2, edge);
            }

            return result;

            void ProcessVertex(Vertex from, Edge edge)
            {
                if(!result.ContainsKey(from))
                    result.Add(from, new List<Edge>());

                result[from].Add(edge);
            }
        }
    }
}