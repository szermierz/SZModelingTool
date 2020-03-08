using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public sealed class Toolset : ModelingToolBehaviour
    {
        public Model Model => GetComponentInParent<Model>();

        public IEnumerable<ToolsetGroup> ToolsetGroups => GetComponentsInChildren<ToolsetGroup>();

        public void NotifyEvent(EditorEventWrapper wrapper, SceneView sceneView, IEnumerable<Vertex> vertices)
        {
            if(ShouldLog(wrapper.EventType))
                Debug.Log($"{wrapper.EventType}, {wrapper.MouseButton}, {wrapper.KeyCode}");

            foreach (var toolsetGroup in ToolsetGroups)
                toolsetGroup.NotifyEvent(wrapper, sceneView, vertices);

            //if (e.type == EventType.MouseDown && e.button == 2)
            //{
            //    Debug.Log("Middle Mouse was pressed");

            //    Vector3 mousePos = e.mousePosition;
            //    float ppp = EditorGUIUtility.pixelsPerPoint;
            //    mousePos.y = sceneView.camera.pixelHeight - mousePos.y * ppp;
            //    mousePos.x *= ppp;

            //    Ray ray = sceneView.camera.ScreenPointToRay(mousePos);
            //    RaycastHit hit;

            //    if (Physics.Raycast(ray, out hit))
            //    {
            //        //Do something, ---Example---
            //        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //        go.transform.position = hit.point;
            //        Debug.Log("Instantiated Primitive " + hit.point);
            //    }
            //    e.Use();
            //}
        }

        private bool ShouldLog(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.Repaint:
                case EventType.Layout:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                case EventType.Ignore:
                case EventType.Used:
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    return false;

                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                case EventType.ContextClick:
                    return true;

                default:
                    return true;
            }
        }

        public void DrawModelGizmo(ModelingToolBehaviour drawGizmo)
        {
            foreach (var toolsetGroup in ToolsetGroups)
                toolsetGroup.DrawGizmo(drawGizmo);
        }
    }
}