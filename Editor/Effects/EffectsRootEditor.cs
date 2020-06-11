using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    [CustomEditor(typeof(EffectsRoot))]
    public sealed class EffectsRootEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var effectsRoot = target as EffectsRoot;
            if (!effectsRoot)
                return;

            GUILayout.Space(10);

            if (effectsRoot.IsValid)
            {
                if (GUILayout.Button($"Run {effectsRoot.Effects.Count()} effects"))
                    effectsRoot.RunEffects();
            }
            else
            {
                GUILayout.Label($"Fill fields correctly");
            }
        }
    }
}