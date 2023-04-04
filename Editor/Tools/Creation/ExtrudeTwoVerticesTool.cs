using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class ExtrudeTwoVerticesTool : ToolBase
    {
        [SerializeField]
        private Transform m_edgesRoot;

        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            if (!wrapper.Consume())
                return;

            if (vertices.Count() != 2)
                return;

            var v1 = vertices.ElementAt(0);
            var v2 = vertices.ElementAt(1);

            var newV1 = Spawn<Vertex>(v1.transform.parent);
            newV1.Position = v1.Position;

            var newV2 = Spawn<Vertex>(v2.transform.parent);
            newV2.Position = v2.Position;

            if(m_edgesRoot)
            {
                var newEdge1 = Spawn<Edge>(m_edgesRoot);
                newEdge1.V1 = v1;
                newEdge1.V2 = newV1;
                var newEdge2 = Spawn<Edge>(m_edgesRoot);
                newEdge2.V1 = newV1;
                newEdge2.V2 = newV2;
                var newEdge3 = Spawn<Edge>(m_edgesRoot);
                newEdge3.V1 = newV2;
                newEdge3.V2 = v2;
                var newEdge4 = Spawn<Edge>(m_edgesRoot);
                newEdge4.V1 = v1;
                newEdge4.V2 = newV2;
            }

            Selection.objects = new[] { newV1.gameObject, newV2.gameObject };
        }
    }
}
