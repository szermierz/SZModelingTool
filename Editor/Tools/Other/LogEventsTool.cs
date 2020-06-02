using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class LogEventsTool : ToolBase
    {
        [SerializeField]
        private bool m_logEventType = true;

        [SerializeField]
        private bool m_logMouseButton = true;

        [SerializeField]
        private bool m_logKeyCode = true;

        [SerializeField]
        private EventType[] m_ignoredEventTypes = default;

        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            if (m_ignoredEventTypes.Contains(wrapper.EventType))
                return;

            var builder = new StringBuilder();

            if (m_logEventType)
                builder.Append($"{BuilderPrefix()}EventType: {wrapper.EventType}");
            if (m_logMouseButton)
                builder.Append($"{BuilderPrefix()}MouseButton: {wrapper.MouseButton}");
            if (m_logKeyCode)
                builder.Append($"{BuilderPrefix()}KeyCode: {wrapper.KeyCode}");

            Debug.Log(builder.ToString());

            string BuilderPrefix() => builder.Length > 0 ? ", " : string.Empty;
        }
    }
}