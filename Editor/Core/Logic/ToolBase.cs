using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public abstract class ToolBase : ModelingToolBehaviour
    {
        public Model Model => Toolset.Model;
        public Toolset Toolset => ToolsetGroup.Toolset;
        public ToolsetGroup ToolsetGroup => GetComponentInParent<ToolsetGroup>();

        public virtual void DrawToolGizmo(ModelingToolBehaviour drawGizmo, SceneView sceneView, Vector2 mousePos)
        { }

        public virtual void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        { }
    }
}