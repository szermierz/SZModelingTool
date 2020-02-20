﻿using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class Face : ModelingToolBehaviour
    {
        public Vector3 Position => Vector3.zero;

        public Vertex[] Vertices = default;

        public override bool Selected => Vertices.All(_vertex => _vertex.Selected);

        public Model Model => GetComponentInParent<Model>();

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawEditorGizmos(Face face, GizmoType gizmoType)
        {
            face.Model.DrawModelGizmos(face);
        }
    }
}