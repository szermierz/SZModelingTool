using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class CreateEdgeTool : ToolBase
    {
        [SerializeField]
        private Transform m_edgesRoot = default;

        [SerializeField]
        private bool m_deleteOnExisting = true;

        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            if (!wrapper.Consume())
                return;

            if (2 != vertices.Count())
                return;

            var v1 = vertices.ElementAt(0);
            var v2 = vertices.ElementAt(1);

            var edges = m_edgesRoot.GetComponentsInChildren<Edge>();
            var existing = edges.FirstOrDefault(_edge => Matching(_edge));

            if (existing)
            {
                if (m_deleteOnExisting)
                    DestroyImmediate(existing.gameObject);
            }
            else
            {
                var edge = new GameObject(nameof(Edge), typeof(Edge)).GetComponent<Edge>();
                edge.transform.SetParent(m_edgesRoot);
                edge.V1 = v1;
                edge.V2 = v2;
            }

            bool Matching(Edge edge)
            {
                var matching = false;
                matching |= edge.V1 == v1 && edge.V2 == v2;
                matching |= edge.V1 == v2 && edge.V2 == v1;
                return matching;
            }
        }
    }
}