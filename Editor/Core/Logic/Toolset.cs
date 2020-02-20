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

        public void DrawModelGizmo(ModelingToolBehaviour drawGizmo)
        {
            foreach (var toolsetGroup in ToolsetGroups)
                toolsetGroup.DrawGizmo(drawGizmo);
        }
    }
}