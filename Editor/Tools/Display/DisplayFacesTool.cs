using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class DisplayFacesTool : ToolBase
    {
        [SerializeField]
        private MeshFilter m_nonSelectedMesh = default;

        [SerializeField]
        private MeshFilter m_selectedMesh = default;

        private Dictionary<Face, FaceDesc> m_faces = new Dictionary<Face, FaceDesc>();

        public override void DrawToolGizmo(ModelingToolBehaviour drawGizmo, SceneView sceneView, Vector2 mousePos)
        {
            base.DrawToolGizmo(drawGizmo, sceneView, mousePos);

            var face = drawGizmo as Face;
            if (!face)
                return;

            if (face.Vertices.Length != 3)
                return;

            bool dirty = face;
            
            if(!m_faces.ContainsKey(face))
            {
                dirty = true;
                m_faces.Add(face, FaceDesc.FromFace(face));
            }
            else if(!m_faces[face].Equals(face))
            {
                dirty = true;
                m_faces[face] = FaceDesc.FromFace(face);
            }

            if (dirty)
                Redraw();
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
                    return Vertices(
                        _face.Vertices[0].Position,
                        _face.Vertices[1].Position,
                        _face.Vertices[2].Position
                        );
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

        private static IEnumerable<Vector3> Vertices(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            yield return v1;
            yield return v2;
            yield return v3;
        }

        private struct FaceDesc
        {
            public Vector3[] Positions;
            public bool Selected;

            public bool Equals(Face face)
            {
                if (face.Vertices == null)
                    throw new ArgumentNullException("Invalid face");
                if (face.Vertices.Length != 3)
                    throw new ArgumentException("Invalid face");

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
                    Positions = face.Vertices.Select(_vertex => _vertex.Position).ToArray(),
                };
            }
        }
    }
}