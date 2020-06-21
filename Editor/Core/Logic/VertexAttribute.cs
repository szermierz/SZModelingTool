
namespace SZ.ModelingTool
{
    public abstract class VertexAttribute<VertexAttributeType> : ModelingToolBehaviour
        where VertexAttributeType : VertexAttribute<VertexAttributeType>
    {
        public static VertexAttributeType Create(Vertex vertex)
        {
            return vertex.gameObject.AddComponent<VertexAttributeType>();
        }
    }
}