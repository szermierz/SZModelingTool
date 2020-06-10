using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class TouchSelectTool : ToolBase
    {
        [SerializeField]
        private bool m_addToSelection = false;

        [SerializeField]
        private bool m_deselectIfSelected = true;

        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            var closest = Model
                .Vertices
                .Select(_vertex =>
                {
                    var vertexScreenPoint = sceneView.camera.WorldToScreenPoint(_vertex.Position);
                    vertexScreenPoint.z = 0;
                    var sqrDistance = (mousePos - (Vector2)vertexScreenPoint).sqrMagnitude;

                    return new Tuple<Vertex, float>(_vertex, sqrDistance);
                })
                .OrderBy(_pair => _pair.Item2)
                .FirstOrDefault();

            if (null == closest)
                return;

            if (wrapper.Consume())
            {
                var obj = (UnityEngine.Object)closest.Item1.gameObject;
                var isSelected = Selection.objects.Contains(obj);
                IEnumerable<UnityEngine.Object> targetSelection = Selection.objects;

                if (m_addToSelection)
                    targetSelection = targetSelection.Concat(Enumerable.Repeat(obj, 1));
                else
                    targetSelection = Enumerable.Repeat(obj, 1);

                if (m_deselectIfSelected && isSelected)
                    targetSelection = targetSelection.Where(_object => obj != _object);

                Selection.objects = targetSelection.ToArray();
            }
        }
    }
}