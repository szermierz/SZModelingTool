using System;
using System.Linq;
using System.Text;

namespace SZ.ModelingTool
{
    public sealed class Serializer_Obj : ModelingToolBehaviour, ISerializer
    {
        public bool SeparateFaces = false;

        public string Serialize(Model model, string _)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"# SZ.ModelingTool {model.ModelName}");
            builder.AppendLine($"");

            builder.AppendLine($"g {model.ModelName}");
            builder.AppendLine($"");

            var vertices = model.Vertices.ToArray();
            var faces = model.Faces.ToArray();

            if (SeparateFaces)
                SerializeSeparatedFaceVertices(vertices, faces, builder);
            else
                SerializeVertices(vertices, faces, builder);

            return builder.ToString();
        }

        private void SerializeVertices(Vertex[] vertices, Face[] faces, StringBuilder builder)
        {
            foreach (var v in vertices)
                builder.AppendLine($"v {FormatFloat(v.Position.x)} {FormatFloat(v.Position.y)} {FormatFloat(v.Position.z)} 1.0");

            builder.AppendLine($"");

            foreach (var f in faces)
            {
                builder.Append($"f");
                foreach (var v in f.Vertices)
                    builder.Append($" {Array.IndexOf(vertices, v) + 1}");

                builder.Append("\n");
            }
        }

        private void SerializeSeparatedFaceVertices(Vertex[] vertices, Face[] faces, StringBuilder builder)
        {
            foreach (var f in faces)
            {
                if (f.Vertices == null || f.Vertices.Length != 3 || f.Vertices.Any(_vertex => !_vertex))
                    continue;

                for(int i = 0; i < f.Vertices.Length; ++i)
                {
                    var v = f.Vertices[i];
                    builder.AppendLine($"v {FormatFloat(v.Position.x)} {FormatFloat(v.Position.y)} {FormatFloat(v.Position.z)} 1.0");
                }

                builder.Append("\n");
            }

            builder.AppendLine($"");

            int vertexCounter = 0;
            foreach (var f in faces)
            {
                builder.Append($"f {vertexCounter + 1} {vertexCounter + 2} {vertexCounter + 3}");
                vertexCounter += 3;

                builder.Append("\n");
            }
        }

        private static string FormatFloat(float f)
        {
            return f.ToString("0.000").Replace(',', '.');
        }
    }
}