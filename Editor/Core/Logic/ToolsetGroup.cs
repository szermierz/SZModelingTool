using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public abstract class ToolsetGroup : ModelingToolBehaviour
    {
        public Model Model => Toolset.Model;
        public Toolset Toolset => GetComponentInParent<Toolset>();

        protected IEnumerable<ToolBase> Tools => GetComponentsInChildren<ToolBase>();

        protected Vector2 MousePosition { get; private set; }
        protected SceneView SceneView { get; private set; }

        public virtual void DrawGizmo(ModelingToolBehaviour drawGizmo)
        {
            foreach (var tool in Tools)
                DrawGizmo(drawGizmo, tool);
        }

        protected virtual void DrawGizmo(ModelingToolBehaviour drawGizmo, ToolBase tool)
        {
            tool.DrawToolGizmo(drawGizmo, SceneView, MousePosition);
        }

        public void NotifyEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        {
            SceneView = sceneView;
            MousePosition = wrapper.MousePosition;
            OnEvent(wrapper, sceneView, vertices);
        }

        protected abstract void OnEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices);
    }
}