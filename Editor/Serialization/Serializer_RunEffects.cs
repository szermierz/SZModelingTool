using UnityEngine;

namespace SZ.ModelingTool
{
    public class Serializer_RunEffects : ModelingToolBehaviour, ISerializer
    {
        [SerializeField]
        private EffectsRoot m_effectsRoot;

        public string Serialize(Model model, string previous)
        {
            if (m_effectsRoot)
                m_effectsRoot.RunEffects();

            return previous;
        }
    }
}