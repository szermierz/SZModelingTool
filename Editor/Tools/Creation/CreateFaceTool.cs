using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class CreateFaceTool : ToolBase
    {
        [SerializeField]
        private Transform m_facesRoot = default;

        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            if(wrapper.Consume() && vertices.Count() == 3)
            {
                var faces = m_facesRoot.GetComponentsInChildren<Face>();
                var existingFace = faces.FirstOrDefault(_face => EqualVertices(vertices, _face.Vertices));
                if(existingFace)
                {
                    existingFace.Vertices = new Vertex[] 
                    {
                        existingFace.Vertices[0],
                        existingFace.Vertices[2],
                        existingFace.Vertices[1],
                    };
                }
                else
                {
                    var faceGameObject = new GameObject(nameof(Face), typeof(Face));
                    faceGameObject.transform.parent = m_facesRoot;

                    var face = faceGameObject.GetComponent<Face>();
                    face.Vertices = vertices.ToArray();
                }

            }
        }

        private static bool EqualVertices(IEnumerable<Vertex> lhs, IEnumerable<Vertex> rhs)
        {
            var leftIt = lhs.GetEnumerator(); leftIt.MoveNext();
            var v1 = leftIt.Current; leftIt.MoveNext();
            var v2 = leftIt.Current; leftIt.MoveNext();
            var v3 = leftIt.Current;

            if (v1 == v2 || v1 == v3 || v2 == v3)
                return false;

            var rightIt = rhs.GetEnumerator(); rightIt.MoveNext();
            var v4 = rightIt.Current; rightIt.MoveNext();
            var v5 = rightIt.Current; rightIt.MoveNext();
            var v6 = rightIt.Current; rightIt.MoveNext();

            if (v4 == v5 || v4 == v6 || v5 == v6)
                return false;

            if (v1 != v4 && v1 != v5 && v1 != v6)
                return false;
            if (v2 != v4 && v2 != v5 && v2 != v6)
                return false;
            if (v3 != v4 && v3 != v5 && v3 != v6)
                return false;

            return true;
        }
    }
}