using System.Collections.Generic;
using UnityEngine;

namespace SZ.ModelingTool
{
    public sealed class EffectsRoot : ModelingToolBehaviour
    {
        [Header("Source")]
        [Space(5)]

        [SerializeField]
        private GameObject m_sourceVerticesRoot = default;
        public GameObject SourceVerticesRoot => m_sourceVerticesRoot;

        [SerializeField]
        private GameObject m_sourceFacesRoot = default;
        public GameObject SourceFacesRoot => m_sourceFacesRoot;

        [Header("Destination")]
        [Space(5)]

        [SerializeField]
        private GameObject m_destVerticesRoot = default;
        public GameObject DestVerticesRoot => m_destVerticesRoot;

        [SerializeField]
        private GameObject m_destFacesRoot = default;
        public GameObject DestFacesRoot => m_destFacesRoot;

        public bool IsValid => SourceVerticesRoot
            && SourceFacesRoot
            && DestVerticesRoot
            && DestFacesRoot;

        [Space(5)]
        [SerializeField]
        private bool m_triggerNextEffects = true;

        public IEnumerable<EffectBase> Effects => GetComponents<EffectBase>();

        private Dictionary<Vertex, Vertex> m_vertices = null;
        private Dictionary<Face, Face> m_faces = null;

        public void RunEffects()
        {
            SourceVerticesRoot.SetActive(true);
            SourceFacesRoot.SetActive(true);
            DestVerticesRoot.SetActive(true);
            DestFacesRoot.SetActive(true);

            ClearDestination();

            m_vertices = null;
            m_faces = null;

            foreach (var effect in Effects)
                effect.Run(this);

            SourceVerticesRoot.SetActive(false);
            SourceFacesRoot.SetActive(false);
            DestVerticesRoot.SetActive(true);
            DestFacesRoot.SetActive(true);

            if(m_triggerNextEffects)
            {
                var nextEffectsRoot = DestVerticesRoot.GetComponent<EffectsRoot>();
                if(!nextEffectsRoot)
                    nextEffectsRoot = DestFacesRoot.GetComponent<EffectsRoot>();

                if (nextEffectsRoot)
                    nextEffectsRoot.RunEffects();
            }
        }

        public void CloneModel(out Dictionary<Vertex, Vertex> vertices, out Dictionary<Face, Face> faces)
        {
            if(null == m_vertices)
                m_vertices = CloneModel<Vertex>(SourceVerticesRoot, DestVerticesRoot);

            if (null == m_faces)
                m_faces = CloneModel<Face>(SourceFacesRoot, DestFacesRoot);

            foreach (var face in m_faces)
            {
                var newFace = face.Value;

                for(int i = 0; i < newFace.Vertices.Length; ++i)
                {
                    var oldVertex = newFace.Vertices[i];
                    var newVertex = m_vertices.ContainsKey(oldVertex) ? m_vertices[oldVertex] : null;
                    newFace.Vertices[i] = newVertex;
                }
            }

            vertices = m_vertices;
            faces = m_faces;
        }

        private Dictionary<T, T> CloneModel<T>(GameObject root, GameObject dest)
            where T : Component
        {
            var result = new Dictionary<T, T>();
            var components = root.GetComponentsInChildren<T>();

            foreach (var component in components)
            {
                var newGameObject = Instantiate(component.gameObject, dest.transform);
                var newComponent = newGameObject.GetComponent<T>();

                result.Add(component, newComponent);
            }

            return result;
        }

        private void ClearDestination()
        {
            ClearChildren(DestVerticesRoot.transform);
            ClearChildren(DestFacesRoot.transform);

            void ClearChildren(Transform t)
            {
                while (t.childCount > 0)
                    DestroyImmediate(t.GetChild(0).gameObject);
            }
        }
    }
}