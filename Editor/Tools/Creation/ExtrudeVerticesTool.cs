using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class ExtrudeVerticesTool : ToolBase
    {
        public enum Mode
        {
            ExtrudeOne,
            ExtrudeMany
        }

        [SerializeField]
        private Mode m_mode;

        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            if (!wrapper.Consume())
                return;

            switch (m_mode)
            {
                case Mode.ExtrudeOne:
                    {
                        if (vertices.Count() != 2)
                            return;

                        var edges = Model.GetComponentsInChildren<Edge>()
                            .Where(_edge => _edge.Valid && (vertices.Any(v => v == _edge.V1 || v == _edge.V2)))
                            .ToArray();

                        if (!edges.Any())
                            return;

                        var v1 = vertices.ElementAt(0);
                        var v2 = vertices.ElementAt(1);
                        ExtrudeOne(edges, v1, v2);
                    }
                    break;
                case Mode.ExtrudeMany:
                    {
                        if (vertices.Count() < 2)
                            return;

                        var edges = Model.GetComponentsInChildren<Edge>()
                            .Where(_edge => _edge.Valid && (vertices.Contains(_edge.V1) && vertices.Contains(_edge.V2)))
                            .ToArray();

                        if (!edges.Any())
                            return;

                        ExtrudeMany(edges, vertices);
                    }
                    break;
                default:
                    throw new System.NotSupportedException(m_mode.ToString());
            }
        }

        private void ExtrudeOne(IReadOnlyList<Edge> edges, Vertex v1, Vertex v2)
        {
            var edgesRoot = edges.First().transform.parent;

            var newV1 = Spawn<Vertex>(v1.transform.parent);
            newV1.Position = (v1.Position + v2.Position) / 2.0f;
            
            var newEdge1 = Spawn<Edge>(edgesRoot);
            newEdge1.V1 = v1;
            newEdge1.V2 = newV1;
            var newEdge2 = Spawn<Edge>(edgesRoot);
            newEdge2.V1 = newV1;
            newEdge2.V2 = v2;

            Selection.objects = new[] { newV1.gameObject };
        }

        private void ExtrudeMany(IReadOnlyList<Edge> edges, IEnumerable<Vertex> vertices)
        {
            var edgesRoot = edges.First().transform.parent;
            var connectedVertices = edges.BuildConnectionsMap(v => vertices.Contains(v) ? v : null);
            vertices = OrderByConnections(vertices, connectedVertices);

            List<GameObject> selection = new ();

            Vertex previousOld = null;
            Vertex previousNew = null;
            foreach(var v in vertices)
            {
                var newV = Spawn<Vertex>(v.transform.parent);
                newV.Position = v.Position;
                selection.Add(newV.gameObject);

                if (previousOld)
                {
                    var newEdge1 = Spawn<Edge>(edgesRoot);
                    newEdge1.V1 = v;
                    newEdge1.V2 = newV;
                    var newEdge2 = Spawn<Edge>(edgesRoot);
                    newEdge2.V1 = previousOld;
                    newEdge2.V2 = newV;
                    var newEdge3 = Spawn<Edge>(edgesRoot);
                    newEdge3.V1 = previousNew;
                    newEdge3.V2 = newV;
                }
                else
                {
                    var newEdge = Spawn<Edge>(edgesRoot);
                    newEdge.V1 = v;
                    newEdge.V2 = newV;
                }

                previousOld = v;
                previousNew = newV;
            }

            Selection.objects = selection.ToArray();
        }

        private IEnumerable<Vertex> OrderByConnections(IEnumerable<Vertex> vertices, Dictionary<Vertex, List<Vertex>> connectedVertices)
        {
            var visited = new HashSet<Vertex>();
            var start = FindStart();

            for(var v = start; v != null; v = FindNext(v))
            {
                visited.Add(v);
                yield return v;
            }
        
            Vertex FindNext(Vertex v)
            {
                if (!connectedVertices.TryGetValue(v, out var connected))
                    return null;

                var available = connected.Where(x => !visited.Contains(x)).ToArray();
                if (!available.Any())
                    return null;

                return available.Aggregate((currentMin, x) => connectedVertices[x].Count < connectedVertices[currentMin].Count ? x : currentMin);
            }

            Vertex FindStart()
            {
                var result = connectedVertices.Aggregate((currentMin, x) => x.Value.Count < currentMin.Value.Count ? x : currentMin);
                return result.Key;
            }
        }

        
    }
}
