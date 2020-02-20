using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace SZ.ModelingTool
{
    [ExecuteInEditMode]
    public class Model : ModelingToolBehaviour
    {
        public Toolset Toolset => GetComponentInChildren<Toolset>();
        public IEnumerable<Vertex> Vertices => GetComponentsInChildren<Vertex>();
        public IEnumerable<Face> Faces => GetComponentsInChildren<Face>();

        protected virtual bool IsEditor => Application.isEditor && !Application.isPlaying;

        protected virtual void OnEnable()
        {
            if (!IsEditor)
                return;

            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        protected virtual void OnDisable()
        {
            if (!IsEditor)
                return;

            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        protected virtual void OnSceneGUI(SceneView sceneView)
        {
            if (!IsEditor || !Toolset)
                return;

            var selectedVertices = Selection
                .objects
                .Select(_selectedObject => _selectedObject as GameObject)
                .Where(_selectedGameObject => _selectedGameObject)
                .Select(_selectedGameObject => _selectedGameObject.GetComponent<Vertex>())
                .ToArray();

            Utilities.EditorTools.Hidden = selectedVertices.Any();
            Toolset.NotifyEvent(new EditorEventWrapper(Event.current), sceneView, selectedVertices);
        }

        public virtual void DrawModelGizmos(ModelingToolBehaviour drawGizmo)
        {
            Toolset.DrawModelGizmo(drawGizmo);
        }
    }
}