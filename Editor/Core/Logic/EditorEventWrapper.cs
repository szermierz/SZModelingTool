using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public sealed class EditorEventWrapper
    {
        public EditorEventWrapper(Event _event, SceneView _sceneView)
        {
            Event = _event;
            SceneView = _sceneView;
        }

        public readonly Event Event;
        public readonly SceneView SceneView;

        public bool IsConsumed { get; private set; }

        public bool Consume()
        {
            if (IsConsumed)
                return false;

            IsConsumed = true;
            Event.Use();

            return true;
        }

        public EventType EventType => Event.type;

        public int MouseButton => Event.button;

        public Vector2 MousePosition => new Vector2(Event.mousePosition.x, SceneView.camera.pixelHeight - Event.mousePosition.y);

        public KeyCode KeyCode => Event.keyCode;
    }
}