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

        public Dictionary<Vertex, Vertex> OldByNewVertices { get; private set; }
        public Dictionary<Vertex, Vertex> NewByOldVertices { get; private set; }
        public Dictionary<Face, Face> OldByNewFaces { get; private set; }
        public Dictionary<Face, Face> NewByOldFaces { get; private set; }
        public IEnumerable<Vertex> OldVertices => OldByNewVertices.Keys;
        public IEnumerable<Vertex> NewVertices => OldByNewVertices.Values;
        public IEnumerable<Face> OldFaces => OldByNewFaces.Keys;
        public IEnumerable<Face> NewFaces => OldByNewFaces.Values;

        public void RunEffects()
        {
            var sourceVericesRootActive = SourceVerticesRoot.activeSelf;
            var sourceFacesRootActive = SourceFacesRoot.activeSelf;
            var destVerticesRootActive = DestVerticesRoot.activeSelf;
            var destFacesRootActive = DestFacesRoot.activeSelf;

            SourceVerticesRoot.SetActive(true);
            SourceFacesRoot.SetActive(true);
            DestVerticesRoot.SetActive(true);
            DestFacesRoot.SetActive(true);

            ClearDestination();

            OldByNewVertices = null;
            NewByOldVertices = null;
            OldByNewFaces = null;
            NewByOldFaces = null;

            foreach (var effect in Effects)
                effect.Run(this);

            SourceVerticesRoot.SetActive(false);
            SourceFacesRoot.SetActive(false);
            DestVerticesRoot.SetActive(true);
            DestFacesRoot.SetActive(true);

            if(m_triggerNextEffects)
            {
                var nextEffectsRoot = DestVerticesRoot.GetComponentInParent<EffectsRoot>();
                if(!nextEffectsRoot)
                    nextEffectsRoot = DestFacesRoot.GetComponentInParent<EffectsRoot>();

                if (nextEffectsRoot)
                    nextEffectsRoot.RunEffects();
            }

            SourceVerticesRoot.SetActive(sourceVericesRootActive);
            SourceFacesRoot.SetActive(sourceFacesRootActive);
            DestVerticesRoot.SetActive(destVerticesRootActive);
            DestFacesRoot.SetActive(destFacesRootActive);
        }

        public void CloneModel(bool cloneVertices, bool cloneFaces)
        {
            if (cloneVertices)
            {
                if (null == OldByNewVertices || null == NewByOldVertices)
                {
                    CloneModel<Vertex>(SourceVerticesRoot, DestVerticesRoot, out var oldByNew, out var newByOld);
                    OldByNewVertices = oldByNew;
                    NewByOldVertices = newByOld;
                }
            }
            else
            {
                OldByNewVertices ??= new();
                NewByOldVertices ??= new();
            }

            if (cloneFaces)
            {
                if (null == OldByNewFaces || null == NewByOldFaces)
                {
                    CloneModel<Face>(SourceFacesRoot, DestFacesRoot, out var oldByNew, out var newByOld);
                    OldByNewFaces = oldByNew;
                    NewByOldFaces = newByOld;
                }
            }
            else
            {
                OldByNewFaces ??= new();
                NewByOldFaces ??= new();
            }

            foreach (var face in OldByNewFaces)
            {
                var newFace = face.Value;

                for(int i = 0; i < newFace.Vertices.Length; ++i)
                {
                    var oldVertex = newFace.Vertices[i];
                    var newVertex = OldByNewVertices.ContainsKey(oldVertex) ? OldByNewVertices[oldVertex] : null;
                    newFace.Vertices[i] = newVertex;
                }
            }
        }

        private void CloneModel<T>(GameObject root, GameObject dest, out Dictionary<T, T> oldByNew, out Dictionary<T, T> newByOld)
            where T : Component
        {
            oldByNew = new Dictionary<T, T>();
            newByOld = new Dictionary<T, T>();

            var components = root.GetComponentsInChildren<T>();

            foreach (var component in components)
            {
                var newGameObject = Instantiate(component.gameObject, dest.transform);
                newGameObject.transform.position = component.transform.position;
                newGameObject.transform.rotation = component.transform.rotation;
                
                var newComponent = newGameObject.GetComponent<T>();

                newByOld.Add(newComponent, component);
                oldByNew.Add(component, newComponent);
            }
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