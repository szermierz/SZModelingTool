using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace SZ.ModelingTool
{
    public class ModelingToolBehaviour : MonoBehaviour
    {
        #region Accessors

        public virtual bool Selected => Selection.objects.Any(_object => (_object as GameObject)?.GetComponents<Component>()?.Any(_component => _component == this) ?? false);

        #endregion

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