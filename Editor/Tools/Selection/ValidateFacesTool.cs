using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class ValidateFacesTool : ToolBase
    {
        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            var v = vertices.FirstOrDefault();
            if (!v)
                return;

            var invalidFaces = v.Model.Faces.Where(_face =>
            {
                if (_face.Vertices.Length != 3)
                    return true;

                if (_face.Vertices.Any(_vertex => !_vertex))
                    return true;

                return false;
            }).ToArray();

            if (invalidFaces.Any() && wrapper.Consume())
            {
                foreach (var item in invalidFaces)
                    DestroyImmediate(item.gameObject);

                Debug.Log($"Destroyed {invalidFaces.Length} invalid faces");
            }
        }


    }
}