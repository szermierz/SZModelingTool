using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace SZ.ModelingTool
{
    public class ModelingToolBehaviour : MonoBehaviour
    {
        #region Accessors

        public virtual bool Selected
        {
            get
            {
                var gameObjects = Selection.objects.OfType<GameObject>();
                var possibles = gameObjects.SelectMany(_gameObject => _gameObject.GetComponentsInChildren(GetType()));
                if (possibles.Contains(this))
                    return true;

                if (gameObjects.Any(_gameObject => _gameObject.GetComponent(GetType()) == this))
                    return true;

                return false;
            }
        }

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