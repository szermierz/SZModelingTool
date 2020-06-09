using System.Collections.Generic;
using System.Linq;
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

        [SerializeField]
        private EventModifiers[] m_requiredModifiers = default;
        private IEnumerable<EventModifiers> RequiredModifiers => m_requiredModifiers ?? Enumerable.Empty<EventModifiers>();

        protected override void OnEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        {
            if (MouseEventType != wrapper.EventType)
                return;

            if (MouseButton != wrapper.MouseButton)
                return;

            if (RequiredModifiers.Any() && !RequiredModifiers.All(_modifier => wrapper.Event.modifiers.HasFlag(_modifier)))
                return;

            foreach (var tool in Tools)
                tool.ActivateTool(wrapper, vertices, sceneView, wrapper.MousePosition);
        }
    }
}