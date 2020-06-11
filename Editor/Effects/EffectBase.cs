using System.Collections.Generic;

namespace SZ.ModelingTool
{
    public abstract class EffectBase : ModelingToolBehaviour
    {
        protected EffectsRoot EffectsRoot { get; private set; }

        private Dictionary<Vertex, Vertex> m_vertices = null;
        private Dictionary<Face, Face> m_faces = null;

        protected Dictionary<Vertex, Vertex> Vertices
        {
            get
            {
                if (null == m_vertices)
                    EffectsRoot.CloneModel(out m_vertices, out m_faces);

                return m_vertices;
            }
        }

        protected Dictionary<Face, Face> Faces
        {
            get
            {
                if (null == m_faces)
                    EffectsRoot.CloneModel(out m_vertices, out m_faces);

                return m_faces;
            }
        }

        public void Run(EffectsRoot effectsRoot)
        {
            EffectsRoot = effectsRoot;
            m_vertices = null;
            m_faces = null;

            EffectImplementation();
        }

        protected abstract void EffectImplementation();
    }
}