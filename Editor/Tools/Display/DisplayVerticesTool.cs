using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class DisplayVerticesTool : ToolBase
    {
        [SerializeField]
        private float m_sphereRadius = 0.1f;
        private float SphereRadius => m_sphereRadius;

        [SerializeField]
        private Color m_sphereColor = default;
        private Color SphereColor => m_sphereColor;

        [SerializeField]
        private Color m_selectedSphereColor = default;
        private Color SelectedSphereColor => m_selectedSphereColor;

        public override void DrawToolGizmo(ModelingToolBehaviour drawGizmo, SceneView sceneView, Vector2 mousePos)
        {
            base.DrawToolGizmo(drawGizmo, sceneView, mousePos);

            var vertex = drawGizmo as Vertex;
            if (!vertex)
                return;

            var prevColor = Gizmos.color;
            var destColor = vertex.Selected ? SelectedSphereColor : SphereColor;

            Gizmos.color = destColor;
            Gizmos.DrawSphere(vertex.Position, SphereRadius);
            Gizmos.color = prevColor;
        }
    }
}