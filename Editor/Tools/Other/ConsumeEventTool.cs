using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class ConsumeEventTool : ToolBase
    {
        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            wrapper.Consume();
        }
    }
}