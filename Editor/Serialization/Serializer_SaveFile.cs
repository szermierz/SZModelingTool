using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class Serializer_SaveFile : ModelingToolBehaviour, ISerializer
    {
        [SerializeField]
        private string m_defaultExtension = "obj";

        public string Serialize(Model model, string serializedModel)
        {
            if (string.IsNullOrEmpty(serializedModel))
            {
                Debug.LogError($"Couldn't serialize model!");
                return serializedModel;
            }

            var path = EditorUtility.SaveFilePanelInProject("Save model", model.ModelName, m_defaultExtension, "Choose where to save model");
            if (string.IsNullOrEmpty(path))
                return serializedModel;

            try
            {
                File.WriteAllText(path, serializedModel);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            AssetDatabase.Refresh();

            return serializedModel;
        }
    }
}