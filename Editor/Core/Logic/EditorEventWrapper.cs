using UnityEngine;

namespace SZ.ModelingTool
{
    public sealed class EditorEventWrapper
    {
        public EditorEventWrapper(Event _event)
        {
            Event = _event;
        }

        public readonly Event Event;

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

        public Vector2 MousePosition => Event.mousePosition;

        public KeyCode KeyCode => Event.keyCode;
    }
}