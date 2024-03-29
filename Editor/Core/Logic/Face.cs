﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    [ExecuteInEditMode]
    public class Face : ModelingToolBehaviour
    {
        public Vector3 Position => Vector3.zero;

        public bool IsDestroying { get; private set; } = false;

        [SerializeField]
        private Vertex[] m_vertices = default;
        public Vertex[] Vertices
        {
            get => m_vertices ?? Array.Empty<Vertex>();
            set => m_vertices = value;
        }

        public override bool Selected => LocallySelected || Vertices.All(_vertex => _vertex ? _vertex.Selected : true);

        public bool IsValid => Vertices.Count() == 3
            && Vertices[0] && Vertices[0].gameObject.activeInHierarchy
            && Vertices[1] && Vertices[1].gameObject.activeInHierarchy
            && Vertices[2] && Vertices[2].gameObject.activeInHierarchy;

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

        private void OnDestroy()
        {
            IsDestroying = true;
            DrawEditorGizmos(this, GizmoType.NonSelected);
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawEditorGizmos(Face face, GizmoType gizmoType)
        {
            if(face && face.Model)
                face.Model.DrawModelGizmos(face);
        }
    }
}