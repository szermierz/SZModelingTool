using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace SZ.ModelingTool
{
    [ExecuteInEditMode]
    public class Model : ModelingToolBehaviour
    {
        [SerializeField]
        private string m_modelName = string.Empty;
        public virtual string ModelName => m_modelName;
        
        public Toolset Toolset => GetComponentInChildren<Toolset>();
        public IEnumerable<Vertex> Vertices => GetComponentsInChildren<Vertex>();
        public IEnumerable<Face> Faces => GetComponentsInChildren<Face>();
        public IEnumerable<Edge> Edges => GetComponentsInChildren<Edge>();

        protected virtual bool IsEditor => Application.isEditor && !Application.isPlaying;

        protected virtual void OnEnable()
        {
            if (!IsEditor)
                return;

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        protected virtual void OnDisable()
        {
            if (!IsEditor)
                return;

            SceneView.duringSceneGui -= OnSceneGUI;
        }

        protected virtual void OnSceneGUI(SceneView sceneView)
        {
            if (!IsEditor || !Toolset)
                return;

            var selectedVertices = Selection
                .objects
                .Select(_selectedObject => _selectedObject as GameObject)
                .Where(_selectedGameObject => _selectedGameObject)
                .SelectMany(_selectedGameObject => _selectedGameObject.GetComponentsInChildren<Vertex>().Concat(Enumerable.Repeat(_selectedGameObject.GetComponent<Vertex>(), 1)))
                .Where(_vertex => _vertex)
                .Distinct()
                .ToArray();
            
            //Utilities.EditorTools.Hidden = selectedVertices.Any();
            Toolset.NotifyEvent(new EditorEventWrapper(Event.current, sceneView), sceneView, selectedVertices);
        }

        public virtual void DrawModelGizmos(ModelingToolBehaviour drawGizmo)
        {
            Toolset.DrawModelGizmo(drawGizmo);
        }

        private void OnValidate()
        {
            if(string.IsNullOrEmpty(m_modelName))
            {
                m_modelName = name;
                EditorUtility.SetDirty(this);
            }
        }
    }
}