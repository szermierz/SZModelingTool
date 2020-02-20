using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public sealed class ToolsetMouseGroup : ToolsetGroup
    {
        [SerializeField]
        private int m_mouseButton = 1;
        private int MouseButton => m_mouseButton;

        [SerializeField]
        private EventType m_mouseEventType = default;
        private EventType MouseEventType => m_mouseEventType;

        protected override void OnEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        {
            if (MouseEventType != wrapper.EventType)
                return;

            if (MouseButton != wrapper.MouseButton)
                return;

            foreach (var tool in Tools)
                tool.ActivateTool(wrapper, vertices, sceneView, wrapper.MousePosition);
        }
    }
}