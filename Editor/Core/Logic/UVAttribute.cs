using UnityEngine;

namespace SZ.ModelingTool
{
    public class UVAttribute : VertexAttribute<UVAttribute>
    {
		[SerializeField]
		private Vector2 m_uv = default;
		public Vector2 UV
		{
			get => m_uv;
			set => m_uv = value;
		}
    }
}