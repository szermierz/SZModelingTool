using System;
using System.Collections.Generic;
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

            ISerializer noParent = null;
            var parenthood = new Dictionary<ISerializer, ISerializer>();
            foreach(var serializer in serializers)
            {
                if(serializer.ParentSerializer is { } parent)
                {
                    if (parenthood.ContainsKey(parent))
                        throw new Exception(
                            $"Duplicated parent serializers: (parent) {GetSerializerName(parent)}, {GetSerializerName(parenthood[parent])}, {GetSerializerName(serializer)}");

                    parenthood.Add(parent, serializer);
                }
                else
                {
                    if(null != noParent)
                        throw new Exception($"Duplicated root serializers: {GetSerializerName(noParent)}, {GetSerializerName(serializer)}");
                    
                    noParent = serializer;
                }
            }

            if (null == noParent)
                throw new Exception($"Failed to find root serializer!");

            var serializedModel = string.Empty;
            for(var serializer = noParent; null != serializer; _ = parenthood.TryGetValue(serializer, out serializer))
            {
                serializedModel = serializer.Serialize(model, serializedModel);
            }

            static string GetSerializerName(ISerializer serializer)
            {
                if (null == serializer)
                    return "null";

                var result = serializer.GetType().Name;
                if (serializer is MonoBehaviour component)
                    result = $"{result}: {component.name}";

                return result;
            }
        }
    }
}