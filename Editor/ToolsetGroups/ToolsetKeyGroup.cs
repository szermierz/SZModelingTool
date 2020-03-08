using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public sealed class ToolsetKeyGroup : ToolsetGroup
    {
        [SerializeField]
        private KeyCode m_keyCode = default;
        private KeyCode KeyCode => m_keyCode;

        [SerializeField]
        private EventType m_keyboardEventType = default;
        private EventType KeyboardEventType => m_keyboardEventType;

        [SerializeField]
        private EventModifiers[] m_requiredModifiers = default;
        private IEnumerable<EventModifiers> RequiredModifiers => m_requiredModifiers ?? Enumerable.Empty<EventModifiers>();

        protected override void OnEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        {
            if (KeyboardEventType != wrapper.EventType)
                return;

            if (KeyCode != wrapper.KeyCode)
                return;

            if (RequiredModifiers.Any() && !RequiredModifiers.All(_modifier => wrapper.Event.modifiers.HasFlag(_modifier)))
                return;

            foreach (var tool in Tools)
                tool.ActivateTool(wrapper, vertices, sceneView, wrapper.MousePosition);
        }
    }
}