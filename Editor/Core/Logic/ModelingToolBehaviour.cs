using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace SZ.ModelingTool
{
    public class ModelingToolBehaviour : MonoBehaviour
    {
        #region PostProcess

        [PostProcessScene]
        public static void OnPostprocessScene()
        {
            var models = Resources
                .FindObjectsOfTypeAll<ModelingToolBehaviour>()
                .Where(_model => !EditorUtility.IsPersistent(_model))
                .ToArray();

            foreach (var model in models)
                DestroyImmediate(model.gameObject);
        }

        #endregion
    }
}