using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class DisplayEdgesTool : ToolBase
    {
        [SerializeField]
        private Color m_defaultColor = default;
        private Color DefaultColor => m_defaultColor;

        [SerializeField]
        private Color m_selectedColor = default;
        private Color SelectedColor => m_selectedColor;

        public override void DrawToolGizmo(ModelingToolBehaviour drawGizmo, SceneView sceneView, Vector2 mousePos)
        {
            base.DrawToolGizmo(drawGizmo, sceneView, mousePos);

            var edge = drawGizmo as Edge;
            if (!edge || !edge.Valid)
                return;

            var prevColor = Gizmos.color;
            var destColor = edge.Selected ? SelectedColor : DefaultColor;

            Gizmos.color = destColor;
            Gizmos.DrawLine(edge.V1.Position, edge.V2.Position);
            Gizmos.color = prevColor;
        }
    }
}