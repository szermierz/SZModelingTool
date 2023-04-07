using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class GenerateFacesFromEdges : EffectBase
    {
        private struct FaceKey
        {
            public int V1;
            public int V2;
            public int V3;

            public void Sort()
            {
                var v1 = V1;
                var v2 = V2;
                var v3 = V3;

                if(V1 <= V2 && V1 <= V3)
                {
                    if(V2 <= V3)
                    {
                        V1 = v1;
                        V2 = v2;
                        V3 = v3;
                    }
                    else
                    {
                        V1 = v1;
                        V2 = v3;
                        V3 = v2;
                    }
                }
                else if(V2 <= V1 && V2 <= V3)
                {
                    if (V1 <= V3)
                    {
                        V1 = v2;
                        V2 = v1;
                        V3 = v3;
                    }
                    else
                    {
                        V1 = v2;
                        V2 = v3;
                        V3 = v1;
                    }
                }
                else if(V3 <= V1 && V3 <= V2)
                {
                    if (V1 <= V2)
                    {
                        V1 = v3;
                        V2 = v1;
                        V3 = v2;
                    }
                    else
                    {
                        V1 = v3;
                        V2 = v2;
                        V3 = v1;
                    }
                }
                else
                {
                    NotSupported(this);
                }

                void NotSupported(FaceKey f)
                {
                    throw new Exception($"{f.V1},{f.V2},{f.V3}");
                }
            }

            public static FaceKey FromFace(IDictionary<Vertex, int> vertices, Face f)
            {
                return FromVertices(vertices, f.Vertices);
            }

            public static FaceKey FromVertices(IDictionary<Vertex, int> vertices, Vertex[] v)
            {
                return FromVertices(vertices, v[0], v[1], v[2]);
            }

            public static FaceKey FromVertices(IDictionary<Vertex, int> vertices, Vertex v1, Vertex v2, Vertex v3)
            {
                var key = new FaceKey()
                {
                    V1 = vertices[v1],
                    V2 = vertices[v2],
                    V3 = vertices[v3],
                };
                key.Sort();
                return key;
            }
        }

        [SerializeField]
        private Transform m_edgesRoot;

        protected override void EffectImplementation()
        {
            EnsureCloneModel(cloneVertices: true, cloneFaces: false);
            var faces = FindFaces();
            var newFaces = GenerateInitialFaces(faces);

            var vertices = IndexVertices();
            var edges = GatherEdges();
            var connectedVertices = edges.BuildConnectionsMap(v => OldByNewVertices[v]);

            var facesToProcess = new List<Face>(newFaces);
            var hashedFaces = new HashSet<FaceKey>();
            foreach(var newFace in newFaces)
            {
                var key = FaceKey.FromFace(vertices, newFace);
                hashedFaces.Add(key);
            }

            ProcessFaces(vertices, connectedVertices, facesToProcess, hashedFaces);
        }

        private List<Edge> GatherEdges()
        {
            var root = m_edgesRoot 
                ? m_edgesRoot 
                : transform;

            return root.GetComponentsInChildren<Edge>()
                            .Where(edge => edge.Valid)
                            .ToList();
        }

        private Dictionary<Vertex, int> IndexVertices()
        {
            return NewVertices
                .Select((vertex, i) => new KeyValuePair<Vertex, int>(vertex, i))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void ProcessFaces(Dictionary<Vertex, int> vertices, Dictionary<Vertex, List<Vertex>> connectedVertices, List<Face> facesToProcess, HashSet<FaceKey> hashedFaces)
        {
            while (facesToProcess.Any())
            {
                var buffer = facesToProcess.ToArray();
                foreach (var face in buffer)
                {
                    ProcessEdge(vertices, connectedVertices, facesToProcess, hashedFaces, face.Vertices[1], face.Vertices[0]);
                    ProcessEdge(vertices, connectedVertices, facesToProcess, hashedFaces, face.Vertices[2], face.Vertices[1]);
                    ProcessEdge(vertices, connectedVertices, facesToProcess, hashedFaces, face.Vertices[0], face.Vertices[2]);

                    facesToProcess.Remove(face);
                }
            }
        }

        private void ProcessEdge(Dictionary<Vertex, int> vertices, Dictionary<Vertex, List<Vertex>> connectedVertices, List<Face> facesToProcess, HashSet<FaceKey> hashedFaces, Vertex v1, Vertex v2)
        {
            if (!connectedVertices.ContainsKey(v1) || !connectedVertices.ContainsKey(v2))
                return;
            
            var v1Cons = connectedVertices[v1];
            var v2Cons = connectedVertices[v2];

            foreach (var v3 in v1Cons.Intersect(v2Cons))
            {
                var fKey = FaceKey.FromVertices(vertices, v1, v2, v3);
                if (hashedFaces.Contains(fKey))
                    continue;

                hashedFaces.Add(fKey);

                var f = GenerateFace(v1, v2, v3);
                facesToProcess.Add(f);
            }
        }

        private IEnumerable<Face> GenerateInitialFaces(IEnumerable<Face> faces)
        {
            var result = new List<Face>();
            foreach(var face in faces)
            {
                var newFace = GenerateFace
                (
                    OldByNewVertices[face.Vertices[0]],
                    OldByNewVertices[face.Vertices[1]],
                    OldByNewVertices[face.Vertices[2]]
                );

                result.Add(newFace);
            }

            return result;
        }

        private Face GenerateFace(Vertex v1, Vertex v2, Vertex v3)
        {
            var newFace = new GameObject("F", typeof(Face)).GetComponent<Face>();
            newFace.transform.SetParent(EffectsRoot.DestFacesRoot.transform);
            newFace.Vertices = new[] {v1, v2, v3};
            return newFace;
        }

        private IEnumerable<Face> FindFaces()
        {
            IEnumerable<Face> faces = EffectsRoot.SourceRoot.GetComponentsInChildren<Face>(includeInactive: false);
            if(!faces.Any())
                faces = EffectsRoot.SourceRoot.GetComponentsInChildren<Face>(includeInactive: true);

            if (!faces.Any())
                throw new Exception("Failed to find a single face");

            faces = faces.Where(face => face.IsValid);

            return faces;
        }
    }
}