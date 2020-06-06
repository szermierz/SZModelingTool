using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public sealed class ToolsetEverythingGroup : ToolsetGroup
    {
        [SerializeField]
        private EventType[] m_ignoredEvents = default;
        
        protected override void OnEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        {
            if (m_ignoredEvents.Contains(wrapper.EventType))
                return;

            foreach (var tool in Tools)
                tool.ActivateTool(wrapper, vertices, sceneView, wrapper.MousePosition);
        }
    }
}
