using System;
using System.Linq;
using System.Text;

namespace SZ.ModelingTool
{
    public sealed class Serializer_Obj : ModelingToolBehaviour, ISerializer
    {
        public string DefaultExtension => "obj";

        public string Serialize(Model model)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"# SZ.ModelingTool {model.ModelName}");
            builder.AppendLine($"");

            var vertices = model.Vertices.ToArray();
            var faces = model.Faces.ToArray();

            foreach (var v in vertices)
                builder.AppendLine($"v {FormatFloat(v.Position.x)} {FormatFloat(v.Position.y)} {FormatFloat(v.Position.z)} 1.0");

            builder.AppendLine($"");

            foreach(var f in faces)
            {
                builder.Append($"f");
                foreach (var v in f.Vertices)
                    builder.Append($" {Array.IndexOf(vertices, v) + 1}");

                builder.Append("\n");
            }

            return builder.ToString();
        }

        private static string FormatFloat(float f)
        {
            return f.ToString("0.000").Replace(',', '.');
        }
    }
}