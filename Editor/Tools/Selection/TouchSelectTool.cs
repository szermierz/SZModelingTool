using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class TouchSelectTool : ToolBase
    {
        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            var closest = Model
                .Vertices
                .Select(_vertex =>
                {
                    var vertexScreenPoint = sceneView.camera.WorldToScreenPoint(_vertex.Position);
                    var sqrDistance = (mousePos - (Vector2)vertexScreenPoint).sqrMagnitude;

                    return new Tuple<Vertex, float>(_vertex, sqrDistance);
                })
                .OrderBy(_pair => _pair.Item2)
                .FirstOrDefault();

            if (null == closest)
                return;

            if(wrapper.Consume())
                Selection.objects = Enumerable.Repeat(closest.Item1.gameObject, 1).ToArray();
        }
    }
}