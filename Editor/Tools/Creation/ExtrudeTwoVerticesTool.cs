using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class ExtrudeTwoVerticesTool : ToolBase
    {
        [SerializeField]
        private string m_vertexNameBase = "V";

        public override void ActivateTool(EditorEventWrapper wrapper, IEnumerable<Vertex> vertices, SceneView sceneView, Vector2 mousePos)
        {
            base.ActivateTool(wrapper, vertices, sceneView, mousePos);

            if(wrapper.Consume() && vertices.Any())
            {
                HashSet<string> names = null;
                List<GameObject> newVertices = new List<GameObject>();
                var parent = vertices.First().transform.parent;

                foreach (var v in vertices)
                {
                    newVertices.Add(Instantiate(v.gameObject, parent));
                    newVertices.Last().name = GetFreeName(parent, m_vertexNameBase, names);
                }

                Selection.objects = newVertices.ToArray();
            }
        }

        private static string GetFreeName(Transform parent, string nameBase, HashSet<string> namesCache = null)
        {
            if (null == namesCache)
            {
                namesCache = new HashSet<string>();
                foreach (Transform item in parent)
                    namesCache.Add(item.name);
            }

            for(int i = 0; ; ++i)
            {
                var candidate = $"{nameBase} ({i})";
                if (!namesCache.Contains(candidate))
                {
                    namesCache.Add(candidate);
                    return candidate;
                }
            }
        }
    }
}
