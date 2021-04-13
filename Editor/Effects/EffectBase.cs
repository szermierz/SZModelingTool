using System.Collections.Generic;

namespace SZ.ModelingTool
{
    public abstract class EffectBase : ModelingToolBehaviour
    {
        protected EffectsRoot EffectsRoot { get; private set; }

        private Dictionary<Vertex, Vertex> m_oldByNewVertices = null;
        private Dictionary<Vertex, Vertex> m_newByOldVertices = null;
        private Dictionary<Face, Face> m_oldByNewFaces = null;
        private Dictionary<Face, Face> m_newByOldFaces = null;

        protected Dictionary<Vertex, Vertex> OldByNewVertices => GetCachedDictionary(ref m_oldByNewVertices);
        protected Dictionary<Vertex, Vertex> NewByOldVertices => GetCachedDictionary(ref m_newByOldVertices);
        protected Dictionary<Face, Face> OldByNewFaces => GetCachedDictionary(ref m_oldByNewFaces);
        protected Dictionary<Face, Face> NewByOldFaces => GetCachedDictionary(ref m_newByOldFaces);
        protected IEnumerable<Vertex> OldVertices => OldByNewVertices.Keys;
        protected IEnumerable<Vertex> NewVertices => OldByNewVertices.Values;
        protected IEnumerable<Face> OldFaces => OldByNewFaces.Keys;
        protected IEnumerable<Face> NewFaces => OldByNewFaces.Values;

        protected Dictionary<T, T> GetCachedDictionary<T>(ref Dictionary<T, T> cache)
        {
            if (null == cache)
                EnsureCloneModel();

            return cache;
        }

        protected void EnsureCloneModel()
        {
            EffectsRoot.CloneModel();
            m_oldByNewVertices = EffectsRoot.OldByNewVertices;
            m_newByOldVertices = EffectsRoot.NewByOldVertices;
            m_oldByNewFaces = EffectsRoot.OldByNewFaces;
            m_newByOldFaces = EffectsRoot.NewByOldFaces;
        }

        public void Run(EffectsRoot effectsRoot)
        {
            EffectsRoot = effectsRoot;
            m_oldByNewVertices = null;
            m_newByOldVertices = null;
            m_oldByNewFaces = null;
            m_newByOldFaces = null;

            EffectImplementation();
        }

        protected abstract void EffectImplementation();
    }
}