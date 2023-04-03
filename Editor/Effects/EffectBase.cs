using System.Collections.Generic;
using UnityEngine;

namespace SZ.ModelingTool
{
    [RequireComponent(typeof(EffectsRoot))]
    public abstract class EffectBase : ModelingToolBehaviour
    {
        protected EffectsRoot EffectsRoot { get; private set; }

        protected Dictionary<Vertex, Vertex> OldByNewVertices => EffectsRoot.OldByNewVertices;
        protected Dictionary<Vertex, Vertex> NewByOldVertices => EffectsRoot.NewByOldVertices;
        protected Dictionary<Face, Face> OldByNewFaces => EffectsRoot.OldByNewFaces;
        protected Dictionary<Face, Face> NewByOldFaces => EffectsRoot.NewByOldFaces;
        protected IEnumerable<Vertex> OldVertices => OldByNewVertices.Keys;
        protected IEnumerable<Vertex> NewVertices => OldByNewVertices.Values;
        protected IEnumerable<Face> OldFaces => OldByNewFaces.Keys;
        protected IEnumerable<Face> NewFaces => OldByNewFaces.Values;

        public void Run(EffectsRoot effectsRoot)
        {
            EffectsRoot = effectsRoot;
            EffectImplementation();
        }

        protected abstract void EffectImplementation();

        protected void EnsureCloneModel(bool cloneVertices, bool cloneFaces)
        {
            EffectsRoot.CloneModel(cloneVertices, cloneFaces);
        }
    }
}