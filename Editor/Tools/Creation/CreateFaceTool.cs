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
                var faceGameObject = new GameObject(nameof(Face), typeof(Face));
                faceGameObject.transform.parent = m_facesRoot;

                var face = faceGameObject.GetComponent<Face>();
                face.Vertices = vertices.ToArray();
            }
        }
    }
}