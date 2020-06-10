using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class DeleteFaceTool : ToolBase
    {
        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            if (vertices.Count() != 3)
                return;

            var face = vertices.First().Model.Faces.FirstOrDefault(IsFace);

            if (wrapper.Consume())
            {
                DestroyImmediate(face.gameObject);
            }

            bool IsFace(Face _face)
            {
                if (_face.Vertices.Length != 3)
                    return false;
                if (!vertices.Contains(_face.Vertices[0]))
                    return false;
                if (!vertices.Contains(_face.Vertices[1]))
                    return false;
                if (!vertices.Contains(_face.Vertices[2]))
                    return false;

                return true;
            }
        }


    }
}