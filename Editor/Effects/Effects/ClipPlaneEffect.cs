using System;
using System.Collections.Generic;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class ClipPlaneEffect : EffectBase
    {
        [SerializeField]
        private BorderLine2D[] m_borders;

        // ToDo refactor this and put to SZUtilities
        [Serializable]
        private class BorderLine2D
        {
            private const float c_almostZero = 0.000001f;

            [SerializeField]
            private bool m_enabled = true;
            public bool Enabled => m_enabled;

            [SerializeField]
            private float m_a;

            [SerializeField]
            private float m_b;

            [SerializeField]
            private float m_c;

            [SerializeField]
            private bool m_borderDirection;
            public bool BorderDirection => m_borderDirection;

            [SerializeField]
            private Color m_gizmoLineColor = Color.cyan;

            [SerializeField]
            private Vector2 m_gizmoLineRange;

            public bool IsInFront(Vector2 point)
            {
                if (Mathf.Abs(m_b) <= c_almostZero && Mathf.Abs(m_a) > c_almostZero)
                {
                    var lineX = -m_c / m_a;
                    return lineX <= point.x;
                }
                else if(Mathf.Abs(m_b) > c_almostZero)
                {
                    var lineY = (m_a * point.x + m_c) / (-m_b);
                    return lineY >= point.y;
                }
                else
                {
                    return false;
                }
            }

            public Vector2 GetIntersection(Vector2 from, Vector2 to)
            {
                BuildLine(from, to, out float a, out float b, out float c);

                if (Mathf.Abs(m_b) < c_almostZero)
                {
                    if (Mathf.Abs(b) < c_almostZero)
                    {
                        var x1 = -m_c / m_a;
                        var x2 = -c / a;
                        return Mathf.Abs(x1 - x2) < c_almostZero ? new Vector2(x1, 0.0f) : default;
                    }
                    else
                    {
                        var x = -m_c / m_a;
                        var y = (a * x + c) / (-b);
                        return new Vector2(x, y);
                    }
                }
                else
                {
                    var x = (m_b * c - b * m_c) / (b * m_a - m_b * a);
                    var y = (m_a * x + m_c) / (-m_b);
                    return new Vector2(x, y);
                }
            }

            public static void BuildLine(Vector2 from, Vector2 to, out float a, out float b, out float c)
            {
                var deltaX = to.x - from.x;
                var deltaY = to.y - from.y;

                if (Mathf.Abs(deltaX) < c_almostZero && Mathf.Abs(deltaY) < c_almostZero)
                {
                    a = 0.0f;
                    b = 0.0f;
                    c = 0.0f;
                }
                else if(Mathf.Abs(deltaX) < c_almostZero)
                {
                    a = 1.0f;
                    b = 0.0f;
                    c = -from.x;
                }
                else if(Mathf.Abs(deltaY) < c_almostZero)
                {
                    a = 0.0f;
                    b = 1.0f;
                    c = -from.y;
                }
                else
                {
                    a = 1.0f;
                    b = (from.x - to.x) / (to.y - from.y);
                    c = -a * from.x - b * from.y;
                }
            }

            public void DrawLineGizmo()
            {
                if (!m_enabled)
                    return;

                if (Mathf.Abs(m_b) <= c_almostZero && Mathf.Abs(m_a) > c_almostZero)
                {
                    var lineX = -m_c / m_a;
                    var from = new Vector3(lineX, 0.0f, m_gizmoLineRange.x);
                    var to = new Vector3(lineX, 0.0f, m_gizmoLineRange.y);
                    Gizmos.color = m_gizmoLineColor;
                    Gizmos.DrawLine(from, to);
                }
                else if (Mathf.Abs(m_b) > c_almostZero && Mathf.Abs(m_a) <= c_almostZero)
                {
                    var lineY = -m_c / m_b;
                    var from = new Vector3(m_gizmoLineRange.x, 0.0f, lineY);
                    var to = new Vector3(m_gizmoLineRange.y, 0.0f, lineY);
                    Gizmos.color = m_gizmoLineColor;
                    Gizmos.DrawLine(from, to);
                }
                else if(Mathf.Abs(m_b) > c_almostZero && Mathf.Abs(m_a) > c_almostZero)
                {
                    var x1 = m_gizmoLineRange.x;
                    var x2 = m_gizmoLineRange.y;
                    var y1 = (m_a * x1 + m_c) / (-m_b);
                    var y2 = (m_a * x2 + m_c) / (-m_b);
                    var from = new Vector3(x1, 0.0f, y1);
                    var to = new Vector3(x2, 0.0f, y2);
                    Gizmos.color = m_gizmoLineColor;
                    Gizmos.DrawLine(from, to);
                }
            }
        }

        private static IList<Vertex> s_inFrontVertices = new List<Vertex>(2);
        private static IList<Vertex> s_outFrontVertices =new List<Vertex>(2);
        
        protected override void EffectImplementation()
        {
            EnsureCloneModel(true, true);

            var toDiscard = new List<Face>();

            // For each line, for each triangle, try clip triangle
            foreach (var line2D in m_borders)
            {
                if (!line2D.Enabled)
                    continue;

                var faces = EffectsRoot.DestFacesRoot.GetComponentsInChildren<Face>();


                foreach (var face in faces)
                {
                    // Discard invalid faces
                    if (face.Vertices.Length != 3)
                    {
                        toDiscard.Add(face);
                        continue;
                    }

                    var is0 = line2D.BorderDirection == line2D.IsInFront(GetPosition(face.Vertices[0]));
                    var is1 = line2D.BorderDirection == line2D.IsInFront(GetPosition(face.Vertices[1]));
                    var is2 = line2D.BorderDirection == line2D.IsInFront(GetPosition(face.Vertices[2]));

                    if (is0 == is1 && is0 == is2)
                    {
                        // Discard if whole is outside
                        if(!is0)
                            toDiscard.Add(face);
                    }
                    else
                    {
                        toDiscard.Add(face);

                        // Clip triangle
                        s_inFrontVertices.Clear();
                        s_outFrontVertices.Clear();

                        void StoreVertex(Vertex v, bool inFront)
                        {
                            if(inFront)
                                s_inFrontVertices.Add(v);
                            else
                                s_outFrontVertices.Add(v);
                        }

                        StoreVertex(face.Vertices[0], is0);
                        StoreVertex(face.Vertices[1], is1);
                        StoreVertex(face.Vertices[2], is2);

                        if (s_inFrontVertices.Count == 1)
                        {
                            var vi0Pos = GetPosition(s_inFrontVertices[0]);
                            var vo1Pos = GetPosition(s_outFrontVertices[0]);
                            var vo2Pos = GetPosition(s_outFrontVertices[1]);

                            var vi0Index = Array.IndexOf(face.Vertices, s_inFrontVertices[0]);
                            var vo1Index =  Array.IndexOf(face.Vertices, s_outFrontVertices[0]);
                            var vo2Index =  Array.IndexOf(face.Vertices, s_outFrontVertices[1]);

                            var v3GO = new GameObject(nameof(Vertex), typeof(Vertex));
                            var v3 = v3GO.GetComponent<Vertex>();
                            v3.transform.SetParent(EffectsRoot.DestVerticesRoot.transform);
                            v3.Position = SetPosition(line2D.GetIntersection(vi0Pos, vo1Pos));

                            var v4GO = new GameObject(nameof(Vertex), typeof(Vertex));
                            var v4 = v4GO.GetComponent<Vertex>();
                            v4.transform.SetParent(EffectsRoot.DestVerticesRoot.transform);
                            v4.Position = SetPosition(line2D.GetIntersection(vi0Pos, vo2Pos));

                            var newFaceGO = new GameObject(nameof(Face), typeof(Face));
                            var newFace = newFaceGO.GetComponent<Face>();
                            newFace.transform.SetParent(EffectsRoot.DestFacesRoot.transform);

                            newFace.Vertices = new Vertex[3];
                            newFace.Vertices[vi0Index] = s_inFrontVertices[0];
                            newFace.Vertices[vo1Index] = v3;
                            newFace.Vertices[vo2Index] = v4;
                        }
                        else if(s_inFrontVertices.Count == 2)
                        {
                            var vi0Pos = GetPosition(s_inFrontVertices[0]);
                            var vi1Pos = GetPosition(s_inFrontVertices[1]);
                            var vo2Pos = GetPosition(s_outFrontVertices[0]);

                            var vi0Index = Array.IndexOf(face.Vertices, s_inFrontVertices[0]);
                            var vi1Index =  Array.IndexOf(face.Vertices, s_inFrontVertices[1]);
                            var vo2Index =  Array.IndexOf(face.Vertices, s_outFrontVertices[0]);

                            var v3GO = new GameObject(nameof(Vertex), typeof(Vertex));
                            var v3 = v3GO.GetComponent<Vertex>();
                            v3.transform.SetParent(EffectsRoot.DestVerticesRoot.transform);
                            v3.Position = SetPosition(line2D.GetIntersection(vi0Pos, vo2Pos));

                            var v4GO = new GameObject(nameof(Vertex), typeof(Vertex));
                            var v4 = v4GO.GetComponent<Vertex>();
                            v4.transform.SetParent(EffectsRoot.DestVerticesRoot.transform);
                            v4.Position = SetPosition(line2D.GetIntersection(vi1Pos, vo2Pos));

                            var newFace1GO = new GameObject(nameof(Face), typeof(Face));
                            var newFace1 = newFace1GO.GetComponent<Face>();
                            newFace1.transform.SetParent(EffectsRoot.DestFacesRoot.transform);

                            newFace1.Vertices = new Vertex[3];
                            newFace1.Vertices[vi0Index] = s_inFrontVertices[0];
                            newFace1.Vertices[vi1Index] = v4;
                            newFace1.Vertices[vo2Index] = v3;

                            var newFace2GO = new GameObject(nameof(Face), typeof(Face));
                            var newFace2 = newFace2GO.GetComponent<Face>();
                            newFace2.transform.SetParent(EffectsRoot.DestFacesRoot.transform);

                            newFace2.Vertices = new Vertex[3];
                            newFace2.Vertices[vi0Index] = s_inFrontVertices[0];
                            newFace2.Vertices[vi1Index] = s_inFrontVertices[1];
                            newFace2.Vertices[vo2Index] = v4;
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                }

                // Remove faces to discard
                while (toDiscard.Count > 0)
                {
                    var lastIndex = toDiscard.Count - 1;
                    var face = toDiscard[lastIndex];
                    toDiscard.RemoveAt(lastIndex);

                    if (!face)
                        continue;
                    
                    DestroyImmediate(face.gameObject);
                }

                faces = EffectsRoot.DestFacesRoot.GetComponentsInChildren<Face>();

                // Weld duplicated vertices
                var positionVertices = new Dictionary<int, Dictionary<int, Vertex>>();
                foreach (var face in faces)
                {
                    for (int i = 0; i < face.Vertices.Length; ++i)
                    {
                        var vertex = face.Vertices[i];
                        var x = Mathf.RoundToInt(1000.0f * vertex.Position.x);
                        var y = Mathf.RoundToInt(1000.0f * vertex.Position.z);

                        if(!positionVertices.ContainsKey(x))
                            positionVertices.Add(x, new Dictionary<int, Vertex>());

                        var column = positionVertices[x];
                        if (column.ContainsKey(y))
                            face.Vertices[i] = column[y];
                        else
                            column.Add(y, vertex);
                    }
                }

                // Remove invalid faces
                foreach (var face in faces)
                {
                    if(face.Vertices[0] == face.Vertices[1]
                       || face.Vertices[0] == face.Vertices[2]
                       || face.Vertices[1] == face.Vertices[2])
                        DestroyImmediate(face.gameObject);
                }

                // Gather used vertices
                var usedVertices = new HashSet<Vertex>();
                foreach (var face in faces)
                {
                    if (!face)
                        continue;

                    foreach (var vertex in face.Vertices)
                        usedVertices.Add(vertex);
                }

                // Remove unused vertices
                var vertices = EffectsRoot.DestVerticesRoot.GetComponentsInChildren<Vertex>();
                foreach (var vertex in vertices)
                {
                    if (usedVertices.Contains(vertex))
                        continue;

                    DestroyImmediate(vertex.gameObject);
                }
            }

            Vector2 GetPosition(Vertex v) => new Vector2(v.Position.x, v.Position.z);
            Vector3 SetPosition(Vector2 v) => new Vector3(v.x, 0.0f, v.y);
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var line in m_borders)
                line.DrawLineGizmo();
        }
    }
}