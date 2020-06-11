using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class Face : ModelingToolBehaviour
    {
        public Vector3 Position => Vector3.zero;

        [SerializeField]
        private Vertex[] m_vertices = default;
        public Vertex[] Vertices
        {
            get => m_vertices ?? Array.Empty<Vertex>();
            set => m_vertices = value;
        }

        public override bool Selected => LocallySelected || Vertices.All(_vertex => _vertex ? _vertex.Selected : true);

        public virtual bool LocallySelected
        {
            get
            {
                var gameObjects = Selection.objects.OfType<GameObject>();
                var possibles = gameObjects.SelectMany(_gameObject => _gameObject.GetComponentsInChildren(GetType()));
                if (possibles.Contains(this))
                    return true;

                if (gameObjects.Any(_gameObject => _gameObject.GetComponent(GetType()) == this))
                    return true;

                return false;
            }
        }

        public Model Model => GetComponentInParent<Model>();

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawEditorGizmos(Face face, GizmoType gizmoType)
        {
            if(face && face.Model)
                face.Model.DrawModelGizmos(face);
        }
    }
}