using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    [ExecuteInEditMode]
    public class DisplayFacesTool : ToolBase
    {
        [SerializeField]
        private MeshFilter m_nonSelectedMesh = default;

        [SerializeField]
        private MeshFilter m_selectedMesh = default;

        [SerializeField]
        private float m_redrawInterval = 0.0f;

        [SerializeField]
        private bool m_drawEdges;

        private Dictionary<Face, FaceDesc> m_faces = new Dictionary<Face, FaceDesc>();

        private DateTime? m_nextRedraw = null;

        private bool m_dirty = false;

        private void OnEnable()
        {
            m_nonSelectedMesh.gameObject.SetActive(true);
            m_selectedMesh.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            m_nonSelectedMesh.gameObject.SetActive(false);
            m_selectedMesh.gameObject.SetActive(false);
        }

        public override void DrawToolGizmo(ModelingToolBehaviour drawGizmo, SceneView sceneView, Vector2 mousePos)
        {
            base.DrawToolGizmo(drawGizmo, sceneView, mousePos);

            var face = drawGizmo as Face;
            if (!face)
                return;

            if (face.Vertices.Length != 3 || face.Vertices.Any(_vertex => !_vertex))
                return;

            if(face.IsDestroying)
            {
                m_dirty = true;
                m_faces.Remove(face);
            }
            else if(!m_faces.ContainsKey(face))
            {
                m_dirty = true;
                m_faces.Add(face, FaceDesc.FromFace(face));
            }
            else if(!m_faces[face].Equals(face))
            {
                m_dirty = true;
                m_faces[face] = FaceDesc.FromFace(face);
            }

            if(m_nextRedraw.HasValue && m_nextRedraw <= DateTime.Now)
                m_nextRedraw = null;

            if (m_dirty && null == m_nextRedraw)
            {
                Redraw();

                m_dirty = false;
                m_nextRedraw = DateTime.Now + TimeSpan.FromSeconds(m_redrawInterval);
            }

            if (m_drawEdges)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(face.Vertices[0].Position, face.Vertices[1].Position);
                Gizmos.DrawLine(face.Vertices[0].Position, face.Vertices[2].Position);
                Gizmos.DrawLine(face.Vertices[1].Position, face.Vertices[2].Position);
            }
        }

        private static List<Face> _facesBuffer = new List<Face>();
        private void Redraw()
        {
            RedrawMesh(_face => _face.Selected, m_selectedMesh);            
            RedrawMesh(_face => !_face.Selected, m_nonSelectedMesh);            
        }

        private void RedrawMesh(Func<Face, bool> validator, MeshFilter destMesh)
        {
            _facesBuffer.Clear();
            _facesBuffer.AddRange(
                m_faces
                    .Where(_faceRecord => validator(_faceRecord.Key))
                    .Select(_faceRecord => _faceRecord.Key)
                );

            var vertices = _facesBuffer
                .SelectMany(_face =>
                {
                    return _face
                        .Vertices
                        .Select(_vertex => _vertex ? _vertex.Position : Vector3.zero);
                })
                .ToArray();

            var indices = Enumerable.Range(0, vertices.Length).ToArray();
            var normals = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i += 3)
            {
                var normal = Vector3.Cross(vertices[i + 1] - vertices[i + 0], vertices[i + 2] - vertices[i + 0]);

                normals[i + 0] = normal;
                normals[i + 1] = normal;
                normals[i + 2] = normal;
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.SetNormals(normals);

            destMesh.mesh = mesh;
        }

        private struct FaceDesc
        {
            public Vector3[] Positions;
            public bool Selected;

            public bool Equals(Face face)
            {
                if (face.Vertices == null)
                    return false;
                if (face.Vertices.Length != 3)
                    return false;
                if (face.Vertices.Any(_vertex => !_vertex))
                    return false;

                if (face.Selected != Selected)
                    return false;

                if (Positions == null)
                    Positions = new Vector3[3];

                for (int i = 0; i < 3; ++i)
                    if (Positions[i] != face.Vertices[i].Position)
                        return false;

                return true;
            }

            public static FaceDesc FromFace(Face face)
            {
                return new FaceDesc()
                {
                    Selected = face.Selected,
                    Positions = face.Vertices.Select(_vertex => _vertex ? _vertex.Position : Vector3.zero).ToArray(),
                };
            }
        }
    }
}