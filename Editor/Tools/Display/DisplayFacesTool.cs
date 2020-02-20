using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class DisplayFacesTool : ToolBase
    {
        [SerializeField]
        private Color m_faceColor = default;
        private Color FaceColor => m_faceColor;

        public override void DrawToolGizmo(ModelingToolBehaviour drawGizmo, SceneView sceneView, Vector2 mousePos)
        {
            var face = drawGizmo as Face;
            if (!face)
                return;

            if (face.Vertices.Length != 3)
                return;

            var prevColor = Handles.color;
            Handles.color = FaceColor;

            Handles.DrawAAConvexPolygon(face.Vertices.Select(vertex => vertex.Position).ToArray());

            Handles.color = prevColor;
        }
    }
}