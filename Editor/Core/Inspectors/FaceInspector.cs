using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Face))]
    public class FaceInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Select vertices"))
            {
                var toSelect = new List<GameObject>(3 * targets.Length);
                foreach (var target in targets)
                {
                    var face = (Face) target;
                    foreach (var vertex in face.Vertices)
                    {
                        var go = vertex.gameObject;
                        if(!toSelect.Contains(go))
                            toSelect.Add(go);
                    }
                }

                Selection.objects = toSelect.ToArray();
            }
        }
    }
}