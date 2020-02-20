using System.Collections.Generic;
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

        protected override void OnEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        {
            if (KeyboardEventType != wrapper.EventType)
                return;

            if (KeyCode != wrapper.KeyCode)
                return;

            foreach (var tool in Tools)
                tool.ActivateTool(wrapper, vertices, sceneView, wrapper.MousePosition);
        }
    }
}