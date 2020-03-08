using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    [CustomEditor(typeof(Model))]
    public class ModelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var model = target as Model;
            if (!model)
                return;

            if (GUILayout.Button($"Save model (as {model.ModelName})"))
                Save(model);
        }

        private void Save(Model model)
        {
            if (!model)
                return;

            var serializers = model.GetComponentsInChildren<MonoBehaviour>()
                .OfType<ISerializer>()
                .Distinct()
                .Where(_serializer => null != _serializer)
                .ToArray();

            if (serializers.Length != 1)
            {
                Debug.LogError($"Found {serializers.Length} serializers!");
                return;
            }

            var serializer = serializers.Single();

            var serializedModel = serializer.Serialize(model);
            if (string.IsNullOrEmpty(serializedModel))
            {
                Debug.LogError($"Couldn't serialize model!");
                return;
            }

            var path = EditorUtility.SaveFilePanelInProject("Save model", model.ModelName, serializer.DefaultExtension, "Choose where to save model");
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                File.WriteAllText(path, serializedModel);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }

            AssetDatabase.Refresh();
        }
    }
}