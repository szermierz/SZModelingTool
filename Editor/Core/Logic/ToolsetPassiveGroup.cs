using System.Collections.Generic;
using UnityEditor;

namespace SZ.ModelingTool
{
    public sealed class ToolsetPassiveGroup : ToolsetGroup
    {
        protected override void OnEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        { }
    }
}